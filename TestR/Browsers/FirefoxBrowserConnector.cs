#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TestR.Extensions;
using TestR.Helpers;
using TestR.Logging;

#endregion

namespace TestR.Browsers
{
	/// <summary>
	/// Represents the connector to communicated to the Firefox browser.
	/// </summary>
	/// <exclude />
	public class FirefoxBrowserConnector : IDisposable
	{
		#region Fields

		private readonly string _hostname;
		private readonly JsonSerializerSettings _jsonSerializerSettings;
		private readonly FirefoxBuffer _messageBuffer;
		private readonly int _port;
		private readonly byte[] _socketBuffer;
		private string _consoleActor;
		private Task _readTask;
		private Socket _socket;
		private string _tabActor;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the Browser class.
		/// </summary>
		/// <param name="hostname">The hostname of the Firefox browser.</param>
		/// <param name="port">The remote debugger port to connect to.</param>
		public FirefoxBrowserConnector(string hostname, int port)
		{
			_hostname = hostname;
			_jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
			_port = port;
			_consoleActor = string.Empty;
			_messageBuffer = new FirefoxBuffer();
			_socketBuffer = new byte[FirefoxBuffer.InitialSize];
			_tabActor = string.Empty;
			Responses = new List<dynamic>();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets a flag indicating the browser has changed locations itself.
		/// </summary>
		public bool BrowserHasNavigated { get; set; }

		/// <summary>
		/// Gets the responses from the server.
		/// </summary>
		public List<dynamic> Responses { get; private set; }

		/// <summary>
		/// Gets the current URI of the browser.
		/// </summary>
		public string Uri { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Connect to the Firefox browser debugger port.
		/// </summary>
		/// <exception cref="Exception">All debugging sessions are taken.</exception>
		public void Connect()
		{
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_socket.Connect(_hostname, _port);

			_readTask = Task.Run(() =>
			{
				LogManager.Write("Firefox: Read thread is starting...", LogLevel.Verbose);

				while (ReadResponseAsync())
				{
					Thread.Sleep(10);
				}

				LogManager.Write("Firefox: Read thread is closing...", LogLevel.Verbose);
			});

			// Wait for the connect response.
			WaitForResponse(x => x.from == "root" && x.applicationType == "browser");

			// Initialize the actor references.
			InitializeActors();
		}

		/// <summary>
		/// Disconnect the connector from the browser.
		/// </summary>
		public void Disconnect()
		{
			_socket.Disconnect(true);
			_socket.Dispose();
			_socket = null;

			while (_readTask.Status == TaskStatus.Running)
			{
				Thread.Sleep(50);
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="script"></param>
		/// <returns></returns>
		public string ExecuteJavaScript(string script)
		{
			var request = new
			{
				To = _consoleActor,
				Type = "evaluateJS",
				Text = script
			};

			var response = SendRequestAndReadResponse(request, x => x.from == _consoleActor);
			var result = ((object) response.result).ToString();
			return result.Contains("\"type\": \"longString\"") ? ReadLongResponse(result) : result;
		}

		/// <summary>
		/// Get the current document in the browser.
		/// </summary>
		/// <returns>The dynamic version of the document.</returns>
		/// <exception cref="Exception"></exception>
		public string GetCurrentUri()
		{
			return ExecuteJavaScript("document.location.href");
		}

		/// <summary>
		/// Navigates the browser to the provided URI.
		/// </summary>
		/// <param name="uri">The URI to navigate the browser to.</param>
		public void NavigateTo(string uri)
		{
			// First redirect then make a second request so we get back our "action->stop" message. I expected Firefox
			// to just send us the stop but it doesn't happen unless we make another request to the browser. 
			ExecuteJavaScript("document.location.href = \"" + uri + "\"");
			SendRequest("Wake up, Neo...");
			if (!Utility.Wait(() => BrowserHasNavigated, 1000, 100))
			{
				throw new Exception("Failed to navigate to new location.");
			}
		}

		/// <summary>
		/// Refreshes the state of the connector.
		/// </summary>
		public void Refresh()
		{
			Disconnect();
			Connect();
			Uri = GetCurrentUri();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <param name="disposing">True if deposing and false if otherwise.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && _socket != null)
			{
				_socket.Dispose();
				_socket = null;
			}
		}

		private void InitializeActors()
		{
			var listTabRequest = new { To = "root", Type = "listTabs" };
			var listTabResponse = SendRequestAndReadResponse(listTabRequest, x => x.from == "root" && x.tabs != null);
			var selected = listTabResponse.tabs[(int) listTabResponse.selected];
			_consoleActor = selected.consoleActor;
			_tabActor = selected.actor;

			var attachTabRequest = new { To = _tabActor, Type = "attach" };
			SendRequestAndReadResponse(attachTabRequest, x => x.from == _tabActor && x.type == "tabAttached");
		}

		private string ReadLongResponse(string response)
		{
			var result = (dynamic) JsonConvert.DeserializeObject(response);
			if (result.type != "longString")
			{
				throw new Exception("This response was not a long string response.");
			}

			var length = (int) result.length;
			var builder = new StringBuilder(length);
			var offset = builder.Length;
			var chuckLength = 131070;
			var actor = (string) result.actor;

			while (offset < length)
			{
				SendRequest(new { To = actor, Type = "substring", Start = offset, End = offset + chuckLength });
				var subresult = WaitForResponse(x => x.@from == actor && x.substring != null);
				builder.Append((string) subresult.substring);
				offset += chuckLength;
			}

			return builder.ToString();
		}

		private bool ReadResponseAsync()
		{
			try
			{
				if (_socket == null)
				{
					return false;
				}

				var result = _socket.Receive(_socketBuffer);
				_messageBuffer.Add(_socketBuffer, result);

				var messages = _messageBuffer.GetMessages();
				foreach (var message in messages)
				{
					try
					{
						var token = message.AsJToken() as dynamic;
						LogManager.Write("Debugger Response: " + message, LogLevel.Verbose);

						if (message.Contains("\"type\":\"tabNavigated\""))
						{
							BrowserHasNavigated = message.Contains("\"state\":\"stop\"");
							continue;
						}

						Responses.Add(token);
					}
					catch
					{
						LogManager.Write("Invalid message! -> " + messages, LogLevel.Fatal);
					}
				}
				return true;
			}
			catch (ObjectDisposedException)
			{
				return false;
			}
			catch (SocketException)
			{
				return false;
			}
			catch (Exception ex)
			{
				LogManager.Write(ex.Message, LogLevel.Fatal);
				return false;
			}
		}

		private void SendRequest<T>(T request)
		{
			var json = JsonConvert.SerializeObject(request, _jsonSerializerSettings);
			var data = json.Length + ":" + json;
			LogManager.Write("Debugger Request: " + data, LogLevel.Verbose);
			var jsonBuffer = Encoding.UTF8.GetBytes(data);
			_socket.Send(jsonBuffer);
		}

		private dynamic SendRequestAndReadResponse(dynamic request, Func<dynamic, bool> action)
		{
			SendRequest(request);
			return WaitForResponse(action);
		}

		private dynamic WaitForResponse(Func<dynamic, bool> action)
		{
			var response = Utility.Retry(() => Responses.First(action), 10, 100, "Failed to get a response...");
			Responses.Remove(response);
			return response;
		}

		#endregion

		#region Classes

		private class FirefoxBuffer
		{
			#region Constants

			public const int InitialSize = 131070;

			#endregion

			#region Fields

			private readonly List<byte> _buffer;

			#endregion

			#region Constructors

			public FirefoxBuffer()
			{
				_buffer = new List<byte>(InitialSize);
			}

			#endregion

			#region Methods

			public void Add(IEnumerable<byte> data)
			{
				_buffer.AddRange(data);
			}

			public void Add(byte[] buffer, int length)
			{
				var data = new byte[length];
				Array.Copy(buffer, 0, data, 0, data.Length);
				_buffer.AddRange(data);
			}

			public IEnumerable<string> GetMessages()
			{
				var response = new List<string>();
				var message = ReadNextMessage();

				while (message != null)
				{
					response.Add(message);
					message = ReadNextMessage();
				}

				return response;
			}

			private byte[] Read(int index, int length)
			{
				if (index >= _buffer.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}

				if (length > _buffer.Count)
				{
					throw new ArgumentOutOfRangeException("length");
				}

				if (index + length > _buffer.Count)
				{
					throw new ArgumentOutOfRangeException("length");
				}

				var response = new byte[length];
				Array.Copy(_buffer.ToArray(), index, response, 0, response.Length);
				return response;
			}

			private string ReadNextMessage()
			{
				var index = _buffer.IndexOf(58);
				if (index == -1)
				{
					return null;
				}

				var lengthString = Encoding.UTF8.GetString(Read(0, index));
				var length = int.Parse(lengthString);
				if (_buffer.Count < index + 1 + length)
				{
					return null;
				}

				var response = Encoding.UTF8.GetString(Read(index + 1, length));
				_buffer.RemoveRange(0, index + 1 + length);
				return response;
			}

			#endregion
		}

		#endregion
	}
}
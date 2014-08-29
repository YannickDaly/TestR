#region References

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using TestR.Extensions;
using TestR.Helpers;

#endregion

namespace TestR.Browsers
{
	/// <summary>
	/// Represents the connector to communicated to the Chrome browser.
	/// </summary>
	/// <exclude />
	public class ChromeBrowserConnector : IDisposable
	{
		#region Constants

		private const string JsonPostfix = "/json";

		#endregion

		#region Fields

		private readonly JsonSerializerSettings _jsonSerializerSettings;
		private readonly List<dynamic> _socketResponses;
		private readonly string _uri;
		private int _requestId;
		private ClientWebSocket _socket;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the Browser class.
		/// </summary>
		/// <param name="uri"></param>
		public ChromeBrowserConnector(string uri)
		{
			_jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
			_socketResponses = new List<dynamic>();
			_uri = uri;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets a flag indicating the browser has changed locations itself.
		/// </summary>
		public bool BrowserHasNavigated { get; set; }

		/// <summary>
		/// Gets the current URI of the browser.
		/// </summary>
		public string Uri { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Connect to the Chrome browser debugger port.
		/// </summary>
		/// <exception cref="Exception">All debugging sessions are taken.</exception>
		public void Connect()
		{
			var sessions = GetAvailableSessions();
			if (sessions.Count == 0)
			{
				throw new Exception("All debugging sessions are taken.");
			}

			var session = sessions.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.WebSocketDebuggerUrl));
			if (session == null)
			{
				throw new Exception("Could not find a valid debugger enabled page. Make sure you close the debugger tools.");
			}

			var sessionWsEndpoint = new Uri(session.WebSocketDebuggerUrl);
			_socket = new ClientWebSocket();

			if (!_socket.ConnectAsync(sessionWsEndpoint, CancellationToken.None).Wait(5000))
			{
				throw new Exception("Failed to connect to the server.");
			}

			Task.Run(() =>
			{
				while (ReadResponseAsync())
				{
					Thread.Sleep(10);
				}
			});
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
				Id = _requestId++,
				Method = "Runtime.evaluate",
				Params = new
				{
					DoNotPauseOnExceptions = true,
					Expression = script,
					ObjectGroup = "console",
					IncludeCommandLineAPI = true,
					ReturnByValue = true
				}
			};

			var data = SendRequestAndReadResponse(request, x => x.id == request.Id);
			var response = data.AsJToken() as dynamic;
			if (response.result == null || response.result.result == null)
			{
				return data;
			}

			var value = response.result.result.value;
			if (value == null)
			{
				return string.Empty;
			}

			var typeName = value.GetType().Name;
			return typeName != "JValue"
				? JsonConvert.SerializeObject(value)
				: (string) value;
		}

		/// <summary>
		/// Navigates the browser to the provided URI.
		/// </summary>
		/// <param name="uri">The URI to navigate the browser to.</param>
		public void NavigateTo(string uri)
		{
			var request = new
			{
				Id = _requestId++,
				Method = "Page.navigate",
				Params = new
				{
					Url = uri
				}
			};

			SendRequestAndReadResponse(request, x => x.id == request.Id);
			Refresh();
		}

		/// <summary>
		/// Reloads the current page.
		/// </summary>
		public void ReLoad()
		{
			var request = new
			{
				Id = _requestId++,
				Method = "Page.reload",
				Params = new
				{
					ignoreCache = true
				}
			};

			SendRequestAndReadResponse(request, x => x.id == request.Id);
			Refresh();
		}

		/// <summary>
		/// Refreshes the state of the connector.
		/// </summary>
		public void Refresh()
		{
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

		/// <summary>
		/// Get the current document in the browser.
		/// </summary>
		/// <returns>The dynamic version of the document.</returns>
		/// <exception cref="Exception"></exception>
		public string GetCurrentUri()
		{
			var request = new
			{
				Id = _requestId++,
				Method = "DOM.getDocument"
			};

			return Utility.Retry(() =>
			{
				var document = SendRequestAndReadResponse(request, x => x.id == request.Id).AsJToken() as dynamic;
				var body = FindBody(document.result.root);
				if (body == null)
				{
					throw new Exception("Failed to get the URI.");
				}

				return document.result.root.documentURL;
			}, 4, 250);
		}

		private dynamic FindBody(dynamic node)
		{
			if (node.nodeName == "BODY" && node.childNodeCount > 0)
			{
				return node;
			}

			if (node.children == null)
			{
				return null;
			}

			foreach (var child in node.children)
			{
				var response = FindBody(child);
				if (response != null)
				{
					return response;
				}
			}

			return null;
		}

		private List<RemoteSessionsResponse> GetAvailableSessions()
		{
			var request = (HttpWebRequest) WebRequest.Create(_uri + JsonPostfix);
			using (var response = Utility.Retry<WebResponse>(request.GetResponse))
			{
				var stream = response.GetResponseStream();
				if (stream == null)
				{
					throw new Exception("Failed to get a response.");
				}

				using (var reader = new StreamReader(stream))
				{
					var sessions = JsonConvert.DeserializeObject<List<RemoteSessionsResponse>>(reader.ReadToEnd());
					sessions.RemoveAll(x => x.Url.StartsWith("chrome-extension"));
					sessions.RemoveAll(x => x.Url.StartsWith("chrome-devtools"));
					return sessions;
				}
			}
		}

		private bool ReadResponseAsync()
		{
			var buffer = new ArraySegment<byte>(new byte[131072]);
			var builder = new StringBuilder();

			try
			{
				WebSocketReceiveResult result;

				do
				{
					result = _socket.ReceiveAsync(buffer, CancellationToken.None).Result;
					var data = new byte[result.Count];
					Array.Copy(buffer.Array, 0, data, 0, data.Length);
					builder.Append(Encoding.UTF8.GetString(data));
				} while (!result.EndOfMessage);

				var response = builder.ToString();
				Logger.Write("Debugger Response: " + response, LogLevel.Trace);

				if (response == "{\"method\":\"DOM.documentUpdated\"}")
				{
					BrowserHasNavigated = true;
					return true;
				}

				_socketResponses.Add(response.AsJToken() as dynamic);
				return true;
			}
			catch (ObjectDisposedException)
			{
				return false;
			}
			catch (AggregateException)
			{
				return false;
			}
		}

		private void SendRequest<T>(T request)
		{
			var json = JsonConvert.SerializeObject(request, _jsonSerializerSettings);
			Logger.Write("Debugger Request: " + json, LogLevel.Trace);
			var jsonBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
			_socket.SendAsync(jsonBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
		}

		private string SendRequestAndReadResponse(dynamic request, Func<dynamic, bool> action)
		{
			SendRequest(request);
			var response = Utility.Retry(() => _socketResponses.First(action), 50, 100, "Failed to get a response...");
			_socketResponses.Remove(response);
			return response.ToString();
		}

		#endregion

		#region Classes

		[Serializable]
		[DataContract]
		internal class RemoteSessionsResponse
		{
			#region Properties

			[DataMember]
			public string DevtoolsFrontendUrl { get; set; }

			[DataMember]
			public string FaviconUrl { get; set; }

			[DataMember]
			public string ThumbnailUrl { get; set; }

			[DataMember]
			public string Title { get; set; }

			[DataMember]
			public string Url { get; set; }

			[DataMember]
			public string WebSocketDebuggerUrl { get; set; }

			#endregion
		}

		#endregion
	}
}
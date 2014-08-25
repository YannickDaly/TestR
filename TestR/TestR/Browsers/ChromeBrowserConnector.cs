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

		private readonly ChromeBrowser _browser;
		private readonly JsonSerializerSettings _jsonSerializerSettings;
		private readonly List<dynamic> _socketResponses;
		private readonly string _uri;
		private string _currentUrl;
		private int _requestId;
		private ClientWebSocket _socket;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the Browser class.
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="browser"></param>
		public ChromeBrowserConnector(string uri, ChromeBrowser browser)
		{
			_browser = browser;
			_currentUrl = string.Empty;
			_jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
			_socketResponses = new List<dynamic>();
			_uri = uri;

			Elements = new List<ChromeElement>();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a list of elements for the current loaded page.
		/// </summary>
		public List<ChromeElement> Elements { get; private set; }

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

			if (!_socket.ConnectAsync(sessionWsEndpoint, CancellationToken.None).Wait(500))
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
		public string ExecuteJavascript(string script)
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

			return response.result.result.value ?? string.Empty;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="element"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public string GetProperty(ChromeElement element, string name)
		{
			// Get property by id.
			return ExecuteJavascript("document.getElementById('" + element.Id + "')." + name);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string GetUri()
		{
			return _currentUrl;
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
			var body = Utility.Retry(() => GetDocument(), 5, 500);
			AddElementAndChildren(body, Elements);
		}

		/// <summary>
		/// Type character into the browser.
		/// </summary>
		/// <param name="key">The key to be typed.</param>
		public void SendKey(string key)
		{
			var request = new
			{
				Id = _requestId++,
				Method = "Input.dispatchKeyEvent",
				Params = new
				{
					Text = key,
					UnmodifiedText = key,
					Type = "keyDown",
				}
			};

			SendRequestAndReadResponse(request, x => x.id == request.Id);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="element"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public string SetAttribute(ChromeElement element, string name, string value)
		{
			var request = new
			{
				Id = _requestId++,
				Method = "DOM.setAttributeValue",
				Params = new
				{
					element.NodeId,
					Name = name,
					Value = value,
				}
			};

			return SendRequestAndReadResponse(request, x => x.id == request.Id || x.method == "DOM.attributeModified");
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

		private void AddElementAndChildren(dynamic node, ICollection<ChromeElement> collection)
		{
			if (node.nodeType != 1 || node.nodeName == "SCRIPT")
			{
				return;
			}

			var element = new ChromeElement(node, _browser);
			element.Initialize();

			collection.Add(element);

			if (node.childNodeCount <= 0)
			{
				return;
			}

			if (node.children != null)
			{
				foreach (var item in node.children)
				{
					AddElementAndChildren(item, collection);
				}

				return;
			}

			var result = RequestChildNodes((int) node.nodeId).AsJToken() as dynamic;
			var nodes = result["params"].nodes;

			foreach (var childItem in nodes)
			{
				AddElementAndChildren(childItem, collection);
			}
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
			using (var response = Utility.Retry(request.GetResponse, 10, 100))
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

		private dynamic GetDocument()
		{
			var request = new
			{
				Id = _requestId++,
				Method = "DOM.getDocument"
			};

			var document = SendRequestAndReadResponse(request, x => x.id == request.Id).AsJToken() as dynamic;
			var body = FindBody(document.result.root);
			if (body == null)
			{
				throw new Exception("Failed to get the body.");
			}

			_currentUrl = document.result.root.documentURL;
			return body;
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
				_socketResponses.Add(response.AsJToken() as dynamic);

				return true;
			}
			catch (AggregateException)
			{
				return false;
			}
		}

		private string RequestChildNodes(int nodeId)
		{
			var request = new
			{
				Id = _requestId++,
				Method = "DOM.requestChildNodes",
				Params = new
				{
					NodeId = nodeId
				}
			};

			return SendRequestAndReadResponse(request, x => x.method == "DOM.setChildNodes");
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
			var response = Utility.Retry(() => _socketResponses.First(action), 20, 100, "Failed to get a response...");
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
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
using Newtonsoft.Json;

#endregion

namespace TestR.Browsers
{
	public class ChromeBrowserConnector : IDisposable
	{
		#region Constants

		private const string JsonPostfix = "/json";

		#endregion

		#region Fields

		private readonly string _uri;
		private ClientWebSocket _socket;

		#endregion

		#region Constructors

		public ChromeBrowserConnector(string uri)
		{
			_uri = uri;
		}

		#endregion

		#region Methods

		public void Connect()
		{
			var sessions = GetAvailableSessions();
			if (sessions.Count == 0)
			{
				throw new Exception("All debugging sessions are taken.");
			}

			var sessionWsEndpoint = new Uri(sessions[0].WebSocketDebuggerUrl);
			_socket = new ClientWebSocket();
			_socket.ConnectAsync(sessionWsEndpoint, CancellationToken.None).Wait(5000);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public string GetChildren(int nodeId)
		{
			var json = @"{""id"": 1,""method"":""DOM.requestChildNodes"",""params"": {""nodeId"": " + nodeId + "}}";
			return SendCommand(json);
		}

		public string GetDocument()
		{
			var json = @"{""id"": 1,""method"":""DOM.getDocument""}";
			return SendCommand(json);
		}

		public string NavigateTo(string uri)
		{
			// Instead of Page.navigate, we can use document.location
			var json = @"{""method"":""Runtime.evaluate"",""params"":{""expression"":""document.location='" + uri + @"'"",""objectGroup"":""console"",""includeCommandLineAPI"":true,""doNotPauseOnExceptions"":false,""returnByValue"":false},""id"":1}";
			return SendCommand(json);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && _socket != null)
			{
				_socket.Dispose();
				_socket = null;
			}
		}

		private T Deserialise<T>(string json)
		{
			return JsonConvert.DeserializeObject<T>(json);
		}

		private List<RemoteSessionsResponse> GetAvailableSessions()
		{
			var res = SendRequest<List<RemoteSessionsResponse>>();
			return (from r in res
				where r.DevtoolsFrontendUrl != null
				select r).ToList();
		}

		private string SendCommand(string json)
		{
			var jsonBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(json));
			var buffer = new ArraySegment<byte>(new byte[4096]);
			_socket.SendAsync(jsonBuffer, WebSocketMessageType.Text, true, CancellationToken.None).Wait();
			var result = _socket.ReceiveAsync(buffer, CancellationToken.None).Result;
			var data = new byte[result.Count];
			Array.Copy(buffer.Array, 0, data, 0, data.Length);
			return Encoding.UTF8.GetString(data);
		}

		private T SendRequest<T>()
		{
			var request = (HttpWebRequest) WebRequest.Create(_uri + JsonPostfix);
			using (var response = request.GetResponse())
			{
				var stream = response.GetResponseStream();
				if (stream == null)
				{
					throw new Exception("Failed to get a response.");
				}

				using (var reader = new StreamReader(stream))
				{
					return Deserialise<T>(reader.ReadToEnd());
				}
			}
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
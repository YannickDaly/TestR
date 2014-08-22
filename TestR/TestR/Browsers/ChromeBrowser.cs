#region References

using System;
using System.Diagnostics;
using TestR.Collections;

#endregion

namespace TestR.Browsers
{
	/// <summary>
	/// This is the place hold for what may be Chrome support.
	/// </summary>
	/// <exclude />
	public class ChromeBrowser : Browser
	{
		#region Fields

		private ChromeBrowserConnector _connector;
		private Process _process;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the Chrome class.
		/// </summary>
		public ChromeBrowser()
		{
			CreateInstance();
			ConnectInstance();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the current active element.
		/// </summary>
		public override Element ActiveElement
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets a list of all elements on the current page.
		/// </summary>
		public override ElementCollection Elements
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets the ID of the browser.
		/// </summary>
		public override int Id
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets the URI of the current page.
		/// </summary>
		public override string Uri
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Gets the window handle of the current browser.
		/// </summary>
		/// <value>Window handle of the current browser.</value>
		protected override IntPtr WindowHandle
		{
			get { throw new NotImplementedException(); }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Execute JavaScript code in the current document.
		/// </summary>
		/// <param name="script"></param>
		/// <returns></returns>
		public override string ExecuteJavascript(string script)
		{
			return string.Empty;
		}

		/// <summary>
		/// Navigates the browser to the provided URI.
		/// </summary>
		/// <param name="uri">The URI to navigate to.</param>
		public override void NavigateTo(string uri)
		{
			_connector.NavigateTo(uri);
		}

		/// <summary>
		/// Waits until the browser to complete any outstanding operations.
		/// </summary>
		public override void WaitForComplete()
		{
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <param name="disposing">True if disposing and false if otherwise.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_connector != null)
				{
					_connector.Dispose();
				}

				if (_process != null)
				{
					_process.CloseMainWindow();
					_process.Close();
					_process.Dispose();
					_process = null;
				}
			}

			base.Dispose(disposing);
		}

		private void ConnectInstance()
		{
			_connector = new ChromeBrowserConnector("http://localhost:9222");
			_connector.Connect();
		}

		private void CreateInstance()
		{
			var info = new ProcessStartInfo("chrome.exe");
			info.Arguments = "--remote-debugging-port=9222";
			info.WindowStyle = ProcessWindowStyle.Normal;
			info.UseShellExecute = true;

			_process = new Process();
			_process.StartInfo = info;
			_process.Exited += (sender, args) =>
			{
				_process.Dispose();
				_process = null;
			};

			if (!_process.Start())
			{
				throw new Exception("Failed to start the process. ExitCode: " + _process.ExitCode);
			}
		}

		#endregion
	}
}
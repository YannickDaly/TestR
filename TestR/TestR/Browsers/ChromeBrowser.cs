#region References

using System;
using System.Diagnostics;
using TestR.Helpers;

#endregion

namespace TestR.Browsers
{
	/// <summary>
	/// This is the place hold for what may be Chrome support.
	/// </summary>
	/// <exclude />
	public class ChromeBrowser : Browser
	{
		#region Constants

		/// <summary>
		/// The name of the browser. 
		/// </summary>
		public const string Name = "chrome";

		private const string DebugArgument = "--remote-debugging-port=9222";

		#endregion

		#region Fields

		private Window _window;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the ChromeBrowser class.
		/// </summary>
		public ChromeBrowser()
			: this(Create())
		{
		}

		/// <summary>
		/// Initializes a new instance of the ChromeBrowser class.
		/// </summary>
		/// <param name="process">The process of the existing browser.</param>
		public ChromeBrowser(Process process)
			: this(new Window(process))
		{
		}

		/// <summary>
		/// Initializes a new instance of the ChromeBrowser class.
		/// </summary>
		/// <param name="window">The window of the existing browser.</param>
		public ChromeBrowser(Window window)
		{
			_window = window;
			Connector = new ChromeBrowserConnector("http://localhost:9222");
			Connector.Connect();
		}

		#endregion

		#region Properties

		/// <summary>
		/// The connector to communicate with the browser.
		/// </summary>
		public ChromeBrowserConnector Connector { get; private set; }

		/// <summary>
		/// Gets the ID of the browser.
		/// </summary>
		public override int Id
		{
			get { return _window.Handle.ToInt32(); }
		}

		/// <summary>
		/// Gets the URI of the current page.
		/// </summary>
		public override string Uri
		{
			get { return Connector.GetUri(); }
		}

		/// <summary>
		/// Gets the window handle of the current browser.
		/// </summary>
		/// <value>Window handle of the current browser.</value>
		protected override IntPtr WindowHandle
		{
			get { return _window.Handle; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Move the window and resize it.
		/// </summary>
		/// <param name="x">The x coordinate to move to.</param>
		/// <param name="y">The y coordinate to move to.</param>
		/// <param name="width">The width of the window.</param>
		/// <param name="height">The height of the window.</param>
		public override void MoveWindow(int x, int y, int width, int height)
		{
			_window.MoveWindow(x, y, width, height);
		}

		/// <summary>
		/// Navigates the browser to the provided URI.
		/// </summary>
		/// <param name="uri">The URI to navigate to.</param>
		public override void NavigateTo(string uri)
		{
			Connector.NavigateTo(uri);
			Utility.Retry(() => Connector.GetUri(), 10, 500);
			Refresh();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <param name="disposing">True if disposing and false if otherwise.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (Connector != null)
				{
					Connector.Dispose();
				}

				if (AutoClose && _window != null)
				{
					_window.Close();
				}

				if (_window != null)
				{
					_window.Dispose();
					_window = null;
				}
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Execute JavaScript code in the current document.
		/// </summary>
		/// <param name="script"></param>
		/// <returns></returns>
		protected override string ExecuteJavaScript(string script)
		{
			return Connector.ExecuteJavaScript(script);
		}

		/// <summary>
		/// Injects the test script into the browser.
		/// </summary>
		protected override void InjectTestScript()
		{
			ExecuteJavaScript(GetTestScript());
		}

		/// <summary>
		/// Check to see if the browser has changed if so process the changes.
		/// </summary>
		protected override void Reconcile()
		{
			if (!Connector.BrowserHasNavigated)
			{
				return;
			}

			Refresh();
			Connector.BrowserHasNavigated = false;
		}

		private void Refresh()
		{
			InjectTestScript();
			DetectJavascriptLibraries();
			GetElementsFromScript();
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Attempts to attach to an existing browser.
		/// </summary>
		/// <returns>The browser instance or null if not found.</returns>
		public static Browser Attach()
		{
			var window = Window.FindWindow(Name, DebugArgument);
			return window != null ? new ChromeBrowser(window) : null;
		}

		/// <summary>
		/// Attempts to attach to an existing browser. If one is not found then create and return a new one.
		/// </summary>
		/// <returns>The browser instance.</returns>
		public static Browser AttachOrCreate()
		{
			return Attach() ?? new ChromeBrowser(Create());
		}

		/// <summary>
		/// Attempts to create a new browser. If one is not found then we'll make sure it was started with the 
		/// remote debugger argument. If not an exception will be thrown.
		/// </summary>
		/// <returns>The browser instance.</returns>
		private static Process Create()
		{
			var window1 = Window.FindWindow(Name);
			var window2 = Window.FindWindow(Name, DebugArgument);
			if (window1 != null && window2 == null)
			{
				throw new Exception("The first instance of Chrome was not started with the remote debugger enabled.");
			}

			// Create a new instance and return it.
			return CreateInstance(string.Format("{0}.exe", Name), DebugArgument);
		}

		#endregion
	}
}
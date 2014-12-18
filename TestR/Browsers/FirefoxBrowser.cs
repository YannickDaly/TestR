#region References

using System;
using System.Diagnostics;

#endregion

namespace TestR.Browsers
{
	/// <summary>
	/// This is the place hold for what may be FireFox support.
	/// </summary>
	/// <exclude />
	public class FirefoxBrowser : Browser
	{
		#region Constants

		/// <summary>
		/// The name of the browser. 
		/// </summary>
		public const string Name = "firefox";

		#endregion

		#region Fields

		private Window _window;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the FirefoxBrowser class.
		/// </summary>
		public FirefoxBrowser()
			: this(Create())
		{
		}

		/// <summary>
		/// Initializes a new instance of the FirefoxBrowser class.
		/// </summary>
		/// <param name="process">The process of the existing browser.</param>
		public FirefoxBrowser(Process process)
			: this(new Window(process))
		{
		}

		/// <summary>
		/// Initializes a new instance of the FirefoxBrowser class.
		/// </summary>
		/// <param name="window">The window of the existing browser.</param>
		public FirefoxBrowser(Window window)
		{
			_window = window;
			Connector = new FirefoxBrowserConnector("localhost", 6000, Timeout);
			Connector.Connect();
		}

		#endregion

		#region Properties

		/// <summary>
		/// The connector to communicate with the browser.
		/// </summary>
		public FirefoxBrowserConnector Connector { get; private set; }

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
			get
			{
				Reconcile();
				return Connector.Uri;
			}
		}

		/// <summary>
		/// Gets the type of the browser.
		/// </summary>
		internal override BrowserType Type
		{
			get { return BrowserType.Firefox; }
		}

		/// <summary>
		/// Gets or sets a flag indicating the browser has navigated to another page.
		/// </summary>
		protected override bool BrowserHasNavigated
		{
			get { return Connector.BrowserHasNavigated; }
			set { Connector.BrowserHasNavigated = value; }
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
		/// Refreshed the state of the browser.
		/// </summary>
		public override void Refresh()
		{
			Connector.Refresh();
			InjectTestScript();
			DetectJavascriptLibraries();
			RefreshElements();
		}

		/// <summary>
		/// Reads the current URI directly from the browser.
		/// </summary>
		/// <returns>The current URI that was read from the browser.</returns>
		protected override string BrowserGetUri()
		{
			return Connector.GetCurrentUri();
		}

		/// <summary>
		/// Navigates the browser to the provided URI.
		/// </summary>
		/// <param name="uri">The URI to navigate to.</param>
		protected override void BrowserNavigateTo(string uri)
		{
			Connector.NavigateTo(uri);
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

		#endregion

		#region Static Methods

		/// <summary>
		/// Attempts to attach to an existing browser.
		/// </summary>
		/// <returns>The browser instance or null if not found.</returns>
		public static Browser Attach()
		{
			var window = Window.FindWindow(Name);
			if (window == null)
			{
				return null;
			}

			var browser = new FirefoxBrowser(window);
			browser.Refresh();
			return browser;
		}

		/// <summary>
		/// Attempts to attach to an existing browser. If one is not found then create and return a new one.
		/// </summary>
		/// <returns>The browser instance.</returns>
		public static Browser AttachOrCreate()
		{
			return Attach() ?? new FirefoxBrowser(Create());
		}

		/// <summary>
		/// Attempts to create a new browser.
		/// </summary>
		/// <remarks>
		/// The Firefox browser must have the "listen 6000" command run in the console to enable remote debugging. A newly created
		/// browser will not be able to connect until someone manually starts the remote debugger.
		/// </remarks>
		/// <returns>The browser instance.</returns>
		public static Process Create()
		{
			// Create a new instance and return it.
			return CreateInstance(string.Format("{0}.exe", Name));
		}

		#endregion
	}
}
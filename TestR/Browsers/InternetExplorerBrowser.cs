#region References

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Expando;
using System.Threading;
using mshtml;
using NLog;
using SHDocVw;
using TestR.Helpers;

#endregion

namespace TestR.Browsers
{
	/// <summary>
	/// Represents an Internet Explorer browser.
	/// </summary>
	public class InternetExplorerBrowser : Browser
	{
		#region Constants

		/// <summary>
		/// The name of the browser.
		/// </summary>
		public const string Name = "iexplore";

		#endregion

		#region Fields

		private readonly Window _window;
		private InternetExplorer _browser;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the InternetExplorerBrowser class.
		/// </summary>
		public InternetExplorerBrowser()
			: this(CreateInternetExplorerClass())
		{
			Attached = false;
		}

		private InternetExplorerBrowser(InternetExplorer browser)
		{
			_browser = browser;
			_browser.DocumentComplete += BrowserOnDocumentComplete;
			_browser.Visible = true;
			_window = new Window(new IntPtr(_browser.HWND), Name);
			Attached = true;
		}

		#endregion

		#region Properties

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
			get { return BrowserGetUri(); }
		}

		/// <summary>
		/// Gets the type of the browser.
		/// </summary>
		internal override BrowserType Type
		{
			get { return BrowserType.InternetExplorer; }
		}

		/// <summary>
		/// Gets or sets a flag indicating the browser has navigated to another page.
		/// </summary>
		protected override bool BrowserHasNavigated { get; set; }

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
		/// Reads the current URI directly from the browser.
		/// </summary>
		/// <returns>The current URI that was read from the browser.</returns>
		protected override string BrowserGetUri()
		{
			Reconcile();
			WaitForComplete();
			return _browser.LocationURL;
		}

		/// <summary>
		/// Navigates the browser to the provided URI.
		/// </summary>
		/// <param name="uri">The URI to navigate to.</param>
		protected override void BrowserNavigateTo(string uri)
		{
			Logger.Write("InternetExplorerBrowser.NavigateTo(" + uri + ")", LogLevel.Trace);
			_browser.Navigate(uri);
			WaitForComplete();
			Refresh();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <param name="disposing">True if disposing and false if otherwise.</param>
		protected override void Dispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}

			try
			{
				if (_browser == null || !AutoClose)
				{
					return;
				}

				WaitForComplete();

				// We cannot allow the browser to close within a second.
				// I assume that add ons need time to start before closing the browser.
				var timeout = TimeSpan.FromMilliseconds(1000);
				while (Uptime <= timeout)
				{
					Thread.Sleep(50);
				}

				_browser.Quit();
			}
			catch
			{
				_browser = null;
			}
		}

		/// <summary>
		/// Execute JavaScript code in the current document.
		/// </summary>
		/// <param name="script">The script to run.</param>
		/// <returns>The response when executing.</returns>
		protected override string ExecuteJavaScript(string script)
		{
			var document = _browser.Document as IHTMLDocument2;
			if (document == null)
			{
				throw new Exception("Failed to run script because no document is loaded.");
			}

			Logger.Write("Request: " + script, LogLevel.Trace);
			var wrappedScript = "try { var result = eval('" + script.Replace("'", "\\'") + "'); document.executeResult = String(result); }" +
				" catch (error) { document.executeResult = error.message; }";

			document.parentWindow.execScript(wrappedScript, "javascript");
			return GetJavascriptResult();
		}

		/// <summary>
		/// Injects the test script into the browser.
		/// </summary>
		protected override void InjectTestScript()
		{
			var document = Utility.Retry(() =>
			{
				var htmlDocument = _browser.Document as IHTMLDocument2;
				if (htmlDocument == null)
				{
					throw new Exception("Failed to run script because no document is loaded.");
				}

				return htmlDocument;
			});

			var window = document.parentWindow;
			window.execScript(GetTestScript(), "javascript");
			var test = ExecuteJavaScript("typeof TestR");
			if (!test.Equals("object"))
			{
				throw new Exception("Failed to inject the TestR JavaScript. Eh? " + test);
			}
		}

		/// <summary>
		/// Refreshed the state of the browser.
		/// </summary>
		protected override void Refresh()
		{
			Logger.Write("InternetExplorerBrowser.Refresh", LogLevel.Trace);
			WaitForComplete();
			InjectTestScript();
			DetectJavascriptLibraries();
			RefreshElements();
		}

		private void BrowserOnDocumentComplete(object pDisp, ref object url)
		{
			BrowserHasNavigated = true;
		}

		private string GetJavascriptResult()
		{
			try
			{
				var expando = (IExpando) _browser.Document;
				var propertyInfo = expando.GetProperty("executeResult", BindingFlags.Default);
				var property = propertyInfo.GetValue(expando, null);
				var result = property == null ? string.Empty : property.ToString();
				Logger.Write("Response: " + result, LogLevel.Trace);
				return result;
			}
			catch
			{
				// The document may have been redirected which means the member will not be there.
				return string.Empty;
			}
		}

		/// <summary>
		/// Waits until the browser to complete any outstanding operations.
		/// </summary>
		private void WaitForComplete()
		{
			Logger.Write("InterenetExploreBrowser.WaitForComplete", LogLevel.Trace);

			if (_browser == null)
			{
				return;
			}

			var states = new[] { tagREADYSTATE.READYSTATE_COMPLETE, tagREADYSTATE.READYSTATE_UNINITIALIZED };
			if (!Utility.Wait(() => !_browser.Busy && states.Contains(_browser.ReadyState), 2500))
			{
				throw new Exception("The browser never finished loading...");
			}

			if (_browser.ReadyState == tagREADYSTATE.READYSTATE_UNINITIALIZED)
			{
				return;
			}

			var document = _browser.Document as IHTMLDocument2;
			if (document == null)
			{
				return;
			}

			if (!Utility.Wait(() => document.readyState == "complete", 2500))
			{
				throw new Exception("The document never finished loading...");
			}
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Attempts to attach to an existing browser.
		/// </summary>
		/// <returns>An instance of an Internet Explorer browser.</returns>
		public static InternetExplorerBrowser Attach()
		{
			var foundBrowsers = new ShellWindowsClass().Cast<InternetExplorer>().ToList();
			if (foundBrowsers.Count <= 0)
			{
				return null;
			}

			var foundBrowser = foundBrowsers
				.Where(x => x.Visible && x.HWND != 0 && !x.Busy)
				.FirstOrDefault(x => x.ReadyState != tagREADYSTATE.READYSTATE_LOADING);

			return foundBrowser == null ? null : new InternetExplorerBrowser(foundBrowser);
		}

		/// <summary>
		/// Attempts to attach to an existing browser. If one is not found then create and return a new one.
		/// </summary>
		/// <returns>An instance of an Internet Explorer browser.</returns>
		public static InternetExplorerBrowser AttachOrCreate()
		{
			return Attach() ?? Create();
		}

		/// <summary>
		/// Clears the cookies for the provided URL. If the URL is an empty string then all cookies will be cleared.
		/// </summary>
		/// <param name="url">The URL of the cookies to remove. Empty string removes all cookies.</param>
		/// <exception cref="ArgumentNullException">The URL parameter cannot be null.</exception>
		public static void ClearCookies(string url = "")
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}

			var path = Environment.GetFolderPath(Environment.SpecialFolder.Cookies);
			var files = Directory.GetFiles(path)
				.Union(Directory.GetFiles(path + "\\low"))
				.ToList();

			if (string.IsNullOrWhiteSpace(url))
			{
				files.ForEach(File.Delete);
				return;
			}

			foreach (var file in files)
			{
				var text = File.ReadAllText(file);
				if (!text.Contains(url))
				{
					continue;
				}

				File.Delete(file);
			}
		}

		/// <summary>
		/// Creates a new instance of an Internet Explorer browser.
		/// </summary>
		/// <returns>An instance of an Internet Explorer browser.</returns>
		public static InternetExplorerBrowser Create()
		{
			return new InternetExplorerBrowser(CreateInternetExplorerClass());
		}

		/// <summary>
		/// Creates an instance of the InternetExplorerBrowser.
		/// </summary>
		/// <returns>An instance of Internet Explorer.</returns>
		private static InternetExplorer CreateInternetExplorerClass()
		{
			var watch = Stopwatch.StartNew();
			var timeout = TimeSpan.FromMilliseconds(1000);
			var lastException = new Exception("Failed to create an Internet Explorer instance.");

			do
			{
				try
				{
					return new InternetExplorerClass();
				}
				catch (COMException ex)
				{
					lastException = ex;
					Thread.Sleep(50);
				}
			} while (watch.Elapsed <= timeout);

			throw lastException;
		}

		#endregion
	}
}
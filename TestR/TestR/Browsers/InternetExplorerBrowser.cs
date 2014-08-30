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
			WaitForComplete();
			return _browser.LocationURL;
		}

		/// <summary>
		/// Navigates the browser to the provided URI.
		/// </summary>
		/// <param name="uri">The URI to navigate to.</param>
		protected override void BrowserNavigateTo(string uri)
		{
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
				// We cannot allow the browser to close within a second.
				// I assume that addons need time to start before closing the browser.
				var timeout = TimeSpan.FromMilliseconds(1000);
				while (Uptime <= timeout)
				{
					Thread.Sleep(50);
				}

				if (_browser != null)
				{
					_browser.DocumentComplete -= BrowserOnDocumentComplete;

					if (AutoClose)
					{
						_browser.Quit();
					}
				}
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

			Logger.Write(script, LogLevel.Trace);
			var wrappedScript = "try { document.executeResult = String(eval('" + script.Replace("'", "\\'") + "')); } catch (error) { document.executeResult = error; }";
			document.parentWindow.execScript(wrappedScript, "javascript");
			return GetJavascriptResult();
		}

		/// <summary>
		/// Injects the test script into the browser.
		/// </summary>
		protected override void InjectTestScript()
		{
			var document = _browser.Document as IHTMLDocument2;
			if (document == null)
			{
				throw new Exception("Failed to run script because no document is loaded.");
			}

			var window = document.parentWindow;
			window.execScript(GetTestScript(), "javascript");
		}

		/// <summary>
		/// Refreshed the state of the browser.
		/// </summary>
		protected override void Refresh()
		{
			WaitForComplete();
			InjectTestScript();
			DetectJavascriptLibraries();
			GetElementsFromScript();
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
				var value = property.ToString();
				return string.IsNullOrWhiteSpace(value) ? null : value;
			}
			catch
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Waits until the browser to complete any outstanding operations.
		/// </summary>
		private void WaitForComplete()
		{
			if (_browser == null)
			{
				return;
			}

			var states = new[] { tagREADYSTATE.READYSTATE_COMPLETE, tagREADYSTATE.READYSTATE_UNINITIALIZED };
			Utility.Wait(() => !_browser.Busy && states.Contains(_browser.ReadyState), 2000);

			if (_browser.ReadyState == tagREADYSTATE.READYSTATE_UNINITIALIZED)
			{
				return;
			}

			var document = _browser.Document as IHTMLDocument2;
			if (document == null)
			{
				return;
			}

			Utility.Wait(() => document.readyState == "complete", 2000);
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Attempts to attach to an existing browser.
		/// </summary>
		/// <returns>An instance of an Internet Explorer browser.</returns>
		public static InternetExplorerBrowser Attach()
		{
			var window = Window.FindWindow(Name);
			return window != null ? new InternetExplorerBrowser(window.GetInternetExplorer()) : null;
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
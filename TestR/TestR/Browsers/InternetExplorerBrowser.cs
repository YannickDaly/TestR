﻿#region References

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
			_browser.Visible = true;
			_window = new Window(new IntPtr(_browser.HWND));

			Attached = true;

			if (_browser.LocationURL.Length <= 0)
			{
				NavigateTo("about:tabs");
			}
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
			get { return _browser.LocationURL; }
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
		/// Execute JavaScript code in the current document.
		/// </summary>
		/// <param name="script">The script to run.</param>
		/// <returns>The response when executing.</returns>
		public override string ExecuteJavascript(string script)
		{
			var document = _browser.Document as IHTMLDocument2;
			if (document == null)
			{
				throw new Exception("Failed to run script because no document is loaded.");
			}

			Logger.Write(script, LogLevel.Trace);
			document.parentWindow.execScript("TestR.execute('" + script.Replace("'", "\\'") + "')", "javascript");
			return GetJavascriptResult();
		}

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
		public override sealed void NavigateTo(string uri)
		{
			object nil = null;
			object absoluteUri = uri;
			_browser.Navigate2(ref absoluteUri, ref nil, ref nil, ref nil, ref nil);
			WaitForComplete();
			Refresh();
		}

		/// <summary>
		/// Refresh the browser to the current page.
		/// </summary>
		public override void Refresh()
		{
			InjectJavascript(GetTestScript());
			DetectJavascriptLibraries();
			GetElementsFromScript();
		}

		/// <summary>
		/// Waits until the browser to complete any outstanding operations.
		/// </summary>
		public override void WaitForComplete()
		{
			if (_browser == null)
			{
				return;
			}

			Utility.Wait(() => !_browser.Busy && _browser.ReadyState == tagREADYSTATE.READYSTATE_COMPLETE);

			var document = _browser.Document as IHTMLDocument2;
			if (document == null)
			{
				return;
			}

			Utility.Wait(() => document.readyState == "complete"); // || document.readyState == "interactive");
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

			// We cannot allow the browser to close within a second.
			// I assume that addons need time to start before closing the browser.
			var timeout = TimeSpan.FromMilliseconds(1000);
			while (Uptime <= timeout)
			{
				Thread.Sleep(50);
			}

			if (_browser != null && AutoClose)
			{
				_browser.Quit();
				_browser = null;
			}
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

		private void InjectJavascript(string script)
		{
			var document = _browser.Document as IHTMLDocument2;
			if (document == null)
			{
				throw new Exception("Failed to run script because no document is loaded.");
			}

			document.parentWindow.execScript(script, "javascript");
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Attempts to attach to an existing browser.
		/// </summary>
		/// <returns>An instance of an Internet Explorer browser.</returns>
		public static InternetExplorerBrowser Attach()
		{
			var window = Window.FindWindow("iexplore");
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
		/// Closes all instances of the Internet Explorer browser.
		/// </summary>
		public static void CloseAllBrowsers()
		{
			Window.CloseAll("iexplore");
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
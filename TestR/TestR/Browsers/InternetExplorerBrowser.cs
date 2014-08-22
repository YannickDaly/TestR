#region References

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Expando;
using System.Threading;
using mshtml;
using SHDocVw;
using TestR.Collections;
using TestR.Extensions;
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

			Attached = true;

			if (_browser.LocationURL.Length <= 0)
			{
				NavigateTo("about:tabs");
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the current active element.
		/// </summary>
		public override Element ActiveElement
		{
			get { return new Element(new InternetExplorerElement(((IHTMLDocument2) _browser.Document).activeElement, this)); }
		}

		/// <summary>
		/// Gets the list of element in the current document.
		/// </summary>
		public override ElementCollection Elements
		{
			get { return new InternetExplorerElementCollection(((IHTMLDocument2) _browser.Document).all, this).ToElementCollection(); }
		}

		/// <summary>
		/// Gets the ID of the browser.
		/// </summary>
		public override int Id
		{
			get { return _browser.HWND; }
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
			get { return new IntPtr(_browser.HWND); }
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

			var resultName = "TestR_Script_Result";
			var errorName = "TestR_Script_Error";

			var wrappedCommand = string.Format("document.{0} = ''; document.{1} = ''; try {{ document.{0} = String(eval('{2}')) }} catch (error) {{ document.{1} = error }}; console.log('Result: ' + document.{0}); console.log('Error: ' + document.{1});",
				resultName, errorName, script.Replace("'", "\\'"));

			var watch = Stopwatch.StartNew();
			var timeout = TimeSpan.FromMilliseconds(1000);
			var lastException = new Exception("Failed to execute the JavaScript.");

			do
			{
				try
				{
					document.parentWindow.execScript(wrappedCommand, "javascript");
					return GetJavascriptResult(errorName) ?? GetJavascriptResult(resultName);
				}
				catch
				{
					Thread.Sleep(50);
				}
			} while (watch.Elapsed <= timeout);

			throw lastException;
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
			DetectJavascriptLibraries();
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

		private void DetectJavascriptLibraries()
		{
			if (Uri.Length <= 0 || Uri.Equals("about:tabs"))
			{
				return;
			}

			var libraries = new List<JavaScriptLibrary>();
			var hasLibrary = ExecuteJavascript("typeof jQuery !== 'undefined'");
			if (hasLibrary == "true")
			{
				libraries.Add(JavaScriptLibrary.JQuery);
			}

			hasLibrary = ExecuteJavascript("typeof angular !== 'undefined'");
			if (hasLibrary == "true")
			{
				libraries.Add(JavaScriptLibrary.Angular);
			}

			JavascriptLibraries = libraries;
		}

		private string GetJavascriptResult(string name)
		{
			var expando = (IExpando) _browser.Document;
			var property = expando.GetProperty(name, BindingFlags.Default);
			var value = property.GetValue(expando, null).ToString();
			return string.IsNullOrWhiteSpace(value) ? null : value;
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Attempts to attach to an existing browser. If one is not found then create and return a new one.
		/// </summary>
		/// <returns>An instance of an Internet Explorer browser.</returns>
		public static InternetExplorerBrowser AttachOrCreate()
		{
			var instance = NativeMethods.FindInternetExplorerInstances().FirstOrDefault();
			return instance != null ? new InternetExplorerBrowser(instance) : new InternetExplorerBrowser();
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
		/// Close all open Internet Explorer browsers.
		/// </summary>
		public static void CloseAllOpenBrowsers()
		{
			NativeMethods.FindInternetExplorerInstances().ForEach(x => x.Quit());
		}

		/// <summary>
		/// Gets a list of IDs for all current Internet Explorer browsers.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<int> GetExistingBrowserIds()
		{
			return NativeMethods.FindInternetExplorerInstances().Select(x => x.HWND);
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
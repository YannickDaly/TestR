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

#endregion

namespace TestR.Browsers
{
	public class InternetExplorerBrowser : Browser
	{
		#region Fields

		private IWebBrowser2 _browser;

		#endregion

		#region Constructors

		public InternetExplorerBrowser()
			: this(CreateInternetExplorerClass())
		{
			Attached = false;
		}

		private InternetExplorerBrowser(IWebBrowser2 browser)
		{
			_browser = browser;
			_browser.Visible = true;
			Attached = true;
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

		public bool Attached { get; private set; }

		public override ElementCollection Elements
		{
			get { return new InternetExplorerElementCollection(((IHTMLDocument2) _browser.Document).all, this).ToElementCollection(); }
		}

		public int Id
		{
			get { return _browser.HWND; }
		}

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

		public override void NavigateTo(string uri)
		{
			object nil = null;
			object absoluteUri = uri;
			_browser.Navigate2(ref absoluteUri, ref nil, ref nil, ref nil, ref nil);
			WaitForComplete();
			DetectJavascriptLibraries();
		}

		public override void WaitForComplete()
		{
			if (_browser == null)
			{
				return;
			}

			var watch = Stopwatch.StartNew();
			var timeout = new TimeSpan(0, 0, 5);

			while (_browser.Busy || _browser.ReadyState != tagREADYSTATE.READYSTATE_COMPLETE)
			{
				if (watch.Elapsed > timeout)
				{
					break;
				}

				Thread.Sleep(10);
			}

			IHTMLDocument2 document;

			try
			{
				document = _browser.Document as IHTMLDocument2;
				if (document == null)
				{
					return;
				}
			}
			catch (COMException)
			{
				return;
			}

			while (document.readyState != "complete" && document.readyState != "interactive")
			{
				if (watch.Elapsed > timeout)
				{
					break;
				}

				Thread.Sleep(10);
			}
		}

		public override string ExecuteJavascript(string script)
		{
			var document = _browser.Document as IHTMLDocument2;
			if (document == null)
			{
				throw new Exception("Failed to run script because no document is loaded.");
			}

			var resultName = "TestR_Script_Result";
			var errorName = "TestR_Script_Error";

			var wrappedCommand = string.Format("document.{0} = ''; document.{1} = ''; try {{ document.{0} = String(eval('{2}')) }} catch (error) {{ document.{1} = error }};",
				resultName, errorName, script.Replace("'", "\\'"));

			document.parentWindow.execScript(wrappedCommand, "javascript");

			// See if an error occurred.
			var errorResult = GetExpandoValue(errorName);
			if (!string.IsNullOrEmpty(errorResult))
			{
				throw new Exception(errorResult);
			}

			// Return the result
			return GetExpandoValue(resultName);
		}

		public override void ClearCookies(string url)
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
			var libraries = new List<JavascriptLibrary>();
			var hasLibrary = ExecuteJavascript("typeof jQuery !== 'undefined'");
			if (hasLibrary == "true")
			{
				libraries.Add(JavascriptLibrary.JQuery);
			}

			hasLibrary = ExecuteJavascript("typeof angular !== 'undefined'");
			if (hasLibrary == "true")
			{
				libraries.Add(JavascriptLibrary.Angular);
			}

			JavascriptLibraries = libraries;
		}

		private string GetExpandoValue(string attributeName)
		{
			var expando = (IExpando) _browser.Document;
			var property = expando.GetProperty(attributeName, BindingFlags.Default);
			if (property == null)
			{
				return null;
			}

			try
			{
				return property.GetValue(expando, null).ToString();
			}
			catch (COMException)
			{
				return null;
			}
		}

		#endregion

		#region Static Methods

		public static InternetExplorerBrowser AttachOrCreate()
		{
			var instance = NativeMethods.FindInternetExplorerInstances().FirstOrDefault();
			return instance != null ? new InternetExplorerBrowser(instance) : new InternetExplorerBrowser();
		}

		public static void CloseAllOpenBrowsers()
		{
			NativeMethods.FindInternetExplorerInstances().ForEach(x => x.Quit());
		}

		public static IEnumerable<int> GetExistingBrowserIds()
		{
			return NativeMethods.FindInternetExplorerInstances().Select(x => x.HWND);
		}

		private static IWebBrowser2 CreateInternetExplorerClass()
		{
			var watch = Stopwatch.StartNew();
			var timeout = TimeSpan.FromMilliseconds(5000);
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
				}
			} while (watch.Elapsed <= timeout);

			throw lastException;
		}

		#endregion
	}
}
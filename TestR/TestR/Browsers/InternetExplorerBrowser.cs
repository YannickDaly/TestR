#region References

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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

			var document = _browser.Document as IHTMLDocument2;
			if (document == null)
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

		protected override void Dispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}

			// We cannot allow the browser to close within a second.
			// Addons need time to start before closing the browser.
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
#region References

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using mshtml;
using SHDocVw;

#endregion

namespace TestR
{
	internal static class NativeMethods
	{
		#region Fields

		private static Guid _sidSTopLevelBrowser = new Guid(0x4C96BE40, 0x915C, 0x11CF, 0x99, 0xD3, 0x00, 0xAA, 0x00, 0x4A, 0xE8, 0x37);
		private static Guid _sidSWebBrowserApp = new Guid(0x0002DF05, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);

		#endregion

		#region Static Methods

		internal static IEnumerable<InternetExplorer> FindInternetExplorerInstances()
		{
			var browsers = new List<InternetExplorer>();
			var topLevelWindows = GetWindows("IEFrame");

			foreach (var mainBrowserWindow in topLevelWindows)
			{
				var windows = GetChildWindows(mainBrowserWindow.Hwnd, "TabWindowClass");

				foreach (var window in windows)
				{
					var webBrowser2 = GetWebBrowser(window) as InternetExplorer;
					if (webBrowser2 == null)
					{
						continue;
					}

					browsers.Add(webBrowser2);
				}
			}

			return browsers;
		}

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern IntPtr SetFocus(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		private static bool CompareClassNames(IntPtr hWnd, string expectedClassName)
		{
			if (hWnd == IntPtr.Zero)
			{
				return false;
			}
			if (string.IsNullOrEmpty(expectedClassName))
			{
				return false;
			}

			var className = GetClassName(hWnd);
			return className.Equals(expectedClassName);
		}

		[DllImport("user32", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool EnumChildWindows(IntPtr hWndParent, EnumChildWindowProc lpEnumFunc, IntPtr lParam);

		[DllImport("user32", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool EnumWindows(EnumWindowProc lpEnumFunc, IntPtr lParam);

		private static IntPtr GetChildWindowHwnd(IntPtr parentHwnd, string className)
		{
			var hWnd = IntPtr.Zero;
			EnumChildWindows(parentHwnd, (childHwnd, lParam) =>
			{
				if (CompareClassNames(childHwnd, className))
				{
					hWnd = childHwnd;
					return false;
				}

				return true;
			}, IntPtr.Zero);

			return hWnd;
		}

		private static IEnumerable<Window> GetChildWindows(IntPtr hwnd, string childClass)
		{
			var childWindows = new List<Window>();

			EnumChildWindows(hwnd, (childHwnd, lParam) =>
			{
				var childWindow = new Window(childHwnd);
				if (CompareClassNames(childWindow.Hwnd, childClass))
				{
					childWindows.Add(childWindow);
				}

				return true;
			}, IntPtr.Zero);

			return childWindows;
		}

		private static string GetClassName(IntPtr hwnd)
		{
			const int maxCapacity = 255;

			var className = new StringBuilder(maxCapacity);
			var lRes = GetClassName(hwnd, className, maxCapacity);

			return lRes == 0 ? String.Empty : className.ToString();
		}

		[DllImport("user32", SetLastError = true)]
		private static extern int GetClassName(IntPtr handleToWindow, StringBuilder className, int maxClassNameLength);

		private static IHTMLDocument2 GetDocumentFromWindow(IntPtr handle)
		{
			var documentGuid = new Guid("626FC520-A41E-11CF-A731-00A0C9082637");

			if (!IsInternetExplorerServerWindow(handle))
			{
				// Get 1st child IE server window
				handle = GetChildWindowHwnd(handle, "Internet Explorer_Server");
			}

			if (!IsInternetExplorerServerWindow(handle))
			{
				return null;
			}

			var lMsg = RegisterWindowMessage("WM_HTML_GETOBJECT");

			var result = 0;
			SendMessageTimeout(handle, lMsg, 0, 0, 2, 1000, ref result);
			if (result == 0)
			{
				return null;
			}

			// Get the object from lRes
			IHTMLDocument2 document = null;
			var hr = ObjectFromLresult(result, ref documentGuid, 0, ref document);
			if (hr != 0)
			{
				throw new COMException("ObjectFromLresult has thrown an exception", hr);
			}
			return document;
		}

		private static IWebBrowser2 GetWebBrowser(Window window)
		{
			var document2 = GetDocumentFromWindow(window.Hwnd);
			if (document2 == null)
			{
				return null;
			}

			var parentWindow = document2.parentWindow;
			if (parentWindow == null)
			{
				return null;
			}

			return RetrieveIWebBrowser2FromIHtmlWindw2Instance(parentWindow);
		}

		private static IEnumerable<Window> GetWindows(string className)
		{
			var windows = new List<Window>();

			EnumWindows((hwnd, lParam) =>
			{
				var window = new Window(hwnd);
				if (!window.HasParentWindow && CompareClassNames(window.Hwnd, className))
				{
					windows.Add(window);
				}

				return true;
			}, IntPtr.Zero);

			return windows;
		}

		private static bool IsInternetExplorerServerWindow(IntPtr hWnd)
		{
			return CompareClassNames(hWnd, "Internet Explorer_Server");
		}

		[DllImport("oleacc", SetLastError = true)]
		private static extern Int32 ObjectFromLresult(Int32 lResult, ref Guid riid, Int32 wParam, ref IHTMLDocument2 ppvObject);

		[DllImport("user32", SetLastError = true)]
		private static extern Int32 RegisterWindowMessage(string lpString);

		private static IWebBrowser2 RetrieveIWebBrowser2FromIHtmlWindw2Instance(IHTMLWindow2 ihtmlWindow2)
		{
			var guidIServiceProvider = typeof (IServiceProvider).GUID;

			var serviceProvider = ihtmlWindow2 as IServiceProvider;
			if (serviceProvider == null)
			{
				return null;
			}

			object objIServiceProvider;
			serviceProvider.QueryService(ref _sidSTopLevelBrowser, ref guidIServiceProvider, out objIServiceProvider);

			serviceProvider = objIServiceProvider as IServiceProvider;
			if (serviceProvider == null)
			{
				return null;
			}

			object objIWebBrowser;
			var guidIWebBrowser = typeof (IWebBrowser2).GUID;
			serviceProvider.QueryService(ref _sidSWebBrowserApp, ref guidIWebBrowser, out objIWebBrowser);
			var webBrowser = objIWebBrowser as IWebBrowser2;

			return webBrowser;
		}

		[DllImport("user32", SetLastError = true)]
		private static extern Int32 SendMessageTimeout(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam, Int32 fuFlags, Int32 uTimeout, ref Int32 lpdwResult);

		#endregion

		#region Delegates

		private delegate bool EnumChildWindowProc(IntPtr hWnd, IntPtr lParam);

		private delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

		#endregion

		#region Classes

		private class Window
		{
			#region Constructors

			public Window(IntPtr hwnd)
			{
				Hwnd = hwnd;
			}

			#endregion

			#region Properties

			public bool HasParentWindow { get; set; }
			public IntPtr Hwnd { get; set; }

			#endregion
		}

		#endregion

		#region Interfaces

		[ComImport]
		[Guid("6d5140c1-7436-11ce-8034-00aa006009fa")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		public interface IServiceProvider
		{
			[return: MarshalAs(UnmanagedType.I4)]
			[PreserveSig]
			uint QueryService(ref Guid guidService, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppvObject);
		}

		#endregion
	}
}
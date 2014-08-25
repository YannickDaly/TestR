#region References

using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using mshtml;
using SHDocVw;
using TestR.Helpers;

#endregion

namespace TestR
{
	/// <summary>
	/// Represents a window resource.
	/// </summary>
	public sealed class Window : IDisposable
	{
		#region Fields

		private Process _process;

		#endregion

		#region Constructors

		/// <summary>
		/// Instantiates an instance of the Window class.
		/// </summary>
		/// <param name="handle">The handle of the window.</param>
		public Window(IntPtr handle)
		{
			_process = Process.GetProcessesByName("iexplore").First(x => x.MainWindowHandle == handle);
		}

		/// <summary>
		/// Instantiates an instance of the Window class.
		/// </summary>
		public Window(Process process)
		{
			_process = process;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the handle for this window.
		/// </summary>
		public IntPtr Handle
		{
			get { return _process.MainWindowHandle; }
		}

		/// <summary>
		/// Gets a value indicating if this window has a parent window.
		/// </summary>
		public bool HasParentWindow { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Closes the window.
		/// </summary>
		public void Close()
		{
			_process.CloseMainWindow();
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Gets the browser from the window.
		/// </summary>
		/// <returns>The InternetExplorer hosted in the window.</returns>
		public InternetExplorer GetInternetExplorer()
		{
			var document2 = Utility.Retry(() => GetDocumentFromWindow(Handle), r => r != null, 20, 100);
			if (document2 == null)
			{
				throw new Exception("Failed to get the document from the window. " + Handle);
			}

			var htmlWindow = document2.parentWindow;
			if (htmlWindow == null)
			{
				throw new Exception("Failed to get the HTML window from the document.");
			}

			return (InternetExplorer) NativeMethods.GetWebBrowserFromHtmlWindow(htmlWindow);
		}

		/// <summary>
		/// Gets a value indicating if the current window is the foreground window.
		/// </summary>
		public bool IsForegroundWindow()
		{
			var handle = NativeMethods.GetForegroundWindow();
			return handle == Handle;
		}

		/// <summary>
		/// Sets the focus on this window.
		/// </summary>
		public void SetFocus()
		{
			NativeMethods.SetFocus(Handle);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <param name="disposing">True if you are disposing and false if otherwise.</param>		
		private void Dispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}

			if (_process != null)
			{
				_process.Dispose();
				_process = null;
			}
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Finds all windows my name and closes them.
		/// </summary>
		/// <param name="name">The name of the window (process) to close.</param>
		public static void CloseAll(string name)
		{
			var processes = Process.GetProcessesByName(name)
				.Where(x => x.MainWindowHandle != IntPtr.Zero)
				.ToList();

			do
			{
				foreach (var process in processes)
				{
					process.CloseMainWindow();
					process.Close();
					process.Dispose();
				}

				processes = Process.GetProcessesByName(name)
					.Where(x => x.MainWindowHandle != IntPtr.Zero)
					.ToList();
			} while (processes.Count > 0);
		}

		/// <summary>
		/// Finds a window by the process name and argument.
		/// </summary>
		/// <param name="processName">The name of the process to find.</param>
		/// <param name="argument">Any argument that should have been used to start the process. Defaults to an empty string.</param>
		/// <returns>Returns the window found or null if a window could not be found.</returns>
		public static Window FindWindow(string processName, string argument = "")
		{
			var query = string.Format("SELECT Handle, CommandLine FROM Win32_Process WHERE Name='{0}'", processName + ".exe");
			using (var searcher = new ManagementObjectSearcher(query))
			{
				foreach (var result in searcher.Get())
				{
					var managementObject = (ManagementObject) result;
					var handle = int.Parse(managementObject["Handle"].ToString());
					var data = managementObject["CommandLine"];
					if (data == null || !data.ToString().Contains(argument))
					{
						continue;
					}

					var process = Process.GetProcessesByName(processName).FirstOrDefault(x => x.Id == handle);
					if (process == null)
					{
						continue;
					}

					if (process.MainWindowHandle == IntPtr.Zero || !NativeMethods.IsWindowVisible(process.MainWindowHandle))
					{
						continue;
					}

					return new Window(process);
				}
			}

			return null;
		}

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

		private static IntPtr GetChildWindowHwnd(IntPtr parentHwnd, string className)
		{
			var hWnd = IntPtr.Zero;
			NativeMethods.EnumChildWindows(parentHwnd, (childHwnd, lParam) =>
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

		private static string GetClassName(IntPtr hwnd)
		{
			const int maxCapacity = 255;

			var className = new StringBuilder(maxCapacity);
			var lRes = NativeMethods.GetClassName(hwnd, className, maxCapacity);

			return lRes == 0 ? String.Empty : className.ToString();
		}

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

			var result = 0;
			var lMsg = NativeMethods.RegisterWindowMessage("WM_HTML_GETOBJECT");
			NativeMethods.SendMessageTimeout(handle, lMsg, 0, 0, 2, 1000, ref result);
			if (result == 0)
			{
				return null;
			}

			// Get the object from lRes
			IHTMLDocument2 document = null;
			var hr = NativeMethods.ObjectFromLresult(result, ref documentGuid, 0, ref document);
			if (hr != 0)
			{
				throw new COMException("ObjectFromLresult has thrown an exception", hr);
			}

			return document;
		}

		private static bool IsInternetExplorerServerWindow(IntPtr hWnd)
		{
			return CompareClassNames(hWnd, "Internet Explorer_Server");
		}

		#endregion
	}
}
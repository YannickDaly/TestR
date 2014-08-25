﻿#region References

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NLog;
using TestR.Browsers;
using TestR.Collections;
using TestR.Helpers;

#endregion

namespace TestR
{
	/// <summary>
	/// This is the base class for browsers.
	/// </summary>
	/// <exclude>Test?</exclude>
	public abstract class Browser : IDisposable
	{
		#region Fields

		private readonly Stopwatch _watch;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the Browser class.
		/// </summary>
		protected Browser()
		{
			_watch = Stopwatch.StartNew();
			AutoClose = true;
			JavascriptLibraries = new JavaScriptLibrary[0];
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the current active element.
		/// </summary>
		public abstract Element ActiveElement { get; }

		/// <summary>
		/// Gets a flag determining if the browser was attached to an existing open browser.
		/// </summary>
		public bool Attached { get; protected set; }

		/// <summary>
		/// Gets or sets a flag to auto close the browser when disposed of.
		/// </summary>
		public bool AutoClose { get; set; }

		/// <summary>
		/// Gets a list of all elements on the current page.
		/// </summary>
		public abstract ElementCollection Elements { get; }

		/// <summary>
		/// Gets the ID of the browser.
		/// </summary>
		public abstract int Id { get; }

		/// <summary>
		/// Gets a list of JavaScript libraries that were detected on the page.
		/// </summary>
		public IEnumerable<JavaScriptLibrary> JavascriptLibraries { get; set; }

		/// <summary>
		/// Gets or sets a flag to tell the browser to act slower.
		/// </summary>
		public bool SlowMotion { get; set; }

		/// <summary>
		/// The amount of time since this browser was created or attached to.
		/// </summary>
		public TimeSpan Uptime
		{
			get { return _watch.Elapsed; }
		}

		/// <summary>
		/// Gets the URI of the current page.
		/// </summary>
		public abstract string Uri { get; }

		/// <summary>
		/// Gets the window handle of the current browser.
		/// </summary>
		/// <value>Window handle of the current browser.</value>
		protected abstract IntPtr WindowHandle { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Brings the referenced Internet Explorer to the front (makes it the top window)
		/// </summary>
		public void BringToFront()
		{
			var result = Utility.Retry(() => NativeMethods.SetForegroundWindow(WindowHandle));
			if (!result)
			{
				Logger.Write("Failed to set " + GetType().Name + " as the foreground window (" + WindowHandle + ").", LogLevel.Warn);
			}
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
		/// Execute JavaScript code in the current document.
		/// </summary>
		/// <param name="script"></param>
		/// <returns></returns>
		public abstract string ExecuteJavascript(string script);

		/// <summary>
		/// Check to see if the browser is current the foreground window. 
		/// </summary>
		/// <returns>Returns true if so and false if otherwise.</returns>
		public bool IsInFront()
		{
			var handle = NativeMethods.GetForegroundWindow();
			return handle == WindowHandle;
		}

		/// <summary>
		/// Navigates the browser to the provided URI.
		/// </summary>
		/// <param name="uri">The URI to navigate to.</param>
		public abstract void NavigateTo(string uri);

		/// <summary>
		/// Waits until the browser to complete any outstanding operations.
		/// </summary>
		public abstract void WaitForComplete();

		/// <summary>
		/// Runs script to detect specific libraries.
		/// </summary>
		protected void DetectJavascriptLibraries()
		{
			if (Uri.Length <= 0 || Uri.Equals("about:tabs"))
			{
				return;
			}

			var libraries = new List<JavaScriptLibrary>();
			var hasLibrary = ExecuteJavascript("typeof jQuery !== 'undefined'");
			if (hasLibrary.Equals("true", StringComparison.OrdinalIgnoreCase))
			{
				libraries.Add(JavaScriptLibrary.JQuery);
			}

			hasLibrary = ExecuteJavascript("typeof angular !== 'undefined'");
			if (hasLibrary.Equals("true", StringComparison.OrdinalIgnoreCase))
			{
				libraries.Add(JavaScriptLibrary.Angular);
			}

			JavascriptLibraries = libraries;
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <param name="disposing">True if disposing and false if otherwise.</param>
		protected virtual void Dispose(bool disposing)
		{
		}

		protected void LinkToTestScript()
		{
			var scriptFilePath = GetTestFileFullPath("TestR.js");
			var script = "var script = document.createElement('script'); script.src = '" + scriptFilePath + "'; document.getElementsByTagName('head')[0].appendChild(script)";
			ExecuteJavascript(script);
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Closes all browsers of the provided type.
		/// </summary>
		/// <param name="type">The type of the browser to close.</param>
		public static void CloseAllBrowsers(BrowserType type)
		{
			if (((int) type & (int) BrowserType.Chrome) == (int) BrowserType.Chrome)
			{
				ChromeBrowser.CloseAllBrowsers();
			}

			if (((int) type & (int) BrowserType.InternetExplorer) == (int) BrowserType.InternetExplorer)
			{
				InternetExplorerBrowser.CloseAllBrowsers();
			}
		}

		/// <summary>
		/// Create or attach a browser of the provided type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static Browser CreateOrAttach<T>()
		{
			var type = typeof (T);
			if (type == typeof (ChromeBrowser))
			{
				return ChromeBrowser.AttachOrCreate();
			}

			if (type == typeof (InternetExplorerBrowser))
			{
				return InternetExplorerBrowser.AttachOrCreate();
			}

			throw new Exception("Invalid type provided.");
		}

		/// <summary>
		/// Creates a new process.
		/// </summary>
		/// <param name="fileName">The filename of the browser.</param>
		/// <param name="arguments">The arguments for the browser.</param>
		/// <returns>The new process for the browser.</returns>
		/// <exception cref="Exception">Failed to start the process.</exception>
		protected static Process CreateInstance(string fileName, string arguments = "")
		{
			var info = new ProcessStartInfo(fileName);
			info.Arguments = arguments;
			info.WindowStyle = ProcessWindowStyle.Normal;
			info.UseShellExecute = true;

			var process = new Process();
			process.StartInfo = info;

			if (!process.Start())
			{
				throw new Exception("Failed to start the process. ExitCode: " + process.ExitCode);
			}

			Utility.Wait(process, p => p.Handle != IntPtr.Zero);

			return process;
		}

		private static string GetTestFileFullPath(string relativePath)
		{
			var name = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) ?? "";
			name += "\\" + relativePath;
			name = name.Replace("\\", "/");
			name = name.Replace("file:/", "file:///");
			return name;
		}

		#endregion
	}
}
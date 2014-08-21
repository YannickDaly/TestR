#region References

using System;
using System.Collections.Generic;
using System.Diagnostics;
using NLog;
using TestR.Collections;

#endregion

namespace TestR
{
	public abstract class Browser : IDisposable
	{
		#region Fields

		private readonly Logger _logger;
		private readonly Stopwatch _watch;

		#endregion

		#region Constructors

		protected Browser()
		{
			_logger = LogManager.GetLogger("TestR");
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
			var result = NativeMethods.SetForegroundWindow(WindowHandle);
			if (!result)
			{
				_logger.Error("Failed to set {0} as the foreground window.", GetType());
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
			return NativeMethods.GetForegroundWindow() == WindowHandle;
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
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <param name="disposing">True if disposing and false if otherwise.</param>
		protected virtual void Dispose(bool disposing)
		{
		}

		#endregion
	}
}
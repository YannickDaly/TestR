#region References

using System;
using System.Diagnostics;
using System.Linq;
using NLog;
using TestR.Collections;
using TestR.Elements;
using TestR.Extensions;

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
		}

		#endregion

		#region Properties

		/// <summary>
		/// The amount of time since this browser was created or atteched to.
		/// </summary>
		public TimeSpan Uptime
		{
			get { return _watch.Elapsed; }
		}

		/// <summary>
		/// Gets the current active element.
		/// </summary>
		public abstract Element ActiveElement { get; }

		/// <summary>
		/// Gets or sets a flag to auto close the browser when disposed of.
		/// </summary>
		public bool AutoClose { get; set; }

		/// <summary>
		/// Gets a list of all button elements.
		/// </summary>
		public ElementCollection<ButtonElement> Buttons
		{
			get { return Elements.OfType<ButtonElement>().ToElementCollection(); }
		}

		/// <summary>
		/// Gets a list of all elements on the current page.
		/// </summary>
		public abstract ElementCollection Elements { get; }

		public ElementCollection<LinkElement> Links
		{
			get { return Elements.OfType<LinkElement>().ToElementCollection(); }
		}

		/// <summary>
		/// Gets a list of all textbox elements.
		/// </summary>
		public ElementCollection<TextBoxElement> TextElements
		{
			get { return Elements.OfType<TextBoxElement>().ToElementCollection(); }
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
			if (IsInFront())
			{
				return;
			}

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

		public ElementCollection GetElementsByClass(string className)
		{
			return Elements.Where(x => x.GetAttributeValue("class").Contains(className)).ToElementCollection();
		}

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
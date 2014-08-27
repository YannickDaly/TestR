#region References

using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Host;
using TestR.Browsers;

#endregion

namespace TestR.PowerShell
{
	public abstract class BrowserTestCmdlet : TestCmdlet
	{
		#region Constructors

		protected BrowserTestCmdlet()
		{
			AutoClose = false;
			BrowserSize = new Size(0, 0);
			BrowserType = BrowserType.All;
			SlowMotion = false;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the flag to automatically close the browser on disposing of the browser.
		/// </summary>
		[Parameter]
		public bool AutoClose { get; set; }

		/// <summary>
		/// Gets or sets the default browser size. If the size width and height are 0 then browser will use its default size and position.
		/// If it is set the browsers will be move and sized side by side using the top left corner.
		/// </summary>
		[Parameter]
		public Size BrowserSize { get; set; }

		/// <summary>
		/// Gets or sets the browser type to run each test with. You can specify a single browser, combination, or all.
		/// </summary>
		[Parameter]
		public BrowserType BrowserType { get; set; }

		/// <summary>
		/// Gets or set a flag to tell the browser to slow down user input so test can be monitored.
		/// </summary>
		[Parameter]
		public bool SlowMotion { get; set; }

		#endregion

		#region Methods

		public IEnumerable<Browser> GetBrowsers()
		{
			var response = new List<Browser>();

			if (HasBrowserType(BrowserType.Chrome))
			{
				var chrome = ChromeBrowser.AttachOrCreate();
				chrome.AutoClose = AutoClose;
				chrome.SlowMotion = SlowMotion;
				response.Add(chrome);
			}

			if (HasBrowserType(BrowserType.InternetExplorer))
			{
				var internetExplorer = InternetExplorerBrowser.AttachOrCreate();
				internetExplorer.AutoClose = AutoClose;
				internetExplorer.SlowMotion = SlowMotion;
				response.Add(internetExplorer);
			}

			if (BrowserSize.Width != 0 && BrowserSize.Height != 0)
			{
				for (var i = 0; i < response.Count; i++)
				{
					response[i].MoveWindow((BrowserSize.Width * i), 0, BrowserSize.Width, BrowserSize.Height);
				}
			}

			return response;
		}

		private bool HasBrowserType(BrowserType type)
		{
			return ((int) BrowserType & (int) type) == (int) type;
		}

		#endregion
	}

	public abstract class BrowserTestCmdlet<T> : TestCmdlet<T>
		where T : Browser, new()
	{
		#region Constructors

		protected BrowserTestCmdlet()
		{
			AutoClose = false;
			SlowMotion = false;
		}

		#endregion

		#region Properties

		[Parameter(Mandatory = false)]
		public bool AutoClose { get; set; }

		[Parameter(Mandatory = false)]
		public bool SlowMotion { get; set; }

		#endregion

		#region Methods

		protected override T CreateItem()
		{
			var response = (T) Browser.AttachOrCreate<T>();
			response.AutoClose = AutoClose;
			response.SlowMotion = SlowMotion;
			WriteVerbose("Creating browser with AutoClose: " + response.AutoClose + " SlowMotion: " + response.SlowMotion);
			return response;
		}

		#endregion
	}
}
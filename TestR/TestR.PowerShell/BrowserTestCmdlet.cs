#region References

using System;
using System.Collections.Generic;
using System.Management.Automation;
using TestR.Browsers;

#endregion

namespace TestR.PowerShell
{
	public abstract class BrowserTestCmdlet : TestCmdlet
	{
		#region Constructors

		protected BrowserTestCmdlet()
		{
			BrowserType = BrowserType.All;
			SlowMotion = false;
			AutoClose = false;
		}

		#endregion

		#region Properties

		[Parameter(Mandatory = false)]
		public bool AutoClose { get; set; }

		[Parameter(Mandatory = false)]
		public BrowserType BrowserType { get; set; }

		[Parameter(Mandatory = false)]
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

			for (var i = 0; i < response.Count; i++)
			{
				response[i].MoveWindow((800 * i), 0, 800, 600);
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
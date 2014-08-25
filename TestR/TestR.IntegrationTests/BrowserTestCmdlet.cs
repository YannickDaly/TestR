#region References

using System;
using System.Collections.Generic;
using System.Management.Automation;
using TestR.Browsers;

#endregion

namespace TestR.IntegrationTests
{
	public abstract class BrowserTestCmdlet : Cmdlet
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
		public BrowserType BrowserType { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter SlowMotion { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter AutoClose { get; set; }

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

			return response;
		}

		public abstract void RunTest();

		protected override void ProcessRecord()
		{
			RunTest();
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
		#region Methods

		protected override T CreateItem()
		{
			var response = (T) Browser.CreateOrAttach<T>();
			response.SlowMotion = SlowMotion;
			return response;
		}

		#endregion
	}
}
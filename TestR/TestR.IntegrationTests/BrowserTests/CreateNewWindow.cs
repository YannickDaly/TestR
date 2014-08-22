#region References

using System.Linq;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestR.Browsers;
using TestR.Helpers;

#endregion

namespace TestR.IntegrationTests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "CreateNewWindow")]
	public class CreateNewWindow : TestCmdlet
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			InternetExplorerBrowser.CloseAllOpenBrowsers();
			Utility.Wait(() => !InternetExplorerBrowser.GetExistingBrowserIds().Any(), delay: 100);
			Assert.AreEqual(0, InternetExplorerBrowser.GetExistingBrowserIds().Count());

			using (var browser = GetBrowser())
			{
				browser.AutoClose = false;
				browser.BringToFront();
				Assert.AreEqual(false, browser.Attached);
			}
		}

		#endregion
	}
}
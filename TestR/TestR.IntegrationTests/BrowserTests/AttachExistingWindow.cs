#region References

using System.Linq;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestR.Browsers;

#endregion

namespace TestR.IntegrationTests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "AttachExistingWindow")]
	public class AttachExistingWindow : TestCmdlet
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			var existingIds = InternetExplorerBrowser.GetExistingBrowserIds().ToList();
			if (!existingIds.Any())
			{
				using (var browser = new InternetExplorerBrowser())
				{
					browser.AutoClose = false;
					browser.BringToFront();
				}
			}

			existingIds = InternetExplorerBrowser.GetExistingBrowserIds().ToList();
			Assert.IsTrue(existingIds.Any(), "Failed to find any browsers.");

			using (var browser = GetBrowser())
			{
				browser.AutoClose = false;
				browser.BringToFront();
				Assert.IsTrue(existingIds.Contains(browser.Id), "Failed to connect to an existing browser.");
				Assert.AreEqual(true, browser.Attached);
			}
		}

		#endregion
	}
}
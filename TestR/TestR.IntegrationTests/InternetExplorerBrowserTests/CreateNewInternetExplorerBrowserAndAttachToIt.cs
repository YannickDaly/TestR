#region References

using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestR.Browsers;

#endregion

namespace TestR.IntegrationTests.InternetExplorerBrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "CreateNewInternetExplorerBrowserAndAttachToIt")]
	public class CreateNewInternetExplorerBrowserAndAttachToIt : TestCmdlet
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			InternetExplorerBrowser.CloseAllBrowsers();

			using (var browser = InternetExplorerBrowser.AttachOrCreate())
			{
				using (var browser2 = InternetExplorerBrowser.AttachOrCreate())
				{
					Assert.AreEqual(browser.Id, browser2.Id);
				}
			}
		}

		#endregion
	}
}
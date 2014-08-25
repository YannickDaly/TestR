#region References

using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestR.Browsers;

#endregion

namespace TestR.IntegrationTests.ChromeBrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "CreateNewChromeBrowserAndBringToFront")]
	public class CreateNewChromeBrowserAndBringToFront : BrowserTestCmdlet<ChromeBrowser>
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			using (var browser = CreateItem())
			{
				browser.BringToFront();
				Assert.IsNotNull(browser, "Failed to create an instance of the Chrome browser.");
			}
		}

		#endregion
	}
}
#region References

using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestR.Browsers;
using TestR.Helpers;

#endregion

namespace TestR.IntegrationTests.ChromeBrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "CreateNewChromeBrowserAndNavigate")]
	public class CreateNewChromeBrowserAndNavigate : BrowserTestCmdlet<ChromeBrowser>
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			using (var browser = CreateItem())
			{
				var expected = "http://www.bing.com/";
				browser.NavigateTo("http://bing.com");
				Utility.Wait(browser, b => b.Uri == expected);
				Assert.AreEqual(browser.Uri, expected);
			}
		}

		#endregion
	}
}
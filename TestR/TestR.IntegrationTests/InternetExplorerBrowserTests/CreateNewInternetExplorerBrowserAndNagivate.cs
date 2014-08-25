#region References

using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestR.Browsers;

#endregion

namespace TestR.IntegrationTests.InternetExplorerBrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "CreateNewInternetExplorerBrowserAndNavigate")]
	public class CreateNewInternetExplorerBrowserAndNagivate : TestCmdlet
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			using (var browser = new InternetExplorerBrowser())
			{
				browser.BringToFront();
				browser.NavigateTo("http://bing.com");
				Assert.AreEqual(browser.Uri, "http://www.bing.com/");
			}
		}

		#endregion
	}
}
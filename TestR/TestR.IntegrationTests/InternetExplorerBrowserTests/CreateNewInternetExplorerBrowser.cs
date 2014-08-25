#region References

using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestR.Browsers;

#endregion

namespace TestR.IntegrationTests.InternetExplorerBrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "CreateNewInternetExplorerBrowser")]
	public class CreateNewInternetExplorerBrowser : BrowserTestCmdlet<InternetExplorerBrowser>
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			using (var browser = CreateItem())
			{
				Assert.IsNotNull(browser, "Failed to create an instance of the Internet Explorer browser.");
			}
		}

		#endregion
	}
}
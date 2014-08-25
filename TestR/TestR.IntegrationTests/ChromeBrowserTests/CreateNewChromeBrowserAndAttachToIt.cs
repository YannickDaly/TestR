#region References

using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestR.Browsers;

#endregion

namespace TestR.IntegrationTests.ChromeBrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "CreateNewChromeBrowserAndAttachToIt")]
	public class CreateNewChromeBrowserAndAttachToIt : BrowserTestCmdlet<ChromeBrowser>
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			using (var browser = CreateItem())
			{
				using (var browser2 = CreateItem())
				{
					Assert.AreEqual(browser.Id, browser2.Id);
				}
			}
		}

		#endregion
	}
}
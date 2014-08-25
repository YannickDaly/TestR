#region References

using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.IntegrationTests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "NavigateTo")]
	public class NavigateTo : BrowserTestCmdlet
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					var expected = TestHelper.GetTestFileFullPath("Index.html");
					browser.AutoClose = false;
					browser.BringToFront();
					browser.NavigateTo(expected);
					Assert.AreEqual(expected, browser.Uri);
				}
			}
		}

		#endregion
	}
}
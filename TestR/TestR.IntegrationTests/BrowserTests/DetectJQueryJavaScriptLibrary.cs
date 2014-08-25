#region References

using System.Linq;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.IntegrationTests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "DetectJQueryJavaScriptLibrary")]
	public class DetectJQueryJavaScriptLibrary : BrowserTestCmdlet
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					browser.BringToFront();
					browser.NavigateTo(TestHelper.GetTestFileFullPath("JQuery.html"));

					Assert.IsTrue(browser.JavascriptLibraries.Contains(JavaScriptLibrary.JQuery));
				}
			}
		}

		#endregion
	}
}
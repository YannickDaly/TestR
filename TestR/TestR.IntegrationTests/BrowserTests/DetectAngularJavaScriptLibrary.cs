#region References

using System.Linq;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.IntegrationTests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "DetectAngularJavaScriptLibrary")]
	public class DetectAngularJavaScriptLibrary : BrowserTestCmdlet
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
					browser.NavigateTo(TestHelper.GetTestFileFullPath("Angular.html"));

					Assert.IsTrue(browser.JavascriptLibraries.Contains(JavaScriptLibrary.Angular));
				}
			}
		}

		#endregion
	}
}
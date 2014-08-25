#region References

using System.Linq;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.IntegrationTests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "DetectNoJavaScriptLibrary")]
	public class DetectNoJavaScriptLibrary : BrowserTestCmdlet
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
					browser.NavigateTo(TestHelper.GetTestFileFullPath("Inputs.html"));

					Assert.AreEqual(0, browser.JavascriptLibraries.Count());
				}
			}
		}

		#endregion
	}
}
#region References

using System.Linq;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.IntegrationTests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "DetectNoJavaScriptLibrary")]
	public class DetectNoJavaScriptLibrary : TestCmdlet
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			using (var browser = GetBrowser())
			{
				browser.AutoClose = false;
				browser.BringToFront();
				browser.NavigateTo(TestHelper.GetTestFileFullPath("Inputs.html"));

				Assert.AreEqual(0, browser.JavascriptLibraries.Count());
			}
		}

		#endregion
	}
}
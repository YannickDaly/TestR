#region References

using System.Linq;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.PowerShell.Tests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "DetectJQueryJavaScriptLibrary")]
	public class DetectJQueryJavaScriptLibrary : TestCmdlet
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			using (var browser = GetBrowser())
			{
				browser.AutoClose = false;
				browser.BringToFront();
				browser.NavigateTo(TestHelper.GetTestFileFullPath("JQuery.html"));

				Assert.IsTrue(browser.JavascriptLibraries.Contains(JavaScriptLibrary.JQuery));
			}
		}

		#endregion
	}
}
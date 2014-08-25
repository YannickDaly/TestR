#region References

using System.Linq;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.IntegrationTests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "FindElementByClass")]
	public class FindElementByClass : BrowserTestCmdlet
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					browser.AutoClose = false;
					browser.NavigateTo(TestHelper.GetTestFileFullPath("index.html"));
					var elements = browser.Elements.Where(x => x.Class.Contains("red"));
					Assert.AreEqual(1, elements.Count());
				}
			}
		}

		#endregion
	}
}
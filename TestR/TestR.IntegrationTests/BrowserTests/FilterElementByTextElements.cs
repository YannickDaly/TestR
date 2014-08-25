#region References

using System.Linq;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.IntegrationTests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "FilterElementByTextElements")]
	public class FilterElementByTextElements : BrowserTestCmdlet
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
					browser.NavigateTo(TestHelper.GetTestFileFullPath("inputs.html"));
					var inputs = browser.Elements.TextInputs.ToList();
					Assert.AreEqual(5, inputs.Count);
				}
			}
		}

		#endregion
	}
}
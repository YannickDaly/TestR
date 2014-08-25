#region References

using System.Linq;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.IntegrationTests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "SetFocus")]
	public class SetFocus : BrowserTestCmdlet
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					browser.NavigateTo(TestHelper.GetTestFileFullPath("inputs.html"));
					var expected = browser.Elements.TextInputs.Last();
					Assert.AreNotEqual(expected.Id, browser.ActiveElement.Id);
					expected.Focus();
					Assert.AreEqual(expected.Id, browser.ActiveElement.Id);
				}
			}
		}

		#endregion
	}
}
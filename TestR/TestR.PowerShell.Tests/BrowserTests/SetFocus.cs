#region References

using System.Linq;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.PowerShell.Tests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "SetFocus")]
	public class SetFocus : TestCmdlet
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			using (var browser = GetBrowser())
			{
				browser.AutoClose = false;
				browser.NavigateTo(TestHelper.GetTestFileFullPath("inputs.html"));
				var expected = browser.Elements.TextInputs.Last();
				Assert.AreNotEqual(expected.Id, browser.ActiveElement.Id);
				expected.Focus();
				Assert.AreEqual(expected.Id, browser.ActiveElement.Id);
			}
		}

		#endregion
	}
}
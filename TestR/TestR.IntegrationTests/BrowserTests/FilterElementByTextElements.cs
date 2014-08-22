#region References

using System.Linq;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.IntegrationTests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "FilterElementByTextElements")]
	public class FilterElementByTextElements : TestCmdlet
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			using (var browser = GetBrowser())
			{
				browser.AutoClose = false;
				browser.NavigateTo(TestHelper.GetTestFileFullPath("inputs.html"));
				var inputs = browser.Elements.TextInputs.ToList();
				Assert.AreEqual(5, inputs.Count);
			}
		}

		#endregion
	}
}
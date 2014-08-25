#region References

using System.Linq;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.IntegrationTests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "TypeTextIntoTextInputs")]
	public class TypeTextIntoTextInputs : BrowserTestCmdlet
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
					browser.NavigateTo(TestHelper.GetTestFileFullPath("inputs.html"));

					var inputs = browser.Elements.TextInputs;
					foreach (var input in inputs)
					{
						input.TypeText(input.Id);
						Assert.AreEqual(input.Id, input.Value);
					}
				}
			}
		}

		#endregion
	}
}
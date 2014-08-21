#region References

using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.PowerShell.Tests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "TypeTextIntoTextInputs")]
	public class TypeTextIntoTextInputs : TestCmdlet
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			using (var browser = GetBrowser())
			{
				browser.AutoClose = false;
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

		#endregion
	}
}
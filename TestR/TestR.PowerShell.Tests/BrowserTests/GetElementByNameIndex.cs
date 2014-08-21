#region References

using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.PowerShell.Tests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "GetElementByNameIndex")]
	public class GetElementByNameIndex : TestCmdlet
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			using (var browser = GetBrowser())
			{
				browser.AutoClose = false;
				browser.NavigateTo(TestHelper.GetTestFileFullPath("inputs.html"));
				var input = browser.Elements["text"];
				Assert.AreEqual("text", input.Id);
			}
		}

		#endregion
	}
}
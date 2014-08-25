#region References

using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.IntegrationTests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "GetElementByNameIndex")]
	public class GetElementByNameIndex : BrowserTestCmdlet
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
					var input = browser.Elements["inputName"];
					Assert.IsNotNull(input, "Failed to find input by name of 'inputName'.");
					Assert.AreEqual("inputName", input.Name);
				}
			}
		}

		#endregion
	}
}
#region References

using System.Linq;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.IntegrationTests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "FindElementByText")]
	public class FindElementByText : TestCmdlet
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			using (var browser = GetBrowser())
			{
				browser.AutoClose = false;
				browser.NavigateTo(TestHelper.GetTestFileFullPath("index.html"));
				var elements = browser.Elements.Where(x => x.Text == "SPAN with ID of 1");
				Assert.AreEqual(1, elements.Count());
			}
		}

		#endregion
	}
}
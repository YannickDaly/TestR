#region References

using System.Linq;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.IntegrationTests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "HighlightElement")]
	public class HighlightElement : BrowserTestCmdlet
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

					foreach (var element in browser.Elements.Where(t => t.TagName == "input"))
					{
						var originalColor = element.GetStyleAttributeValue("background-color");
						element.Highlight(true);
						Assert.AreEqual("yellow", element.GetStyleAttributeValue("background-color"));
						element.Highlight(false);
						Assert.AreEqual(originalColor, element.GetStyleAttributeValue("background-color"));
					}
				}
			}
		}

		#endregion
	}
}
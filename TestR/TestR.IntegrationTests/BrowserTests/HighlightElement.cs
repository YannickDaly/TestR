#region References

using System.Linq;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.IntegrationTests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "HighlightElement")]
	public class HighlightElement : TestCmdlet
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

				foreach (var element in browser.Elements.Where(t => t.TagName == "input"))
				{
					var originalColor = element.GetStyleAttributeValue("backgroundColor");
					element.Highlight(true);
					Assert.AreEqual("yellow", element.GetStyleAttributeValue("backgroundColor"));
					element.Highlight(false);
					Assert.AreEqual(originalColor, element.GetStyleAttributeValue("backgroundColor"));
				}
			}
		}

		#endregion
	}
}
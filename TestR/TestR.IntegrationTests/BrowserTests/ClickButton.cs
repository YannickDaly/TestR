﻿#region References

using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.IntegrationTests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "ClickButton")]
	public class ClickButton : BrowserTestCmdlet
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					browser.NavigateTo(TestHelper.GetTestFileFullPath("index.html"));

					var button = browser.Elements["button"];
					Assert.IsNotNull(button, "Could not find the button.");
					button.Click();

					var textArea = browser.Elements.TextInputs["textarea"];
					Assert.AreEqual(button.Id, textArea.Value);
				}
			}
		}

		#endregion
	}
}
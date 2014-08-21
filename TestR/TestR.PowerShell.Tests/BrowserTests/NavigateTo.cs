#region References

using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.PowerShell.Tests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "NavigateTo")]
	public class NavigateTo : TestCmdlet
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			using (var browser = GetBrowser())
			{
				var expected = TestHelper.GetTestFileFullPath("Index.html");
				browser.AutoClose = false;
				browser.BringToFront();
				browser.NavigateTo(expected);
				Assert.AreEqual(expected, browser.Uri);
			}
		}

		#endregion
	}
}
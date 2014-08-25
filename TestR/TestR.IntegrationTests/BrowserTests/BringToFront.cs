#region References

using System.Management.Automation;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.IntegrationTests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "BringToFront")]
	public class BringToFront : BrowserTestCmdlet
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
					Assert.IsTrue(browser.IsInFront());
				}
			}
		}

		#endregion
	}
}
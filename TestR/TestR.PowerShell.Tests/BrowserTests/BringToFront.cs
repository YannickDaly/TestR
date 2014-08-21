#region References

using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.PowerShell.Tests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "BringToFront")]
	public class BringToFront : TestCmdlet
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			using (var browser = GetBrowser())
			{
				browser.AutoClose = false;
				browser.BringToFront();
				Assert.IsTrue(browser.IsInFront());
			}
		}

		#endregion
	}
}
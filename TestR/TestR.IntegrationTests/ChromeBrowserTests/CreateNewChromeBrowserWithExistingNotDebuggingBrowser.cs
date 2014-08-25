#region References

using System;
using System.Diagnostics;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestR.Browsers;
using TestR.Helpers;

#endregion

namespace TestR.IntegrationTests.ChromeBrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "CreateNewChromeBrowserWithExistingNotDebuggingBrowser")]
	public class CreateNewChromeBrowserWithExistingNotDebuggingBrowser : TestCmdlet
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			ChromeBrowser.CloseAllBrowsers();

			var process = Process.Start("chrome.exe");
			if (process == null)
			{
				Assert.Fail("Could not start Chrome.");
			}

			Utility.Wait(process, p => p.MainWindowHandle != IntPtr.Zero);

			try
			{
				Validate.ExpectedException<Exception>(() =>
				{
					using (var browser2 = new ChromeBrowser())
					{
						browser2.BringToFront();
						Assert.IsNotNull(browser2, "Failed to create an instance of the Chrome browser.");
					}
				}, "The first instance of Chrome was not started with the remote debugger enabled.");
			}
			finally
			{
				process.CloseMainWindow();
				process.Close();
				process.Dispose();
			}
		}

		#endregion
	}
}
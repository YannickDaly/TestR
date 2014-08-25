#region References

using System.Management.Automation;
using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.IntegrationTests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "AngularInputTrigger")]
	public class AngularInputTrigger : BrowserTestCmdlet
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					browser.NavigateTo(TestHelper.GetTestFileFullPath("Angular.html"));
					
					var email = browser.Elements.TextInputs["email"];
					Assert.IsNotNull(email, "Failed to find the email input.");

					email.TypeText("test");
					var expected = "ng-dirty ng-valid-required ng-invalid ng-invalid-email".Split(' ');
					var actual = email.GetAttributeValue("class").Split(' ');
					TestHelper.AllExists(expected, actual);
					
					email.TypeText("test@domain.com");
					expected = "ng-dirty ng-valid-required ng-valid ng-valid-email".Split(' ');
					actual = email.GetAttributeValue("class").Split(' ');
					TestHelper.AllExists(expected, actual);
				}
			}
		}

		#endregion
	}
}
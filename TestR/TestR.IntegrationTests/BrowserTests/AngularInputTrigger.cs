#region References

using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR.IntegrationTests.BrowserTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "AngularInputTrigger")]
	public class AngularInputTrigger : TestCmdlet
	{
		#region Methods

		[TestMethod]
		public override void RunTest()
		{
			using (var browser = GetBrowser())
			{
				browser.AutoClose = false;
				browser.BringToFront();
				browser.NavigateTo(TestHelper.GetTestFileFullPath("Angular.html"));

				var email = browser.Elements.TextInputs["email"];
				Assert.IsNotNull(email, "Failed to find the email input.");

				email.TypeText("test");
				var attribute = email.GetAttributeValue("class");
				Assert.AreEqual("ng-dirty ng-valid-required ng-invalid ng-invalid-email", attribute);

				email.TypeText("test@domain.com");
				attribute = email.GetAttributeValue("class");
				Assert.AreEqual("ng-dirty ng-valid-required ng-valid ng-valid-email", attribute);
			}
		}

		#endregion
	}
}
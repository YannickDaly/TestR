#region References

using System;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestR.PowerShell.Tests;
using TestR.PowerShell.Tests.BrowserTests;

#endregion

namespace TestR.IntegrationTests
{
	[TestClass]
	public class BrowserTests
	{
		#region Methods

		[TestMethod]
		public void AngularInputTrigger()
		{
			RunPowerShellTestCmdlet<AngularInputTrigger>();
		}

		[TestMethod]
		public void AttachExistingWindow()
		{
			RunPowerShellTestCmdlet<AttachExistingWindow>();
		}

		[TestMethod]
		public void BringToFront()
		{
			RunPowerShellTestCmdlet<BringToFront>();
		}

		[TestMethod]
		public void ClickButton()
		{
			RunPowerShellTestCmdlet<ClickButton>();
		}

		[TestMethod]
		public void ClickInputButton()
		{
			RunPowerShellTestCmdlet<ClickInputButton>();
		}

		[TestMethod]
		public void ClickLink()
		{
			RunPowerShellTestCmdlet<ClickLink>();
		}

		[TestMethod]
		public void CreateNewWindow()
		{
			RunPowerShellTestCmdlet<CreateNewWindow>();
		}

		[TestMethod]
		public void DetectAngularJavascriptLibrary()
		{
			RunPowerShellTestCmdlet<DetectAngularJavaScriptLibrary>();
		}

		[TestMethod]
		public void DetectJQueryJavascriptLibrary()
		{
			RunPowerShellTestCmdlet<DetectJQueryJavaScriptLibrary>();
		}

		[TestMethod]
		public void DetectNoJavascriptLibraries()
		{
			RunPowerShellTestCmdlet<DetectNoJavaScriptLibrary>();
		}

		[TestMethod]
		public void FilterElementsForTextElements()
		{
			RunPowerShellTestCmdlet<FilterElementByTextElements>();
		}

		[TestMethod]
		public void FindElementByClass()
		{
			RunPowerShellTestCmdlet<TestFindElementByClass>();
		}

		[TestMethod]
		public void FindElementByText()
		{
			RunPowerShellTestCmdlet<TestFindElementByText>();
		}

		[TestMethod]
		public void GetElementByNameIndex()
		{
			RunPowerShellTestCmdlet<GetElementByNameIndex>();
		}

		[TestMethod]
		public void Hightlight()
		{
			RunPowerShellTestCmdlet<TestHighlightElement>();
		}

		[TestMethod]
		public void NavigateTo()
		{
			RunPowerShellTestCmdlet<TestNavigateTo>();
		}

		[TestMethod]
		public void SetFocus()
		{
			RunPowerShellTestCmdlet<TestSetFocus>();
		}

		[TestMethod]
		public void TextTextIntoTextElements()
		{
			RunPowerShellTestCmdlet<TestTypeTextIntoTextInputs>();
		}

		private void RunPowerShellTestCmdlet<T>()
			where T : TestCmdlet, new()
		{
			var cmdlet = new T();
			cmdlet.SlowMotion = true;
			var response = cmdlet.Invoke();
			foreach (var item in response)
			{
				Console.WriteLine(item.ToString());
			}
		}

		#endregion
	}
}
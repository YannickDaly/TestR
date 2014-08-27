#region References

using System.Linq;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestR.PowerShell;

#endregion

namespace TestR.IntegrationTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "Browsers")]
	public class BrowserTests : BrowserTestCmdlet
	{
		#region Methods

		[TestMethod]
		public void AngularInputTrigger()
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
					var actual = email.GetAttributeValue("class", true).Split(' ');
					TestHelper.AreEqual("test", email.Value);
					TestHelper.AllExists(expected, actual);

					email.TypeText("test@domain.com");
					expected = "ng-dirty ng-valid-required ng-valid ng-valid-email".Split(' ');
					actual = email.GetAttributeValue("class", true).Split(' ');
					TestHelper.AreEqual("testtest@domain.com", email.Value);
					TestHelper.AllExists(expected, actual);
				}
			}
		}

		[TestMethod]
		public void BringToFront()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					browser.BringToFront();
					// Cannot test this method because Windows could choose to just flash the 
					// application on the taskbar. This means we just have to assume it works.
					//Assert.IsTrue(browser.IsInFront());
				}
			}
		}

		[TestMethod]
		public void ClickButton()
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
					Assert.IsNotNull(textArea, "Could not find the textarea.");
					Assert.AreEqual(button.Id, textArea.Value);
				}
			}
		}

		[TestMethod]
		public void ClickButtonByName()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					browser.NavigateTo(TestHelper.GetTestFileFullPath("index.html"));

					var button = browser.Elements["buttonByName"];
					Assert.IsNotNull(button, "Could not find the button.");
					button.Click();

					var textArea = browser.Elements.TextInputs["textarea"];
					Assert.AreEqual(button.Name, textArea.Value);
				}
			}
		}

		[TestMethod]
		public void ClickInputButton()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					browser.NavigateTo(TestHelper.GetTestFileFullPath("index.html"));

					var button = browser.Elements.Buttons["inputButton"];
					Assert.IsNotNull(button, "Could not find the input button.");
					button.Click();

					var textArea = browser.Elements.TextInputs["textarea"];
					Assert.AreEqual(button.Id, textArea.Value);
				}
			}
		}

		[TestMethod]
		public void ClickLink()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					browser.NavigateTo(TestHelper.GetTestFileFullPath("index.html"));

					var button = browser.Elements["link"];
					Assert.IsNotNull(button, "Could not find the button.");
					button.Click();

					var textArea = browser.Elements.TextInputs["textarea"];
					Assert.AreEqual(button.Id, textArea.Value);
				}
			}
		}

		[TestMethod]
		public void DetectAngularJavaScriptLibrary()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					browser.NavigateTo(TestHelper.GetTestFileFullPath("Angular.html"));
					Assert.IsTrue(browser.JavascriptLibraries.Contains(JavaScriptLibrary.Angular));
				}
			}
		}

		[TestMethod]
		public void DetectJQueryJavaScriptLibrary()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					browser.NavigateTo(TestHelper.GetTestFileFullPath("JQuery.html"));
					Assert.IsTrue(browser.JavascriptLibraries.Contains(JavaScriptLibrary.JQuery));
				}
			}
		}

		[TestMethod]
		public void DetectNoJavaScriptLibrary()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					browser.NavigateTo(TestHelper.GetTestFileFullPath("Inputs.html"));
					Assert.AreEqual(0, browser.JavascriptLibraries.Count());
				}
			}
		}

		[TestMethod]
		public void FilterElementByTextElements()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					browser.NavigateTo(TestHelper.GetTestFileFullPath("inputs.html"));
					var inputs = browser.Elements.TextInputs.ToList();
					Assert.AreEqual(5, inputs.Count);
				}
			}
		}

		[TestMethod]
		public void FindElementByClass()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					browser.NavigateTo(TestHelper.GetTestFileFullPath("index.html"));
					var elements = browser.Elements.Where(x => x.Class.Contains("red"));
					Assert.AreEqual(1, elements.Count());
				}
			}
		}

		[TestMethod]
		public void FindElementByText()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					browser.NavigateTo(TestHelper.GetTestFileFullPath("index.html"));
					var elements = browser.Elements.Where(x => x.Text == "SPAN with ID of 1");
					Assert.AreEqual(1, elements.Count());
				}
			}
		}

		[TestMethod]
		public void Focus()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					browser.NavigateTo(TestHelper.GetTestFileFullPath("inputs.html"));

					var expected = browser.Elements.TextInputs.Last();
					Assert.AreNotEqual(expected.Id, browser.ActiveElement.Id);

					expected.Focus();
					Assert.AreEqual(expected.Id, browser.ActiveElement.Id);
				}
			}
		}

		[TestMethod]
		public void GetElementByNameIndex()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					browser.NavigateTo(TestHelper.GetTestFileFullPath("index.html"));
					var input = browser.Elements["inputName"];
					Assert.IsNotNull(input, "Failed to find input by name of 'inputName'.");
					Assert.AreEqual("inputName", input.Name);
				}
			}
		}

		[TestMethod]
		public void HighlightElement()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					browser.NavigateTo(TestHelper.GetTestFileFullPath("inputs.html"));

					var inputElements = browser.Elements.Where(t => t.TagName == "input").ToList();
					foreach (var element in inputElements)
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

		[TestMethod]
		public void NavigateTo()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					var expected = "http://www.bing.com/";
					browser.BringToFront();
					browser.NavigateTo(expected);
					Assert.AreEqual(expected, browser.Uri);
				}
			}
		}

		[TestMethod]
		public void Redirect()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					var expected = TestHelper.GetTestFileFullPath("Index.html");
					browser.NavigateTo(expected);
					TestHelper.AreEqual(expected, browser.Uri);

					var redirectLink = browser.Elements.Links["redirectLink"];
					redirectLink.Click();
					browser.Refresh();

					expected = TestHelper.GetTestFileFullPath("Inputs.html");
					TestHelper.AreEqual(expected, browser.Uri);
				}
			}
		}

		[TestMethod]
		public void TypeTextIntoTextInputs()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					browser.NavigateTo(TestHelper.GetTestFileFullPath("inputs.html"));
					var inputs = browser.Elements.TextInputs;

					foreach (var input in inputs)
					{
						input.TypeText(input.Id);
						Assert.AreEqual(input.Id, input.Value);
					}
				}
			}
		}

		#endregion
	}
}
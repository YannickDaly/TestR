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
					browser.Elements["button"].Click();

					var actual = browser.Elements.TextInputs["textarea"].Text;
					Assert.AreEqual("button", actual);
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
					browser.Elements["buttonByName"].Click();

					var textArea = browser.Elements.TextInputs["textarea"];
					Assert.AreEqual("buttonByName", textArea.Value);
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
					browser.Elements.Buttons["inputButton"].Click();

					var textArea = browser.Elements.TextInputs["textarea"];
					Assert.AreEqual("inputButton", textArea.Value);
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
					browser.Elements["link"].Click();

					var textArea = browser.Elements.TextInputs["textarea"];
					Assert.AreEqual("link", textArea.Value);
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
					var actual = browser.Elements["inputName"].Name;
					Assert.AreEqual("inputName", actual);
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
					Assert.AreEqual(expected, browser.Uri.ToLower());
				}
			}
		}

		[TestMethod]
		public void RedirectByLink()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					var expected = TestHelper.GetTestFileFullPath("Index.html").ToLower();
					browser.NavigateTo(expected);
					Assert.AreEqual(expected, browser.Uri.ToLower());

					// Redirect by the link.
					browser.Elements.Links["redirectLink"].Click();

					expected = TestHelper.GetTestFileFullPath("Inputs.html").ToLower();
					Assert.AreEqual(expected, browser.Uri.ToLower());
					browser.Elements["submit"].Click();
				}
			}
		}

		[TestMethod]
		public void RedirectByScript()
		{
			foreach (var browser in GetBrowsers())
			{
				using (browser)
				{
					var expected = TestHelper.GetTestFileFullPath("Index.html").ToLower();
					browser.NavigateTo(expected);
					Assert.AreEqual(expected, browser.Uri.ToLower());
					Assert.IsNotNull(browser.Elements["link"], "Failed to find the link element.");

					// Redirect by a script.
					browser.ExecuteScript("document.location.href = 'inputs.html'");

					expected = TestHelper.GetTestFileFullPath("inputs.html").ToLower();
					Assert.AreEqual(expected, browser.Uri.ToLower());
					browser.Elements["submit"].Click();
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
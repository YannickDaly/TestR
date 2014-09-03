#region References

using System.Linq;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestR.Elements;
using TestR.PowerShell;

#endregion

namespace TestR.IntegrationTests
{
	[TestClass]
	[Cmdlet(VerbsDiagnostic.Test, "Browsers")]
	public class BrowserTests : BrowserTestCmdlet
	{
		public BrowserTests()
		{
			BrowserType = BrowserType.InternetExplorer;
		}

		#region Methods

		[TestMethod]
		public void AngularInputTrigger()
		{
			ForEachBrowser(browser =>
			{
				browser.NavigateTo("http://localhost/Angular.html");

				var email = browser.Elements.TextInputs["email"];
				email.TypeText("test");

				var expected = "ng-dirty ng-valid-required ng-invalid ng-invalid-email".Split(' ');
				var actual = email.GetAttributeValue("class", true).Split(' ');
				TestHelper.AreEqual("test", email.Text);
				Validate.AllExists(expected, actual);

				email.TypeText("test@domain.com");
				expected = "ng-dirty ng-valid-required ng-valid ng-valid-email".Split(' ');
				actual = email.GetAttributeValue("class", true).Split(' ');
				TestHelper.AreEqual("testtest@domain.com", email.Text);
				Validate.AllExists(expected, actual);
			});
		}

		[TestMethod]
		public void BringToFront()
		{
			// Do not try and test the IsInFront method because Windows could choose to just flash the 
			// application on the taskbar. This means we just have to assume it works.
			// Ex: Assert.IsTrue(browser.IsInFront());
			ForEachBrowser(browser => browser.BringToFront());
		}

		[TestMethod]
		public void ClickButton()
		{
			ForEachBrowser(browser =>
			{
				browser.NavigateTo("http://localhost/index.html");
				browser.Elements["button"].Click();

				var actual = browser.Elements.TextArea["textarea"].Text;
				Assert.AreEqual("button", actual);
			});
		}

		[TestMethod]
		public void ClickButtonByName()
		{
			ForEachBrowser(browser =>
			{
				browser.NavigateTo("http://localhost/index.html");
				browser.Elements.First(x => x.Name == "buttonByName").Click();

				var textArea = browser.Elements.TextArea["textarea"];
				Assert.AreEqual("buttonByName", textArea.Text);
			});
		}

		[TestMethod]
		public void ClickInputButton()
		{
			ForEachBrowser(browser =>
			{
				browser.NavigateTo("http://localhost/index.html");
				browser.Elements.Buttons["inputButton"].Click();

				var textArea = browser.Elements.TextArea["textarea"];
				Assert.AreEqual("inputButton", textArea.Text);
			});
		}

		[TestMethod]
		public void ClickLink()
		{
			ForEachBrowser(browser =>
			{
				browser.NavigateTo("http://localhost/index.html");
				browser.Elements["link"].Click();

				var textArea = browser.Elements.TextArea["textarea"];
				Assert.AreEqual("link", textArea.Text);
			});
		}

		[TestMethod]
		public void DetectAngularJavaScriptLibrary()
		{
			ForEachBrowser(browser =>
			{
				browser.NavigateTo("http://localhost/Angular.html");
				Assert.IsTrue(browser.JavascriptLibraries.Contains(JavaScriptLibrary.Angular));
			});
		}

		[TestMethod]
		public void DetectJQueryJavaScriptLibrary()
		{
			ForEachBrowser(browser =>
			{
				browser.NavigateTo("http://localhost/JQuery.html");
				Assert.IsTrue(browser.JavascriptLibraries.Contains(JavaScriptLibrary.JQuery));
			});
		}

		[TestMethod]
		public void DetectNoJavaScriptLibrary()
		{
			ForEachBrowser(browser =>
			{
				browser.NavigateTo("http://localhost/Inputs.html");
				Assert.AreEqual(0, browser.JavascriptLibraries.Count());
			});
		}

		[TestMethod]
		public void ElementChildren()
		{
			ForEachBrowser(browser =>
			{
				browser.NavigateTo("http://localhost/relationships.html");
				var children = browser.Elements["parent1div"].Children;

				var expected = new[] { "child1div", "child2span", "child3br", "child4input" };
				Assert.AreEqual(4, children.Count);
				Validate.AllExists(expected, children.Select(x => x.Id).ToList());
			});
		}

		[TestMethod]
		public void ElementParent()
		{
			ForEachBrowser(browser =>
			{
				browser.NavigateTo("http://localhost/relationships.html");
				var element = browser.Elements["child1div"].Parent;
				Assert.AreEqual("parent1div", element.Id);
			});
		}

		[TestMethod]
		public void FilterElementByTextElements()
		{
			ForEachBrowser(browser =>
			{
				browser.NavigateTo("http://localhost/inputs.html");
				var inputs = browser.Elements.TextInputs.ToList();
				Assert.AreEqual(5, inputs.Count);
			});
		}

		[TestMethod]
		public void FindElementByClass()
		{
			ForEachBrowser(browser =>
			{
				browser.NavigateTo("http://localhost/index.html");
				var elements = browser.Elements.Where(x => x.Class.Contains("red"));
				Assert.AreEqual(1, elements.Count());
			});
		}

		[TestMethod]
		public void FindElementByClassByValueAccessor()
		{
			ForEachBrowser(browser =>
			{
				browser.NavigateTo("http://localhost/index.html");
				var elements = browser.Elements.Where(x => x["class"].Contains("red"));
				Assert.AreEqual(1, elements.Count());
			});
		}
		
		[TestMethod]
		public void FindSpanElementByText()
		{
			ForEachBrowser(browser =>
			{
				browser.NavigateTo("http://localhost/index.html");
				var elements = browser.Elements.Spans.Where(x => x.Text == "SPAN with ID of 1");
				Assert.AreEqual(1, elements.Count());
			});
		}

		[TestMethod]
		public void FindTextInputsByText()
		{
			ForEachBrowser(browser =>
			{
				browser.NavigateTo("http://localhost/index.html");
				var elements = browser.Elements.OfType<TextInput>().Where(x => x.Text == "Hello World");
				Assert.AreEqual(1, elements.Count());
			});
		}

		[TestMethod]
		public void Focus()
		{
			ForEachBrowser(browser =>
			{
				browser.NavigateTo("http://localhost/inputs.html");

				var expected = browser.Elements.TextInputs.Last();
				Assert.IsNull(browser.ActiveElement, "There should not be an active element.");

				expected.Focus();
				Assert.AreEqual(expected.Id, browser.ActiveElement.Id);
			});
		}

		[TestMethod]
		public void GetElementByNameIndex()
		{
			ForEachBrowser(browser =>
			{
				browser.NavigateTo("http://localhost/index.html");
				var actual = browser.Elements.First(x => x.Name == "inputName").Name;
				Assert.AreEqual("inputName", actual);
			});
		}

		[TestMethod]
		public void HighlightElement()
		{
			ForEachBrowser(browser =>
			{
				browser.NavigateTo("http://localhost/inputs.html");

				var inputElements = browser.Elements.Where(t => t.TagName == "input").ToList();
				foreach (var element in inputElements)
				{
					var originalColor = element.GetStyleAttributeValue("background-color");
					element.Highlight(true);
					Assert.AreEqual("yellow", element.GetStyleAttributeValue("background-color"));
					element.Highlight(false);
					Assert.AreEqual(originalColor, element.GetStyleAttributeValue("background-color"));
				}
			});
		}

		[TestMethod]
		public void NavigateTo()
		{
			ForEachBrowser(browser =>
			{
				var expected = "http://localhost/index.html";
				browser.NavigateTo(expected);
				Assert.AreEqual(expected, browser.Uri.ToLower());
			});
		}

		[TestMethod]
		public void RedirectByLink()
		{
			ForEachBrowser(browser =>
			{
				var expected = "http://localhost/index.html";
				browser.NavigateTo(expected);
				Assert.AreEqual(expected, browser.Uri.ToLower());

				// Redirect by the link.
				browser.Elements.Links["redirectLink"].Click();
				browser.WaitForRedirect();

				expected = "http://localhost/inputs.html";
				Assert.AreEqual(expected, browser.Uri.ToLower());
				browser.Elements["submit"].Click();
			});
		}

		[TestMethod]
		public void RedirectByScript()
		{
			ForEachBrowser(browser =>
			{
				var expected = "http://localhost/index.html";
				browser.NavigateTo(expected);
				Assert.AreEqual(expected, browser.Uri.ToLower());
				Assert.IsNotNull(browser.Elements["link"], "Failed to find the link element.");

				// Redirect by a script.
				browser.ExecuteScript("document.location.href = 'inputs.html'");
				browser.WaitForRedirect();

				expected = "http://localhost/inputs.html";
				Assert.AreEqual(expected, browser.Uri.ToLower());
				browser.Elements["submit"].Click();
			});
		}

		[TestMethod]
		public void TypeTextIntoTextInputs()
		{
			ForEachBrowser(browser =>
			{
				browser.NavigateTo("http://localhost/inputs.html");
				var inputs = browser.Elements.TextInputs;

				foreach (var input in inputs)
				{
					input.TypeText(input.Id);
					Assert.AreEqual(input.Id, input.Text);
				}
			});
		}

		#endregion
	}
}
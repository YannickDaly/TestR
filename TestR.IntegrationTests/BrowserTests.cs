#region References

using System;
using System.Configuration;
using System.Linq;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestR.Data;
using TestR.Elements;
using TestR.Logging;
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
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "AngularInputTrigger");
				browser.NavigateTo("http://localhost:8080/Angular.html");

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
		public void AngularNewElements()
		{
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "AngularNewElements");
				browser.NavigateTo("http://localhost:8080/Angular.html");
				var elementCount = browser.Elements.Count;

				var button = browser.Elements.Buttons["addItem"];
				button.Click();
				browser.Refresh();
				Assert.AreEqual(elementCount + 1, browser.Elements.Count);
				elementCount = browser.Elements.Count;

				button.Click();
				browser.Refresh();
				Assert.AreEqual(elementCount + 1, browser.Elements.Count);
			});
		}

		[TestMethod]
		public void AngularSwitchPageByNavigateTo()
		{
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "AngularInputTrigger");
				browser.NavigateTo("http://localhost:8080/Angular.html#/");
				Assert.AreEqual("http://localhost:8080/Angular.html#/", browser.Uri);

				Assert.IsTrue(browser.Elements.ContainsKey("addItem"));
				Assert.IsTrue(browser.Elements.ContainsKey("anotherPageLink"));

				browser.NavigateTo("http://localhost:8080/Angular.html#/anotherPage");
				Assert.AreEqual("http://localhost:8080/Angular.html#/anotherPage", browser.Uri);

				Assert.IsFalse(browser.Elements.ContainsKey("addItem"));
				Assert.IsTrue(browser.Elements.ContainsKey("pageLink"));

				browser.NavigateTo("http://localhost:8080/Angular.html#/");
				Assert.AreEqual("http://localhost:8080/Angular.html#/", browser.Uri);

				Assert.IsTrue(browser.Elements.ContainsKey("addItem"));
				Assert.IsTrue(browser.Elements.ContainsKey("anotherPageLink"));
			});
		}

		[TestMethod]
		public void BringToFront()
		{
			// Do not try and test the IsInFront method because Windows could choose to just flash the 
			// application on the taskbar. This means we just have to assume it works.
			// Ex: Assert.IsTrue(browser.IsInFront());
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "BringToFront");
				browser.BringToFront();
			});
		}

		[TestMethod]
		public void ClickButton()
		{
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "ClickButton");
				browser.NavigateTo("http://localhost:8080/index.html");
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
				LogManager.UpdateReferenceId(browser, "ClickButtonByName");
				browser.NavigateTo("http://localhost:8080/index.html");
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
				LogManager.UpdateReferenceId(browser, "ClickInputButton");
				browser.NavigateTo("http://localhost:8080/index.html");
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
				LogManager.UpdateReferenceId(browser, "ClickLink");
				browser.NavigateTo("http://localhost:8080/index.html");
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
				LogManager.UpdateReferenceId(browser, "DetectAngularJavaScriptLibrary");
				browser.NavigateTo("http://localhost:8080/Angular.html");
				Assert.IsTrue(browser.JavascriptLibraries.Contains(JavaScriptLibrary.Angular));
			});
		}

		[TestMethod]
		public void DetectJQueryJavaScriptLibrary()
		{
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "DetectJQueryJavaScriptLibrary");
				browser.NavigateTo("http://localhost:8080/JQuery.html");
				Assert.IsTrue(browser.JavascriptLibraries.Contains(JavaScriptLibrary.JQuery));
			});
		}

		[TestMethod]
		public void DetectNoJavaScriptLibrary()
		{
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "DetectNoJavaScriptLibrary");
				browser.NavigateTo("http://localhost:8080/Inputs.html");
				Assert.AreEqual(0, browser.JavascriptLibraries.Count());
			});
		}

		[TestMethod]
		public void ElementChildren()
		{
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "ElementChildren");
				browser.NavigateTo("http://localhost:8080/relationships.html");
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
				LogManager.UpdateReferenceId(browser, "ElementParent");
				browser.NavigateTo("http://localhost:8080/relationships.html");
				var element = browser.Elements["child1div"].Parent;
				Assert.AreEqual("parent1div", element.Id);
			});
		}

		[TestMethod]
		public void EnumerateDivisions()
		{
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "EnumerateDivisions");
				browser.NavigateTo("http://localhost:8080/index.html");
				var elements = browser.Elements.Divisions;
				Assert.AreEqual(1, elements.Count());

				foreach (var division in elements)
				{
					division.Text = "Division!";
				}
			});
		}

		[TestMethod]
		public void EnumerateHeaders()
		{
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "EnumerateHeaders");
				browser.NavigateTo("http://localhost:8080/index.html");
				var elements = browser.Elements.Headers;
				Assert.AreEqual(6, elements.Count());

				foreach (var header in elements)
				{
					header.Text = "Header!";
				}
			});
		}

		[TestMethod]
		public void FilterElementByTextElements()
		{
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "FilterElementByTextElements");
				browser.NavigateTo("http://localhost:8080/inputs.html");
				var inputs = browser.Elements.TextInputs.ToList();
				Assert.AreEqual(8, inputs.Count);
			});
		}

		[TestMethod]
		public void FindElementByClass()
		{
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "FindElementByClass");
				browser.NavigateTo("http://localhost:8080/index.html");
				var elements = browser.Elements.Where(x => x.Class.Contains("red"));
				Assert.AreEqual(1, elements.Count());
			});
		}

		[TestMethod]
		public void FindElementByClassByValueAccessor()
		{
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "FindElementByClassByValueAccessor");
				browser.NavigateTo("http://localhost:8080/index.html");
				var elements = browser.Elements.Where(x => x["class"].Contains("red"));
				Assert.AreEqual(1, elements.Count());
			});
		}

		[TestMethod]
		public void FindElementByClassProperty()
		{
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "FindElementByClassProperty");
				browser.NavigateTo("http://localhost:8080/index.html");
				var elements = browser.Elements.Links.Where(x => x.Class == "bold");
				Assert.AreEqual(1, elements.Count());
			});
		}

		[TestMethod]
		public void FindHeadersByText()
		{
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "FindHeadersByText");
				browser.NavigateTo("http://localhost:8080/index.html");
				var elements = browser.Elements.OfType<Header>().Where(x => x.Text.Contains("Header"));
				Assert.AreEqual(6, elements.Count());
			});
		}

		[TestMethod]
		public void FindSpanElementByText()
		{
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "FindSpanElementByText");
				browser.NavigateTo("http://localhost:8080/index.html");
				var elements = browser.Elements.Spans.Where(x => x.Text == "SPAN with ID of 1");
				Assert.AreEqual(1, elements.Count());
			});
		}

		[TestMethod]
		public void FindTextInputsByText()
		{
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "FindTextInputsByText");
				browser.NavigateTo("http://localhost:8080/index.html");
				var elements = browser.Elements.OfType<TextInput>().Where(x => x.Text == "Hello World");
				Assert.AreEqual(1, elements.Count());
			});
		}

		[TestMethod]
		public void Focus()
		{
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "Focus");
				browser.NavigateTo("http://localhost:8080/inputs.html");

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
				LogManager.UpdateReferenceId(browser, "GetElementByNameIndex");
				browser.NavigateTo("http://localhost:8080/index.html");
				var actual = browser.Elements.First(x => x.Name == "inputName").Name;
				Assert.AreEqual("inputName", actual);
			});
		}

		[TestMethod]
		public void HighlightElement()
		{
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "HighlightElement");
				browser.NavigateTo("http://localhost:8080/inputs.html");

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
				var expected = "http://localhost:8080/index.html";
				browser.NavigateTo(expected);
				Assert.AreEqual(expected, browser.Uri.ToLower());
			});
		}

		[TestMethod]
		public void RedirectByLink()
		{
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "RedirectByLink");
				var expected = "http://localhost:8080/index.html";
				browser.NavigateTo(expected);
				Assert.AreEqual(expected, browser.Uri.ToLower());

				// Redirect by the link.
				browser.Elements.Links["redirectLink"].Click();
				browser.WaitForRedirect();

				expected = "http://localhost:8080/inputs.html";
				Assert.AreEqual(expected, browser.Uri.ToLower());
				browser.Elements["submit"].Click();
			});
		}

		[TestMethod]
		public void RedirectByScript()
		{
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "RedirectByScript");
				var expected = "http://localhost:8080/index.html";
				browser.NavigateTo(expected);
				Assert.AreEqual(expected, browser.Uri.ToLower());
				Assert.IsNotNull(browser.Elements["link"], "Failed to find the link element.");

				// Redirect by a script.
				browser.ExecuteScript("document.location.href = 'inputs.html'");
				browser.WaitForRedirect();

				expected = "http://localhost:8080/inputs.html";
				Assert.AreEqual(expected, browser.Uri.ToLower());
				browser.Elements["submit"].Click();
			});
		}

		[TestMethod]
		public void TypeTextAllInputs()
		{
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "TypeTextAllInputs");
				browser.NavigateTo("http://localhost:8080/inputs.html");
				var inputs = browser.Elements.TextInputs;

				foreach (var input in inputs)
				{
					if (input.Id == "number")
					{
						input.TypeText("100");
						Assert.AreEqual("100", input.Text);
					}
					else
					{
						input.TypeText(input.Id);
						Assert.AreEqual(input.Id, input.Text);
					}
				}
			});
		}

		[TestMethod]
		public void TypeTextAppendInput()
		{
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "TypeTextAppendInput");
				browser.NavigateTo("http://localhost:8080/inputs.html");
				var input = browser.Elements.TextInputs["text"];
				input.Value = "foo";
				input.TypeText("bar");
				Assert.AreEqual("foobar", input.Value);
			});
		}

		[TestMethod]
		public void TypeTextSetInput()
		{
			ForEachBrowser(browser =>
			{
				LogManager.UpdateReferenceId(browser, "TypeTextSetInput");
				browser.NavigateTo("http://localhost:8080/inputs.html");
				var input = browser.Elements.TextInputs["text"];
				input.Value = "foo";
				input.TypeText("bar", true);
				Assert.AreEqual("bar", input.Value);
			});
		}

		#endregion

		#region Static Methods

		[ClassCleanup]
		public static void Cleanup()
		{
			LogManager.Loggers.Clear();
		}

		[ClassInitialize]
		public static void Initialize(TestContext context)
		{
			// Add the Entity Framework logger.
			//DataContext.InitializeMigrations();
			//var connectionStringSettings = ConfigurationManager.ConnectionStrings["DefaultConnection"];
			//if (connectionStringSettings != null)
			//{
			//	var dataContext = new DataContext(connectionStringSettings.ConnectionString);
			//	var logger1 = new DataContextLogger(dataContext);
			//	LogManager.Loggers.Add(logger1);
			//}

			// Add the console logger.
			var logger2 = new ConsoleLogger();
			logger2.Level = LogLevel.Verbose;
			LogManager.Loggers.Add(logger2);
		}

		#endregion
	}
}
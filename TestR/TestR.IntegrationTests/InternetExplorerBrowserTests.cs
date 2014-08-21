#region References

using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestR.Browsers;

#endregion

namespace TestR.IntegrationTests
{
	[TestClass]
	public class InternetExplorerBrowserTests
	{
		#region Methods

		[TestMethod]
		public void AngularInputTrigger()
		{
			using (var browser = InternetExplorerBrowser.AttachOrCreate())
			{
				browser.AutoClose = false;
				browser.BringToFront();
				browser.NavigateTo("http://localhost:61775/Angular.html");

				var email = browser.TextBoxElements["email"];
				Assert.IsNotNull(email, "Failed to find the email input.");

				email.TypeText("test");
				var attribute = email.GetAttributeValue("class");
				Assert.AreEqual("ng-dirty ng-valid-required ng-invalid ng-invalid-email", attribute);

				email.TypeText("test@domain.com");
				attribute = email.GetAttributeValue("class");
				Assert.AreEqual("ng-dirty ng-valid-required ng-valid ng-valid-email", attribute);
			}
		}

		[TestMethod]
		public void AttachOrCreateAttachExistingWindow()
		{
			InternetExplorerBrowser.CloseAllOpenBrowsers();
			var existingIds = InternetExplorerBrowser.GetExistingBrowserIds().ToList();
			Assert.AreEqual(0, existingIds.Count());

			using (var browser = new InternetExplorerBrowser())
			{
				browser.AutoClose = false;
				browser.NavigateTo("http://bing.com");
				Assert.AreEqual(false, browser.Attached);
			}

			existingIds = InternetExplorerBrowser.GetExistingBrowserIds().ToList();
			Assert.AreEqual(1, existingIds.Count);

			using (var browser = InternetExplorerBrowser.AttachOrCreate())
			{
				browser.AutoClose = true;
				Assert.AreEqual(browser.Id, existingIds[0]);
				Assert.AreEqual(true, browser.Attached);
			}
		}

		[TestMethod]
		public void AttachOrCreateCreateNewWindow()
		{
			InternetExplorerBrowser.CloseAllOpenBrowsers();
			Thread.Sleep(100);

			var existingIds = InternetExplorerBrowser.GetExistingBrowserIds();
			Assert.AreEqual(0, existingIds.Count());

			using (var browser = InternetExplorerBrowser.AttachOrCreate())
			{
				Assert.AreEqual(false, browser.Attached);
			}
		}

		[TestMethod]
		public void BringToFront()
		{
			using (Browser browser = new InternetExplorerBrowser())
			{
				browser.BringToFront();
				Assert.IsTrue(browser.IsInFront());
			}
		}

		[TestMethod]
		public void ClearCookies()
		{
			InternetExplorerBrowser.ClearCookies();
		}

		[TestMethod]
		public void ClickButton()
		{
			using (Browser browser = new InternetExplorerBrowser())
			{
				browser.NavigateTo("http://localhost:61775/index.html");

				var button = browser.Buttons["button"];
				Assert.IsNotNull(button, "Could not find the button.");
				button.Click();

				var textArea = browser.TextBoxElements["textarea"];
				Assert.AreEqual(button.Id, textArea.Value);
			}
		}

		[TestMethod]
		public void ClickInputButton()
		{
			using (Browser browser = new InternetExplorerBrowser())
			{
				browser.NavigateTo("http://localhost:61775/index.html");

				var button = browser.Buttons["inputButton"];
				Assert.IsNotNull(button, "Could not find the input button.");
				button.Click();

				var textArea = browser.TextBoxElements["textarea"];
				Assert.AreEqual(button.Id, textArea.Value);
			}
		}

		[TestMethod]
		public void ClickLink()
		{
			using (Browser browser = new InternetExplorerBrowser())
			{
				browser.NavigateTo("http://localhost:61775/index.html");

				var button = browser.Links["link"];
				Assert.IsNotNull(button, "Could not find the button.");
				button.Click();

				var textArea = browser.TextBoxElements["textarea"];
				Assert.AreEqual(button.Id, textArea.Value);
			}
		}

		[TestMethod]
		public void DetectAngularJavascriptLibrary()
		{
			using (var browser = InternetExplorerBrowser.AttachOrCreate())
			{
				browser.BringToFront();
				browser.NavigateTo("http://localhost:61775/Angular.html");

				Assert.IsTrue(browser.JavascriptLibraries.Contains(JavascriptLibrary.Angular));
			}
		}

		[TestMethod]
		public void DetectJQueryJavascriptLibrary()
		{
			using (var browser = InternetExplorerBrowser.AttachOrCreate())
			{
				browser.BringToFront();
				browser.NavigateTo("http://localhost:61775/JQuery.html");

				Assert.IsTrue(browser.JavascriptLibraries.Contains(JavascriptLibrary.JQuery));
			}
		}

		[TestMethod]
		public void DetectNoJavascriptLibraries()
		{
			using (var browser = InternetExplorerBrowser.AttachOrCreate())
			{
				browser.BringToFront();
				browser.NavigateTo("http://localhost:61775/Inputs.html");

				Assert.AreEqual(0, browser.JavascriptLibraries.Count());
			}
		}

		[TestMethod]
		public void FilterElementsByIdIndex()
		{
			using (Browser browser = new InternetExplorerBrowser())
			{
				browser.NavigateTo("http://localhost:61775/inputs.html");
				var input = browser.Elements["text"];
				Assert.AreEqual("text", input.Id);
			}
		}

		[TestMethod]
		public void FilterElementsByTagName()
		{
			using (Browser browser = new InternetExplorerBrowser())
			{
				browser.NavigateTo("http://localhost:61775/inputs.html");
				var inputs = browser.Elements.Where(x => x.TagName == "input").ToList();
				Assert.AreEqual(23, inputs.Count);
			}
		}

		[TestMethod]
		public void FilterElementsForTextElementsByInputType()
		{
			using (Browser browser = new InternetExplorerBrowser())
			{
				browser.NavigateTo("http://localhost:61775/inputs.html");
				var inputs = browser.TextBoxElements.ToList();
				Assert.AreEqual(5, inputs.Count);
			}
		}

		[TestMethod]
		public void FilterTextBoxElementsByIdIndex()
		{
			using (Browser browser = new InternetExplorerBrowser())
			{
				browser.NavigateTo("http://localhost:61775/inputs.html");
				var input = browser.TextBoxElements["text"];
				Assert.AreEqual("text", input.Id);
			}
		}

		[TestMethod]
		public void FindElementByClass()
		{
			using (Browser browser = new InternetExplorerBrowser())
			{
				browser.NavigateTo("http://localhost:61775/index.html");
				var elements = browser.GetElementsByClass("red");
				Assert.AreEqual(1, elements.Count);
			}
		}

		[TestMethod]
		public void FindElementByText()
		{
			using (Browser browser = new InternetExplorerBrowser())
			{
				browser.NavigateTo("http://localhost:61775/index.html");
				var elements = browser.Spans.Where(x => x.Text() == "SPAN with ID of 1");
				Assert.AreEqual(1, elements.Count());
			}
		}

		[TestMethod]
		public void Hightlight()
		{
			using (Browser browser = new InternetExplorerBrowser())
			{
				browser.BringToFront();
				browser.NavigateTo("http://localhost:61775/inputs.html");
				var textBox = browser.TextBoxElements.Last();
				var originalColor = textBox.GetStyleAttributeValue("backgroundColor");
				Assert.AreNotEqual(textBox.Id, browser.ActiveElement.Id);
				textBox.Highlight(true);
				Assert.AreEqual("yellow", textBox.GetStyleAttributeValue("backgroundColor"));
				textBox.Highlight(false);
				Assert.AreEqual(originalColor, textBox.GetStyleAttributeValue("backgroundColor"));
			}
		}

		[TestMethod]
		public void NavigateTo()
		{
			using (Browser browser = new InternetExplorerBrowser())
			{
				var expected = "http://localhost:61775/";
				browser.BringToFront();
				browser.NavigateTo(expected);
				Assert.AreEqual(expected, browser.Uri);
			}
		}

		[TestMethod]
		public void SetFocus()
		{
			using (Browser browser = new InternetExplorerBrowser())
			{
				browser.NavigateTo("http://localhost:61775/inputs.html");
				var expected = browser.TextBoxElements.Last();
				Assert.AreNotEqual(expected.Id, browser.ActiveElement.Id);
				expected.Focus();
				Assert.AreEqual(expected.Id, browser.ActiveElement.Id);
			}
		}

		[TestMethod]
		public void TextTextIntoTextElements()
		{
			using (Browser browser = new InternetExplorerBrowser())
			{
				browser.BringToFront();
				browser.NavigateTo("http://localhost:61775/inputs.html");
				var inputs = browser.TextBoxElements;

				foreach (var input in inputs)
				{
					input.TypeText(input.Id);
					Assert.AreEqual(input.Id, input.Value);
				}
			}
		}

		private string GetTestFileFullPath(string relativePath)
		{
			var name = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) ?? "";
			name = name.Replace("file:\\", string.Empty);
			name = name.Replace(@"TestR.IntegrationTests\bin\Debug", string.Empty);
			return name + "TestR.TestSite\\" + relativePath;
		}

		#endregion
	}
}
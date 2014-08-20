#region References

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestR.Browsers;

#endregion

namespace TestR.IntegrationTests
{
	[TestClass]
	public class ChromeBrowserUnitTest
	{
		#region Methods

		[TestMethod]
		public void Create()
		{
			using (var chrome = new ChromeBrowser())
			{
				chrome.NavigateTo("http://bing.com");
			}
		}

		public void UnitOfWork()
		{
			var chromeConnector = new ChromeBrowserConnector("http://localhost:9222");
			chromeConnector.Connect();

			var test = chromeConnector.GetDocument();
			Console.WriteLine(test);
		}

		public void UnitOfWork2()
		{
			var chromeConnector = new ChromeBrowserConnector("http://localhost:9222");
			chromeConnector.Connect();

			var test = chromeConnector.GetChildren(45);
			Console.WriteLine(test);
		}

		#endregion
	}
}
TestR (beta)
=====

Integration testing framework for developers. TestR allows automating testing of web applications. Currently we are supporting Internet Explorer but have started plans for Chrome and Firefox support. Progress on other browser than IE will depend on the time allowed. Currently IE gives a really nice interface to interacting with the browser so it is supported first.

### Searching Bing

```
using (var browser = new InternetExplorer())
{
	browser.NavigateTo("http://bing.com");
	
	var input = browser.Elements["q"];
	Assert.IsNotNull(button, "Could not find the search input.");
	input.TypeText("Bobby Cannon");

	var button = browser.Elements["go"];
	Assert.IsNotNull(button, "Could not find the search button.");
	button.Click();

	Assert.AreEqual(...);
}
```

### Find Element By Class

```
using (var browser = new InternetExplorer())
{
	browser.NavigateTo("http://bing.com");
	var elements = browser.Elements.Where(x => x.Class.Contains("b_searchbox"));
	Assert.AreEqual(1, elements.Count());
}
```

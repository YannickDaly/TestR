TestR (beta)
=====

Integration testing framework for developers. TestR allows automating testing of web applications. Currently we are supporting Internet Explorer and Chrome. Progress on other browsers will depend on the time allowed.

##### To install TestR, run one of the following command in the  Package Manager Console.

+ Install-Package TestR (without PowerShell)
+ Install-Package TestR.PowerShell (with PowerShell)


### Searching Bing

```
using (var browser = new InternetExplorerBrowser())
{
	browser.AutoClose = false;
	browser.NavigateTo("http://bing.com");

	var input = browser.Elements["q"];
	Assert.IsNotNull(input, "Could not find the search input.");
	input.TypeText("Bobby Cannon");

	var button = browser.Elements["go"];
	Assert.IsNotNull(button, "Could not find the search button.");
	button.Click();
}
```

New: Added Chrome support!

Coming Soon:

* Element relatioship support.
* Auto-detection of DOM changes.

TestR (beta)
=====

Integration testing framework for developers. TestR allows automating testing of web applications. Currently we are supporting Internet Explorer but have started plans for Chrome and Firefox support. Progress on other browser than IE will depend on the time allowed. Currently IE gives a really nice interface to interacting with the browser so it is supported first.

##### To install TestR, run the following command in the  Package Manager Console 

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

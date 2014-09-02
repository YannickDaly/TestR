TestR (beta)
=====

Integration testing framework for developers. TestR allows automating testing of web applications. Currently we are supporting Internet Explorer, Chrome, and Firefox*. We have full automation support for Internet Explorer and Chrome. * Firefox must be manually started and the "listen 6000" ran to start Firefox remote debugging port.

##### To install TestR, run one of the following command in the  Package Manager Console.

+ Install-Package TestR (without PowerShell)
+ Install-Package TestR.PowerShell (with PowerShell)


### Searching Bing using Internet Explorer

```
using (var browser = InternetExplorerBrowser.AttachOrCreate())
{
	browser.NavigateTo("http://bing.com");
	browser.Elements.TextInputs["sb_form_q"].TypeText("Bobby Cannon");
	browser.Elements["sb_form_go"].Click();
	browser.WaitForRedirect();
}
```

### Searching Amazon using Chrome

```
using (var browser = ChromeBrowser.AttachOrCreate()) 
{
	browser.NavigateTo("http://amazon.com");
	browser.Elements.TextInputs["twotabsearchtextbox"].TypeText("protein powder");
	browser.Elements.First(x => x.GetAttributeValue("title") == "Go").Click();
	browser.WaitForRedirect();
}
```

#### Coming Soon

* More element attributes.
* More specific element implementation with their unique attributes.

#### Known Issues

* Firefox debug port must be started manually (listen 6000).
* TestCmdlet does not call [TestInitialize] methods.
* Internet Explorer will fail if you cross security boundaries like going from Internet to Intranet sites.

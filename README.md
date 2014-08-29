TestR (beta)
=====

Integration testing framework for developers. TestR allows automating testing of web applications. Currently we are supporting Internet Explorer and Chrome. Progress on other browsers will depend on the time allowed.

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
	browser.AutoClose = false;
	browser.BringToFront();
	browser.NavigateTo("http://amazon.com");
	
	browser.Elements.TextInputs["twotabsearchtextbox"].TypeText("protein powder");
	browser.Elements.First(x => x.GetAttributeValue("title") == "Go").Click();
	browser.WaitForRedirect();
}
```

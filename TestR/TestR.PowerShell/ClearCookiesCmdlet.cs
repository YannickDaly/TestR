#region References

using System.Management.Automation;
using TestR.Browsers;

#endregion

namespace TestR.PowerShell
{
	[Cmdlet(VerbsCommon.Clear, "Cookies")]
	public class ClearCookiesCmdlet : Cmdlet
	{
		#region Properties

		[Parameter(Mandatory = false)]
		public string Uri { get; set; }

		#endregion

		#region Methods

		protected override void ProcessRecord()
		{
			Browser.CloseBrowsers(BrowserType.InternetExplorer);
			InternetExplorerBrowser.ClearCookies(Uri ?? string.Empty);
		}

		#endregion
	}
}
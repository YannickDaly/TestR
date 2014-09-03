#region References

using System.Management.Automation;
using System.Threading;
using TestR.Browsers;

#endregion

namespace TestR.PowerShell
{
	[Cmdlet(VerbsCommon.New, "Browser")]
	public class NewBrowserCmdlet : Cmdlet
	{
		#region Properties

		/// <summary>
		/// Attempts to attach to an existing browser before creating a new one.
		/// </summary>
		[Parameter(Mandatory = false)]
		public SwitchParameter AttachExisting { get; set; }

		/// <summary>
		/// This will close all browser sessions then clear cookies before returning a new browser.
		/// </summary>
		/// <remarks>
		/// Please note this will close all existing browser sessions.
		/// </remarks>
		[Parameter(Mandatory = false)]
		public SwitchParameter ClearCookies { get; set; }

		/// <summary>
		/// The URI of the domain to clear the cookies for. If this is not provided or empty then all cookies will be cleared.
		/// </summary>
		[Parameter(Mandatory = false)]
		public string CookieUri { get; set; }

		#endregion

		#region Methods

		protected override void ProcessRecord()
		{
			if (ClearCookies)
			{
				Browser.CloseBrowsers(BrowserType.InternetExplorer);
				InternetExplorerBrowser.ClearCookies(CookieUri ?? string.Empty);
				Thread.Sleep(250);
				WriteObject(new InternetExplorerBrowser());
				return;
			}

			var browser = AttachExisting
				? InternetExplorerBrowser.AttachOrCreate()
				: new InternetExplorerBrowser();

			WriteObject(browser);
			base.ProcessRecord();
		}

		#endregion
	}
}
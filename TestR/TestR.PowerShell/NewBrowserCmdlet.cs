#region References

using System.Management.Automation;
using TestR.Browsers;

#endregion

namespace TestR.PowerShell
{
	[Cmdlet(VerbsCommon.New, "InternetExplorer")]
	public class NewBrowserCmdlet : Cmdlet
	{
		#region Properties

		[Parameter(Mandatory = false)]
		public SwitchParameter AttachExisting { get; set; }

		#endregion

		#region Methods

		protected override void ProcessRecord()
		{
			var browser = AttachExisting
				? InternetExplorerBrowser.AttachOrCreate()
				: new InternetExplorerBrowser();

			WriteObject(browser);
		}

		#endregion
	}
}
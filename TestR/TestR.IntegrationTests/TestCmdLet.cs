#region References

using System.Management.Automation;
using TestR.Browsers;

#endregion

namespace TestR.IntegrationTests
{
	public abstract class TestCmdlet : Cmdlet
	{
		#region Properties

		[Parameter(Mandatory = false)]
		public SwitchParameter SlowMotion { get; set; }

		#endregion

		#region Methods

		public Browser GetBrowser()
		{
			var browser = InternetExplorerBrowser.AttachOrCreate();
			browser.SlowMotion = SlowMotion;
			return browser;
		}

		public abstract void RunTest();

		protected override void ProcessRecord()
		{
			RunTest();
		}

		#endregion
	}
}
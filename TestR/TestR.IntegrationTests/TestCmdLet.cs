#region References

using System.Management.Automation;

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

		public abstract void RunTest();

		protected override void ProcessRecord()
		{
			RunTest();
		}

		#endregion
	}

	public abstract class TestCmdlet<T> : Cmdlet
	{
		#region Properties

		[Parameter(Mandatory = false)]
		public SwitchParameter SlowMotion { get; set; }

		#endregion

		#region Methods

		public abstract void RunTest();

		protected abstract T CreateItem();

		protected override void ProcessRecord()
		{
			RunTest();
		}

		#endregion
	}
}
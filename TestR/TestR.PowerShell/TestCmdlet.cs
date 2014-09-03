#region References

using System.Linq;
using System.Management.Automation;
using System.Reflection;

#endregion

namespace TestR.PowerShell
{
	public abstract class TestCmdlet : Cmdlet
	{
		#region Properties

		[Parameter(Mandatory = false)]
		public string Name { get; set; }

		#endregion

		#region Methods

		protected string[] GetTestNames()
		{
			var type = GetType();
			var methods = type.GetMethods();

			return methods
				.Where(x => x.CustomAttributes.Any(a => a.AttributeType.Name == "TestMethodAttribute"))
				.Select(x => x.Name)
				.ToArray();
		}

		protected override void ProcessRecord()
		{
			if (string.IsNullOrWhiteSpace(Name))
			{
				WriteObject(GetTestNames());
				return;
			}

			try
			{
				Initialize();
				GetType().GetMethod(Name).Invoke(this, null);
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}

		private void Initialize()
		{
			var type = GetType();
			var methods = type.GetMethods();

			var initMethods = methods
				.Where(x => x.CustomAttributes.Any(a => a.AttributeType.Name == "TestInitializeAttribute"))
				.Select(x => x.Name)
				.ToArray();

			foreach (var name in initMethods)
			{
				GetType().GetMethod(name).Invoke(this, null);
			}
		}

		#endregion
	}

	public abstract class TestCmdlet<T> : TestCmdlet
	{
		#region Methods

		protected abstract T CreateItem();

		#endregion
	}
}
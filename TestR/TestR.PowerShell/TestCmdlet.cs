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
				GetType().GetMethod(Name).Invoke(this, null);
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
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
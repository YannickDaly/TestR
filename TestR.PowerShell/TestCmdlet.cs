﻿#region References

using System.Linq;
using System.Management.Automation;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestR.Logging;

#endregion

namespace TestR.PowerShell
{
	public abstract class TestCmdlet : Cmdlet
	{
		#region Properties

		/// <summary>
		/// Gets or sets the name of the test to run.
		/// </summary>
		[Parameter]
		public string Name { get; set; }

		/// <summary>
		/// Gets or set a logger for logging.
		/// </summary>
		[Parameter]
		public ILogger Logger { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Get a list of test name that are marked by the TestMethod attribute.
		/// </summary>
		/// <returns>The list of test names in this cmdlet.</returns>
		protected string[] GetTestNames()
		{
			var type = GetType();
			var methods = type.GetMethods();

			return methods
				.Where(x => x.CustomAttributes.Any(a => a.AttributeType.Name == "TestMethodAttribute"))
				.Select(x => x.Name)
				.ToArray();
		}

		/// <summary>
		/// Processes a single request for this cmdlet. If the Name is not set the cmdlet returs a list of 
		/// test names. If the name is set the specific test will be processed.
		/// </summary>
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
			if (Logger != null && !LogManager.Loggers.Contains(Logger))
			{
				LogManager.Loggers.Add(Logger);
			}
			
			var type = GetType();
			var methods = type.GetMethods();

			var initMethods = methods
				.Where(x => x.CustomAttributes.Any(a => a.AttributeType.Name == "TestInitializeAttribute"))
				.Select(x => x.Name)
				.ToList();

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
#region References

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace TestR
{
	/// <summary>
	/// The validate class contains methods to help validate your test.
	/// </summary>
	public static class Validate
	{
		#region Static Methods

		/// <summary>
		/// Ensures the correct exception is thrown.
		/// </summary>
		/// <param name="action">The action that should throw the expected exception.</param>
		/// <param name="message">The message to be contained in the exception.</param>
		/// <typeparam name="T">The type of the exception to be thrown.</typeparam>
		public static void ExpectedException<T>(Action action, string message = "")
			where T : Exception
		{
			try
			{
				action();
				Assert.Fail("The expected exception was not thrown.");
			}
			catch (T ex)
			{
				if (!ex.Message.Contains(message))
				{
					Assert.Fail("The expected exception was thrown but did not contain the expected message.");
				}
			}
		}

		#endregion
	}
}
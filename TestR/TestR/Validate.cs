#region References

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		/// Checks to see if both list contain the same items but ignores order.
		/// </summary>
		/// <param name="expected">The list of expected items.</param>
		/// <param name="actual">The list of actual items.</param>
		/// <typeparam name="T">The type of the item in the lists.</typeparam>
		public static void AllExists<T>(IList<T> expected, IList<T> actual)
		{
			var builder = new StringBuilder();
			foreach (var item in expected.Except(actual))
			{
				builder.AppendLine("Missing [" + item + "] in actual collection.");
			}

			if (builder.Length > 0)
			{
				Assert.Fail(builder.ToString());
			}
		}

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
					var error = "The expected exception was thrown but did not contain the expected message.";
					Assert.Fail("{0}{1}Expected: {2}{1}Actual: {3}", error, Environment.NewLine, message, ex.Message);
				}
			}
		}

		#endregion
	}
}
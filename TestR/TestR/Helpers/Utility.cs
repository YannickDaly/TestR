#region References

using System;
using System.Diagnostics;
using System.Threading;

#endregion

namespace TestR.Helpers
{
	/// <summary>
	/// Represents the Utility class.
	/// </summary>
	public static class Utility
	{
		#region Static Methods

		/// <summary>
		/// Retry the action until we get a valid response or hit the retry limit.
		/// </summary>
		/// <param name="action">The action to perform.</param>
		/// <param name="validateResponse">The function to validate the action response.</param>
		/// <param name="retryCount">The number of times to retry the action.</param>
		/// <param name="delay">The delay (in milliseconds) between each action attempt.</param>
		/// <typeparam name="T">The type of the action response.</typeparam>
		/// <returns>The action response.</returns>
		public static T Retry<T>(Func<T> action, Func<T, bool> validateResponse, int retryCount = 5, int delay = 50)
		{
			var response = action();
			var count = 0;

			while (!validateResponse(response))
			{
				Thread.Sleep(delay);
				response = action();

				if (count++ >= retryCount)
				{
					return response;
				}
			}

			return response;
		}

		/// <summary>
		/// Retry the action until it completes or hit the retry limit..
		/// </summary>
		/// <param name="action">The action to perform.</param>
		/// <param name="retryCount">The number of times to retry the action.</param>
		/// <param name="delay">The delay (in milliseconds) between each action attempt.</param>
		/// <param name="message">The option message to throw if the retry limit is triggered.</param>
		/// <typeparam name="T">The type of the action response.</typeparam>
		/// <returns>The action response.</returns>
		public static T Retry<T>(Func<T> action, int retryCount = 5, int delay = 100, string message = null)
		{
			var lastError = new Exception("Could not complete the retries.");

			for (var i = 0; i < retryCount; i++)
			{
				try
				{
					return action();
				}
				catch (Exception ex)
				{
					lastError = ex;
					Thread.Sleep(delay);
				}
			}

			throw string.IsNullOrWhiteSpace(message) ? lastError : new Exception(message);
		}

		/// <summary>
		/// Retry the action until it completes or hit the retry limit..
		/// </summary>
		/// <param name="action">The action to perform.</param>
		/// <param name="retryCount">The number of times to retry the action.</param>
		/// <param name="delay">The delay (in milliseconds) between each action attempt.</param>
		/// <param name="message">The option message to throw if the retry limit is triggered.</param>
		/// <returns>The action response.</returns>
		public static void Retry(Action action, int retryCount = 5, int delay = 100, string message = null)
		{
			var lastError = new Exception("Could not complete the retries.");

			for (var i = 0; i < retryCount; i++)
			{
				try
				{
					action();
					return;
				}
				catch (Exception ex)
				{
					lastError = ex;
					Thread.Sleep(delay);
				}
			}

			throw string.IsNullOrWhiteSpace(message) ? lastError : new Exception(message);
		}

		/// <summary>
		/// Runs the action until the action returns true or the timeout is reached. Will delay in between actions of the provided time.
		/// </summary>
		/// <param name="action">The action to call.</param>
		/// <param name="timeout">The timeout to attempt the action. This value is in milliseconds.</param>
		/// <param name="delay">The delay in between actions. This value is in milliseconds.</param>
		/// <returns>Returns true of the call completed successfully or false if it timed out.</returns>
		public static bool Wait(Func<bool> action, int timeout = 1000, int delay = 25)
		{
			var watch = Stopwatch.StartNew();
			var watchTimeout = TimeSpan.FromMilliseconds(timeout);

			while (!action())
			{
				if (watch.Elapsed > watchTimeout)
				{
					return false;
				}

				Thread.Sleep(delay);
			}

			return true;
		}

		/// <summary>
		/// Runs the action until the action returns true or the timeout is reached. Will delay in between actions of the provided time.
		/// </summary>
		/// <param name="input">The input to pass to the action.</param>
		/// <param name="action">The action to call.</param>
		/// <param name="timeout">The timeout to attempt the action. This value is in milliseconds.</param>
		/// <param name="delay">The delay in between actions. This value is in milliseconds.</param>
		/// <returns>Returns true of the call completed successfully or false if it timed out.</returns>
		public static bool Wait<T>(T input, Func<T, bool> action, int timeout = 1000, int delay = 25)
		{
			var watch = Stopwatch.StartNew();
			var watchTimeout = TimeSpan.FromMilliseconds(timeout);

			while (!action(input))
			{
				if (watch.Elapsed > watchTimeout)
				{
					return false;
				}

				Thread.Sleep(delay);
			}

			return true;
		}

		#endregion
	}
}
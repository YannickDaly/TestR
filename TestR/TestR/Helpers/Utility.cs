#region References

using System;
using System.Diagnostics;
using System.Threading;

#endregion

namespace TestR.Helpers
{
	public static class Utility
	{
		#region Static Methods

		public static bool Wait(Func<bool> action, int timeout = 1000, int delay = 25)
		{
			var watch = Stopwatch.StartNew();
			var watchTimeout = TimeSpan.FromMilliseconds(timeout);

			while (!action())
			{
				if (watch.Elapsed > watchTimeout)
				{
					return true;
				}

				Thread.Sleep(delay);
			}

			return false;
		}

		public static bool Wait<T>(T input, Func<T, bool> action, int timeout = 1000, int delay = 25)
		{
			var watch = Stopwatch.StartNew();
			var watchTimeout = TimeSpan.FromMilliseconds(timeout);

			while (!action(input))
			{
				if (watch.Elapsed > watchTimeout)
				{
					return true;
				}

				Thread.Sleep(delay);
			}

			return false;
		}

		#endregion
	}
}
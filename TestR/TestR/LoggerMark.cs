#region References

using System;
using System.Diagnostics;

#endregion

namespace TestR
{
	/// <summary>
	/// Represents a mark for benchmarking.
	/// </summary>
	public class LoggerMark : IDisposable
	{
		#region Fields

		private readonly uint _id;
		private readonly string _location;
		private readonly NLog.Logger _logger;
		private readonly Stopwatch _watch;
		private static uint _nextId;

		#endregion

		#region Constructors

		/// <summary>
		/// Instantiates an instance of the LoggerMark class.
		/// </summary>
		/// <param name="location">The location of the mark.</param>
		/// <param name="logger">The logger to log the mark to.</param>
		public LoggerMark(string location, NLog.Logger logger)
		{
			_id = _nextId++;
			_location = location;
			_logger = logger;
			_logger.Trace("Enter " + location + " : " + _id);
			_watch = Stopwatch.StartNew();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Logs the excited of the location to the log.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_logger.Trace("Exit " + _location + " : " + _id + " - Elapsed: " + _watch.Elapsed);
			}
		}

		#endregion
	}
}
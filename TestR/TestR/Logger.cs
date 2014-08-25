#region References

using NLog;
using NLog.Config;
using NLog.Targets;

#endregion

namespace TestR
{
	/// <summary>
	/// A global NLog logger to assist in logging.
	/// </summary>
	public static class Logger
	{
		#region Fields

		private static readonly NLog.Logger _logger;

		#endregion

		#region Constructors

		static Logger()
		{
			_logger = LogManager.GetLogger("TestR");
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Enable console tracing. This really should only be used for debugging / troubleshooting.
		/// </summary>
		public static void EnableConsoleTracing()
		{
			var consoleTarget = new ConsoleTarget();
			consoleTarget.Layout = "${longdate} ${message}";
			var loggingConfiguration = new LoggingConfiguration();
			loggingConfiguration.AddTarget("Console", consoleTarget);
			loggingConfiguration.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, consoleTarget));
			LogManager.Configuration = loggingConfiguration;
		}

		/// <summary>
		/// Writes a messages to the log.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="level">The level of the log.</param>
		public static void Write(string message, LogLevel level)
		{
			if (level == LogLevel.Debug)
			{
				_logger.Debug(message);
			}
			else if (level == LogLevel.Fatal)
			{
				_logger.Fatal(message);
			}
			else if (level == LogLevel.Trace)
			{
				_logger.Trace(message);
			}
			else if (level == LogLevel.Warn)
			{
				_logger.Warn(message);
			}
			else
			{
				_logger.Info(message);
			}
		}

		#endregion
	}
}
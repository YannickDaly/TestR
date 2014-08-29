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

		private static readonly NLog.Logger _benchmarkLogger;
		private static readonly ConsoleTarget _consoleTarget;
		private static readonly NLog.Logger _verboseLogger;
		private static bool _enableBenchmarking;
		private static bool _enableTracing;

		#endregion

		#region Constructors

		static Logger()
		{
			_verboseLogger = LogManager.GetLogger("TestR");
			_benchmarkLogger = LogManager.GetLogger("TestR.Benchmark");
			_consoleTarget = new ConsoleTarget();
			_consoleTarget.Layout = "${longdate} ${message}";
			_enableBenchmarking = false;
			_enableTracing = false;
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Enable console tracing. This really should only be used for debugging / troubleshooting.
		/// </summary>
		public static bool EnableBenchmarking
		{
			get { return _enableBenchmarking; }

			set
			{
				_enableBenchmarking = value;
				UpdateConfiguration();
			}
		}

		/// <summary>
		/// Enable console tracing. This really should only be used for debugging / troubleshooting.
		/// </summary>
		public static bool EnableConsoleTracing
		{
			get { return _enableTracing; }

			set
			{
				_enableTracing = value;
				UpdateConfiguration();
			}
		}

		/// <summary>
		/// Mark a location for benchmarking.
		/// </summary>
		/// <param name="location">The location to mark.</param>
		/// <returns>The mark of the location.</returns>
		public static LoggerMark Mark(string location)
		{
			return new LoggerMark(location, _benchmarkLogger);
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
				_verboseLogger.Debug(message);
			}
			else if (level == LogLevel.Fatal)
			{
				_verboseLogger.Fatal(message);
			}
			else if (level == LogLevel.Trace)
			{
				_verboseLogger.Trace(message);
			}
			else if (level == LogLevel.Warn)
			{
				_verboseLogger.Warn(message);
			}
			else
			{
				_verboseLogger.Info(message);
			}
		}

		private static void UpdateConfiguration()
		{
			var loggingConfiguration = new LoggingConfiguration();
			loggingConfiguration.AddTarget("Console", _consoleTarget);
			if (_enableBenchmarking)
			{
				loggingConfiguration.LoggingRules.Add(new LoggingRule("TestR.Benchmark", LogLevel.Trace, _consoleTarget));
			}

			if (_enableTracing)
			{
				loggingConfiguration.LoggingRules.Add(new LoggingRule("TestR", LogLevel.Trace, _consoleTarget));
			}
			LogManager.Configuration = loggingConfiguration;
		}

		#endregion
	}
}
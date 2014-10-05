#region References

using System;

#endregion

namespace TestR.Logging
{
	/// <summary>
	/// Represents a logger for a console.
	/// </summary>
	public class ConsoleLogger : ILogger
	{
		/// <summary>
		/// Creates an instance of the console logger.
		/// </summary>
		public ConsoleLogger()
		{
			Level = LogLevel.Information;
		}

		#region Properties

		/// <summary>
		/// The max level for this logger to process.
		/// </summary>
		public LogLevel Level { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Write the message to the logger.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="level">The log level of the message.</param>
		public void Write(string message, LogLevel level)
		{
			var originalColor = Console.ForegroundColor;

			try
			{
				switch (level)
				{
					case LogLevel.Verbose:
						Console.ForegroundColor = ConsoleColor.DarkGray;
						break;

					case LogLevel.Debug:
						Console.ForegroundColor = ConsoleColor.Gray;
						break;

					case LogLevel.Warning:
						Console.ForegroundColor = ConsoleColor.Yellow;
						break;
					
					case LogLevel.Error:
						Console.ForegroundColor = ConsoleColor.Red;
						break;
					
					case LogLevel.Fatal:
						Console.ForegroundColor = ConsoleColor.Red;
						break;

					default:
						Console.ForegroundColor = ConsoleColor.White;
						break;
				}

				Console.WriteLine(LogManager.ReferenceId + " : " + message);
			}
			finally
			{
				Console.ForegroundColor = originalColor;
			}
		}

		#endregion
	}
}
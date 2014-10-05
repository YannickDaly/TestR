#region References

using TestR.Data;
using TestR.Models;

#endregion

namespace TestR.Logging
{
	/// <summary>
	/// Represents a logger that targets a data context.
	/// </summary>
	public class DataContextLogger : ILogger
	{
		#region Fields

		private readonly IDataContext _dataContext;

		#endregion

		#region Constructors

		/// <summary>
		/// Creates an instant of the database logger.
		/// </summary>
		/// <param name="dataContext">The data context to use for logging.</param>
		public DataContextLogger(IDataContext dataContext)
		{
			_dataContext = dataContext;
		}

		#endregion

		#region Properties

		/// <summary>
		/// The max level for this logger to process.
		/// </summary>
		public LogLevel Level { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// Write the message to the logger.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="level">The log level of the message.</param>
		public void Write(string message, LogLevel level)
		{
			_dataContext.LogEntries.Add(new LogEntry
			{
				Message = message,
				Level = level,
				ReferenceId = LogManager.ReferenceId,
			});

			_dataContext.SaveChanges();
		}

		#endregion
	}
}
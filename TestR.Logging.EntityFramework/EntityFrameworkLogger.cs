#region References

using System.Data.Entity;
using TestR.Logging.EntityFramework.Migrations;
using TestR.Logging.EntityFramework.Models;

#endregion

namespace TestR.Logging.EntityFramework
{
	public class EntityFrameworkLogger : ILogger
	{
		#region Fields

		private readonly DatabaseDataContext _dataContext;

		#endregion

		#region Constructors

		public EntityFrameworkLogger(string connectionString)
		{
			Database.SetInitializer(new MigrateDatabaseToLatestVersion<DatabaseDataContext, Configuration>());
			_dataContext = new DatabaseDataContext(connectionString);
		}

		#endregion

		#region Properties

		/// <summary>
		/// The max level for this logger to process.
		/// </summary>
		public LogLevel Level { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Write the message to the logger.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="level">The log level of the message.</param>
		public void Write(string message, LogLevel level)
		{
			_dataContext.LogEvents.Add(new LogEvent
			{
				Message = message,
				Level = level.ToString(),
				ReferenceId = LogManager.ReferenceId
			});

			_dataContext.SaveChanges();
		}

		#endregion
	}
}
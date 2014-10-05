#region References

using TestR.Models;

#endregion

namespace TestR.Data
{
	/// <summary>
	/// The data context for TestR.
	/// </summary>
	public interface IDataContext
	{
		#region Properties

		/// <summary>
		/// Gets the list of log entries.
		/// </summary>
		IRepository<LogEntry> LogEntries { get; }

		/// <summary>
		/// Gets the list of users.
		/// </summary>
		IRepository<User> User { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Saves the changes for this context.
		/// </summary>
		/// <returns>A count of the number of items that changed.</returns>
		int SaveChanges();

		#endregion
	}
}
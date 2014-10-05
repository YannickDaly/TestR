namespace TestR.Logging
{
	/// <summary>
	/// Represents the different level of logging.
	/// </summary>
	public enum LogLevel
	{
		/// <summary>
		/// Log level for very detailed messages.
		/// </summary>
		Verbose,
		
		/// <summary>
		/// Log level for debug messages.
		/// </summary>
		Debug,

		/// <summary>
		/// Log level for information messages.
		/// </summary>
		Information,

		/// <summary>
		/// Log level for warning messages.
		/// </summary>
		Warning,

		/// <summary>
		/// Log level for error messages.
		/// </summary>
		Error,

		/// <summary>
		/// Log level for fatal error messages.
		/// </summary>
		Fatal
	}
}
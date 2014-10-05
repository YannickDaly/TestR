namespace TestR.Logging.EntityFramework.Models
{
	public class LogEvent : Entity
	{
		#region Properties

		public string Level { get; set; }
		public string Message { get; set; }
		public string ReferenceId { get; set; }

		#endregion
	}
}
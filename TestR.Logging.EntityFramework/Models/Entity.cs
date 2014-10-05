#region References

using System;

#endregion

namespace TestR.Logging.EntityFramework.Models
{
	public abstract class Entity
	{
		#region Properties

		public DateTime CreatedOn { get; set; }
		public int Id { get; set; }
		public DateTime ModifiedOn { get; set; }

		#endregion
	}
}
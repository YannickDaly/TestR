#region References

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

#endregion

namespace TestR.Logging.EntityFramework.Models.Mappings
{
	internal class LogEventMapping : EntityTypeConfiguration<LogEvent>
	{
		#region Constructors

		public LogEventMapping()
		{
			// Primary Key
			HasKey(t => t.Id);

			// Table & Column Mappings
			ToTable("LogEvents");
			Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			Property(t => t.CreatedOn).IsRequired();
			Property(t => t.Level).IsRequired().HasMaxLength(128);
			Property(t => t.Message).IsRequired();
			Property(t => t.ModifiedOn).IsRequired();
			Property(t => t.ReferenceId).IsRequired();
		}

		#endregion
	}
}
#region References

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TestR.Models;

#endregion

namespace TestR.Data.Mappings
{
	internal class LogEntryMapping : EntityTypeConfiguration<LogEntry>
	{
		#region Constructors

		public LogEntryMapping()
		{
			// Primary Key
			HasKey(t => t.Id);

			// Table & Column Mappings
			ToTable("LogEntries");
			Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			Property(t => t.CreatedOn).IsRequired();
			Property(t => t.Level).IsRequired();
			Property(t => t.Message).IsRequired();
			Property(t => t.ModifiedOn).IsRequired();
			Property(t => t.ReferenceId).HasMaxLength(256);
		}

		#endregion
	}
}
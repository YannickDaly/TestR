#region References

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TestR.Models;

#endregion

namespace TestR.Data.Mappings
{
	internal class UserMapping : EntityTypeConfiguration<User>
	{
		#region Constructors

		public UserMapping()
		{
			// Primary Key
			HasKey(t => t.Id);

			// Table & Column Mappings
			ToTable("Users");
			Property(t => t.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			Property(t => t.CreatedOn).IsRequired();
			Property(t => t.EmailAddress).IsRequired().HasMaxLength(256);
			Property(t => t.ModifiedOn).IsRequired();
			Property(t => t.PasswordHash).IsRequired().HasMaxLength(256);
			Property(t => t.PasswordSalt).IsRequired().HasMaxLength(128);
			Property(t => t.Roles).IsRequired().HasMaxLength(256);
			Property(t => t.UserName).IsRequired().HasMaxLength(128);
		}

		#endregion
	}
}
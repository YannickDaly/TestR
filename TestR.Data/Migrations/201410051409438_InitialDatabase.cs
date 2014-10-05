#region References

using System.Data.Entity.Migrations;

#endregion

namespace TestR.Data.Migrations
{
	public partial class InitialDatabase : DbMigration
	{
		#region Methods

		public override void Down()
		{
			DropTable("dbo.Users");
			DropTable("dbo.LogEntries");
		}

		public override void Up()
		{
			CreateTable(
				"dbo.LogEntries",
				c => new
				{
					Id = c.Int(false, true),
					Level = c.Int(false),
					Message = c.String(false),
					ReferenceId = c.String(maxLength: 256),
					CreatedOn = c.DateTime(false),
					ModifiedOn = c.DateTime(false),
				})
				.PrimaryKey(t => t.Id);

			CreateTable(
				"dbo.Users",
				c => new
				{
					Id = c.Int(false, true),
					EmailAddress = c.String(false, 256),
					PasswordHash = c.String(false, 256),
					PasswordSalt = c.String(false, 128),
					Roles = c.String(false, 256),
					UserName = c.String(false, 128),
					CreatedOn = c.DateTime(false),
					ModifiedOn = c.DateTime(false),
				})
				.PrimaryKey(t => t.Id);
		}

		#endregion
	}
}
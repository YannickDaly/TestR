#region References

using System.Data.Entity.Migrations;

#endregion

namespace TestR.Logging.EntityFramework.Migrations
{
	public partial class InitialDatabase : DbMigration
	{
		#region Methods

		public override void Down()
		{
			DropTable("dbo.LogEvents");
		}

		public override void Up()
		{
			CreateTable(
				"dbo.LogEvents",
				c => new
				{
					Id = c.Int(false, true),
					Level = c.String(false, 128),
					Message = c.String(false),
					ReferenceId = c.String(false),
					CreatedOn = c.DateTime(false),
					ModifiedOn = c.DateTime(false),
				})
				.PrimaryKey(t => t.Id);
		}

		#endregion
	}
}
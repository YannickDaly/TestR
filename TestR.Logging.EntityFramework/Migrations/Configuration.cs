#region References

using System.Data.Entity.Migrations;

#endregion

namespace TestR.Logging.EntityFramework.Migrations
{
	public sealed class Configuration : DbMigrationsConfiguration<DatabaseDataContext>
	{
		#region Constructors

		public Configuration()
		{
			AutomaticMigrationsEnabled = false;

		}

		#endregion

		#region Methods

		protected override void Seed(DatabaseDataContext context)
		{
			//  This method will be called after migrating to the latest version.

			//  You can use the DbSet<T>.AddOrUpdate() helper extension method 
			//  to avoid creating duplicate seed data. E.g.
			//
			//    context.People.AddOrUpdate(
			//      p => p.FullName,
			//      new Person { FullName = "Andrew Peters" },
			//      new Person { FullName = "Brice Lambson" },
			//      new Person { FullName = "Rowan Miller" }
			//    );
			//
		}

		#endregion
	}
}
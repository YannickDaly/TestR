#region References

using System.Data.Entity.Migrations;

#endregion

namespace TestR.Data.Migrations
{
	internal sealed class Configuration : DbMigrationsConfiguration<DataContext>
	{
		#region Constructors

		public Configuration()
		{
			AutomaticMigrationsEnabled = false;
		}

		#endregion

		#region Methods

		protected override void Seed(DataContext context)
		{
		}

		#endregion
	}
}
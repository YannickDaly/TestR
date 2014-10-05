#region References

using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations.History;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TestR.Extensions;
using TestR.Logging.EntityFramework.Models;
using TestR.Logging.EntityFramework.Models.Mappings;

#endregion

namespace TestR.Logging.EntityFramework
{
	[ExcludeFromCodeCoverage]
	[DbConfigurationType(typeof (ModelConfiguration))]
	public class DatabaseDataContext : DbContext
	{
		#region Constructors

		public DatabaseDataContext()
			: base("Name=DefaultConnection")
		{
		}

		public DatabaseDataContext(string connectionString)
			: base(connectionString)
		{
		}

		#endregion

		#region Properties

		public IDbSet<LogEvent> LogEvents { get { return Set<LogEvent>(); } }

		#endregion

		#region Methods

		/// <summary>
		/// Saves all changes made in this context to the underlying database.
		/// </summary>
		/// <returns>
		/// The number of objects written to the underlying database.
		/// </returns>
		/// <exception cref="T:System.InvalidOperationException">Thrown if the context has been disposed.</exception>
		public override int SaveChanges()
		{
			ChangeTracker.Entries().ForEach(ProcessEntity);
			return base.SaveChanges();
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Configurations.Add(new LogEventMapping());
			base.OnModelCreating(modelBuilder);
		}

		/// <summary>
		/// Manages the created on and modified on members of the base entity.
		/// </summary>
		/// <param name="entry"></param>
		private void ProcessEntity(DbEntityEntry entry)
		{
			var states = new[] { EntityState.Added, EntityState.Deleted, EntityState.Modified };
			if (!states.Contains(entry.State))
			{
				// This entity is a state that is not tracked.
				return;
			}

			var entity = entry.Entity as Entity;
			if (entity != null)
			{
				// Check to see if the entity was added.
				if (entry.State == EntityState.Added)
				{
					// Make sure the modified on value matches created on for new items.
					entity.CreatedOn = DateTime.UtcNow;
					entity.ModifiedOn = entity.CreatedOn;
				}

				// Check to see if the entity was modified.
				if (entry.State == EntityState.Modified)
				{
					if (entry.CurrentValues.PropertyNames.Contains("CreatedOn"))
					{
						// Do not allow created on to change for entities.
						entity.CreatedOn = (DateTime) entry.OriginalValues["CreatedOn"];
					}

					// Update modified to now for new entities.
					entity.ModifiedOn = DateTime.UtcNow;
				}
			}
		}

		#endregion

		#region Classes

		private class DbHistoryContext : HistoryContext
		{
			#region Constructors

			public DbHistoryContext(DbConnection dbConnection, string defaultSchema)
				: base(dbConnection, defaultSchema)
			{
			}

			#endregion

			#region Methods

			protected override void OnModelCreating(DbModelBuilder modelBuilder)
			{
				base.OnModelCreating(modelBuilder);
				modelBuilder.Entity<HistoryRow>().ToTable("MigrationHistory", "system");
			}

			#endregion
		}

		private class ModelConfiguration : DbConfiguration
		{
			#region Constructors

			public ModelConfiguration()
			{
				SetHistoryContext("System.Data.SqlClient", (connection, defaultSchema) => new DbHistoryContext(connection, defaultSchema));
			}

			#endregion
		}

		#endregion
	}
}
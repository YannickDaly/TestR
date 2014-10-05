#region References

using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations.History;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TestR.Data.Mappings;
using TestR.Data.Migrations;
using TestR.Extensions;
using TestR.Models;

#endregion

namespace TestR.Data
{
	[ExcludeFromCodeCoverage]
	[DbConfigurationType(typeof (ModelConfiguration))]
	public class DataContext : DbContext, IDataContext
	{
		#region Constructors

		public DataContext()
			: base("Name=DefaultConnection")
		{
		}

		public DataContext(string connectionString)
			: base(connectionString)
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the list of log entries.
		/// </summary>
		public IRepository<LogEntry> LogEntries
		{
			get { return new Repository<LogEntry>(Set<LogEntry>()); }
		}
		
		/// <summary>
		/// Gets the list of users.
		/// </summary>
		public IRepository<User> User
		{
			get { return new Repository<User>(Set<User>()); }
		}

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
			modelBuilder.Configurations.Add(new LogEntryMapping());
			modelBuilder.Configurations.Add(new UserMapping());

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

		#region Static Methods

		public static void InitializeMigrations()
		{
			Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, Configuration>());
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
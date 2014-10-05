#region References

using System.Linq;
using TestR.Models;

#endregion

namespace TestR.Data
{
	/// <summary>
	/// Represents a repository of entities.
	/// </summary>
	/// <typeparam name="T">The type of the entity for this repository.</typeparam>
	public interface IRepository<T> : IQueryable<T> where T : Entity
	{
		#region Methods

		/// <summary>
		/// Add an entity to the repository.
		/// </summary>
		/// <param name="entity">The entity to be added.</param>
		void Add(T entity);

		/// <summary>
		/// Add or update an entity to the repository.
		/// </summary>
		/// <param name="entity">The entity to be added or updated.</param>
		void AddOrUpdate(T entity);

		/// <summary>
		/// Remove the entity from the repository.
		/// </summary>
		/// <param name="entity">The entity to be removed.</param>
		void Remove(T entity);

		#endregion
	}
}
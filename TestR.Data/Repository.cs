#region References

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using TestR.Models;

#endregion

namespace TestR.Data
{
	[ExcludeFromCodeCoverage]
	public class Repository<T> : IRepository<T> where T : Entity
	{
		#region Fields

		private readonly IDbSet<T> _set;

		#endregion

		#region Constructors

		public Repository(IDbSet<T> set)
		{
			_set = set;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the type of the element(s) that are returned when the expression tree associated with this instance of <see cref="T:System.Linq.IQueryable"/> is executed.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Type"/> that represents the type of the element(s) that are returned when the expression tree associated with this object is executed.
		/// </returns>
		public Type ElementType
		{
			get { return _set.ElementType; }
		}

		/// <summary>
		/// Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable"/>.
		/// </summary>
		/// <returns>
		/// The <see cref="T:System.Linq.Expressions.Expression"/> that is associated with this instance of <see cref="T:System.Linq.IQueryable"/>.
		/// </returns>
		public Expression Expression
		{
			get { return _set.Expression; }
		}

		/// <summary>
		/// Gets the query provider that is associated with this data source.
		/// </summary>
		/// <returns>
		/// The <see cref="T:System.Linq.IQueryProvider"/> that is associated with this data source.
		/// </returns>
		public IQueryProvider Provider
		{
			get { return _set.Provider; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Add an entity to the repository.
		/// </summary>
		/// <param name="entity">The entity to be added.</param>
		public void Add(T entity)
		{
			_set.Add(entity);
		}

		/// <summary>
		/// Add or update an entity to the repository.
		/// </summary>
		/// <param name="entity">The entity to be added or updated.</param>
		public void AddOrUpdate(T entity)
		{
			_set.AddOrUpdate(entity);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<T> GetEnumerator()
		{
			return _set.GetEnumerator();
		}

		/// <summary>
		/// Remove the entity from the repository.
		/// </summary>
		/// <param name="entity">The entity to be removed.</param>
		public void Remove(T entity)
		{
			_set.Remove(entity);
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
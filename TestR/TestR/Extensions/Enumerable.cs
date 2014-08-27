#region References

using System;
using System.Collections.Generic;

#endregion

namespace TestR.Extensions
{
	/// <summary>
	/// Represents a static helper class.
	/// </summary>
	public static partial class Helper
	{
		#region Static Methods

		/// <summary>
		/// Adds a range of items to the collection.
		/// </summary>
		/// <param name="collection">The collection to add the items to.</param>
		/// <param name="items">The items to add to the collection.</param>
		/// <typeparam name="T">The type of the item in the collections.</typeparam>
		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
		{
			foreach (var item in items)
			{
				collection.Add(item);
			}
		}

		/// <summary>
		/// Performs an action on each item in the collection.
		/// </summary>
		/// <param name="collection">The collection of items to run the action with.</param>
		/// <param name="action">The action to run against each item in the collection.</param>
		/// <typeparam name="T">The type of the item in the collection.</typeparam>
		public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
		{
			foreach (var item in collection)
			{
				action(item);
			}
		}

		/// <summary>
		/// Converts a IEnumerable collection into an ElementCollection.
		/// </summary>
		/// <param name="collection">The collection to be converted.</param>
		/// <typeparam name="T">The type of the item in the collection.</typeparam>
		/// <returns>The new element collection of item.</returns>
		public static ElementCollection<T> ToElementCollection<T>(this IEnumerable<T> collection)
			where T : Element
		{
			return new ElementCollection<T>(collection);
		}

		/// <summary>
		/// Converts a IEnumerable collection into an ElementCollection.
		/// </summary>
		/// <param name="collection">The collection to be converted.</param>
		/// <returns>The new element collection of item.</returns>
		public static ElementCollection ToElementCollection(this IEnumerable<Element> collection)
		{
			return new ElementCollection(collection);
		}

		#endregion
	}
}
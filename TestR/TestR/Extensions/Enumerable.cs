#region References

using System;
using System.Collections.Generic;
using TestR.Collections;

#endregion

namespace TestR.Extensions
{
	public static class Helper
	{
		#region Static Methods

		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
		{
			foreach (var item in items)
			{
				collection.Add(item);
			}
		}

		public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
		{
			foreach (var item in collection)
			{
				action(item);
			}
		}

		public static ElementCollection<T> ToElementCollection<T>(this IEnumerable<T> collection)
			where T : Element
		{
			return new ElementCollection<T>(collection);
		}

		public static ElementCollection ToElementCollection(this IEnumerable<Element> collection)
		{
			return new ElementCollection(collection);
		}

		#endregion
	}
}
#region References

using System.Collections;
using System.Collections.Generic;
using mshtml;

#endregion

namespace TestR.Browsers
{
	public class InternetExplorerElementCollection : IReadOnlyCollection<Element>
	{
		#region Fields

		private readonly Browser _browser;
		private readonly IHTMLElementCollection _collection;

		#endregion

		#region Constructors

		public InternetExplorerElementCollection(IHTMLElementCollection collection, Browser browser)
		{
			_collection = collection;
			_browser = browser;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the number of elements in the collection.
		/// </summary>
		/// <returns>
		/// The number of elements in the collection. 
		/// </returns>
		public int Count
		{
			get { return _collection.length; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<Element> GetEnumerator()
		{
			foreach (IHTMLElement htmlElement in _collection)
			{
				yield return new Element(new InternetExplorerElement(htmlElement, _browser));
			}
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
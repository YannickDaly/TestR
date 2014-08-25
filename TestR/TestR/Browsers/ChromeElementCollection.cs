#region References

using System.Collections;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace TestR.Browsers
{
	/// <summary>
	/// Represents a collection of Internet Explorer elements.
	/// </summary>
	public class ChromeElementCollection : IReadOnlyCollection<Element>
	{
		#region Fields

		private readonly Browser _browser;
		private readonly IEnumerable<ChromeElement> _elements;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes an instance of InternetExplorerElementCollection class.
		/// </summary>
		public ChromeElementCollection(IEnumerable<ChromeElement> elements, Browser browser)
		{
			_elements = elements;
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
			get { return _elements.Count(); }
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
			foreach (var element in _elements)
			{
				yield return new Element(element);
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
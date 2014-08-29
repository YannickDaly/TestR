#region References

using Newtonsoft.Json.Linq;

#endregion

namespace TestR.Elements
{
	/// <summary>
	/// Represent a browser link element.
	/// </summary>
	public class Link : Element
	{
		#region Constructors

		/// <summary>
		/// Initializes an instance of a Link browser element.
		/// </summary>
		/// <param name="element">The browser element this is for.</param>
		/// <param name="browser">The browser this element is associated with.</param>
		/// <param name="collection">The collection this element is associated with.</param>
		public Link(JToken element, Browser browser, ElementCollection collection)
			: base(element, browser, collection)
		{
		}

		#endregion
	}
}
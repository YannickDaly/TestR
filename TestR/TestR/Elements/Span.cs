#region References

using Newtonsoft.Json.Linq;

#endregion

namespace TestR.Elements
{
	/// <summary>
	/// Represents a browser span element.
	/// </summary>
	public class Span : Element
	{
		#region Constructors

		/// <summary>
		/// Initializes an instance of a browser element.
		/// </summary>
		/// <param name="element">The browser element this is for.</param>
		/// <param name="browser">The browser this element is associated with.</param>
		/// <param name="collection">The collection this element is associated with.</param>
		public Span(JToken element, Browser browser, ElementCollection collection)
			: base(element, browser, collection)
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the text for this span.
		/// </summary>
		public string Text
		{
			get { return this["innerText"]; }
			set { this["innerText"] = value; }
		}

		#endregion
	}
}
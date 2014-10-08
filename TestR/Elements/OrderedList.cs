#region References

using Newtonsoft.Json.Linq;

#endregion

namespace TestR.Elements
{
	/// <summary>
	/// Represents a browser ordered list element.
	/// </summary>
	public class OrderedList : Element
	{
		#region Constructors

		/// <summary>
		/// Initializes an instance of a browser element.
		/// </summary>
		/// <param name="element">The browser element this is for.</param>
		/// <param name="browser">The browser this element is associated with.</param>
		/// <param name="collection">The collection this element is associated with.</param>
		public OrderedList(JToken element, Browser browser, ElementCollection collection)
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
			get { return this[Browser.Type == BrowserType.Firefox ? "textContent" : "innerText"]; }
			set { this[Browser.Type == BrowserType.Firefox ? "textContent" : "innerText"] = value; }
		}

		#endregion
	}
}
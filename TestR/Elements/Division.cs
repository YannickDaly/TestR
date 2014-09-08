#region References

using Newtonsoft.Json.Linq;

#endregion

namespace TestR.Elements
{
	/// <summary>
	/// Represents a browser division (div) element.
	/// </summary>
	public class Division : Element
	{
		#region Constructors

		/// <summary>
		/// Initializes an instance of a browser element.
		/// </summary>
		/// <param name="element">The browser element this is for.</param>
		/// <param name="browser">The browser this element is associated with.</param>
		/// <param name="collection">The collection this element is associated with.</param>
		public Division(JToken element, Browser browser, ElementCollection collection)
			: base(element, browser, collection)
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the text for this division.
		/// </summary>
		public string Text
		{
			get { return this[Browser.Type == BrowserType.Firefox ? "textContent" : "innerText"]; }
			set { this[Browser.Type == BrowserType.Firefox ? "textContent" : "innerText"] = value; }
		}

		#endregion
	}
}
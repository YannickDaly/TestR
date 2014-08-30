#region References

using Newtonsoft.Json.Linq;

#endregion

namespace TestR.Elements
{
	/// <summary>
	/// Represent a browser button element.
	/// </summary>
	public class Button : Element
	{
		#region Constructors

		/// <summary>
		/// Initializes an instance of a Button browser element.
		/// </summary>
		/// <param name="element">The browser element this is for.</param>
		/// <param name="browser">The browser this element is associated with.</param>
		/// <param name="collection">The collection this element is associated with.</param>
		public Button(JToken element, Browser browser, ElementCollection collection)
			: base(element, browser, collection)
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the text.
		/// </summary>
		public string Text
		{
			get { return this["value"]; }
			set { this["value"] = value; }
		}

		#endregion
	}
}
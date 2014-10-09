#region References

using System.Collections.Generic;
using Newtonsoft.Json.Linq;

#endregion

namespace TestR.Elements
{
	/// <summary>
	/// Represents a browser select element.
	/// </summary>
	public class Select : Element
	{
		#region Constructors

		/// <summary>
		/// Initializes an instance of a browser element.
		/// </summary>
		/// <param name="element">The browser element this is for.</param>
		/// <param name="browser">The browser this element is associated with.</param>
		/// <param name="collection">The collection this element is associated with.</param>
		public Select(JToken element, Browser browser, ElementCollection collection)
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
			set
			{
				this[Browser.Type == BrowserType.Firefox ? "textContent" : "innerText"] = value;
				TriggerElement();
			}
		}

		/// <summary>
		/// Gets or sets the value for this select.
		/// </summary>
		public string Value
		{
			get { return this["value"]; }
			set
			{
				this["value"] = value;
				TriggerElement();
			}
		}

		#endregion
	}
}
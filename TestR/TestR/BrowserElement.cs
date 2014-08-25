#region References

using System.Collections.Generic;
using System.Collections.Specialized;

#endregion

namespace TestR
{
	/// <summary>
	/// Represents an element for a browser.
	/// </summary>
	public abstract class BrowserElement
	{
		#region Fields

		/// <summary>
		/// Properties that need to be renamed when requested.
		/// </summary>
		public static readonly Dictionary<string, string> PropertiesToRename = new Dictionary<string, string>
		{
			{ "class", "className" }
		};

		/// <summary>
		/// Attributes of elements that need to be access by properties rather than as attributes.
		/// </summary>
		public static readonly IList<string> UsePropertyInsteadOfAttribute = new[]
		{
			"selected", "textContent", "className", "checked", "readOnly", "multiple", "value",
			"nodeType", "innerText", "innerHTML", "baseURI", "src", "href", "rowIndex", "cellIndex"
		};

		#endregion

		#region Properties

		/// <summary>
		/// Gets the browser this element is currently associated with.
		/// </summary>
		public abstract Browser Browser { get; }

		/// <summary>
		/// Gets the ID of the element.
		/// </summary>
		public abstract string Id { get; }

		/// <summary>
		/// Gets the name of the element.
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// Gets the tag name of the element.
		/// </summary>
		public abstract string TagName { get; }

		/// <summary>
		/// Gets the text of the element.
		/// </summary>
		public string Text
		{
			get { return GetAttributeValue("innerText"); }
		}

		/// <summary>
		/// Gets the delay (in milliseconds) between each character.
		/// </summary>
		public abstract int TypingDelay { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Clicks the element.
		/// </summary>
		public abstract void Click();

		/// <summary>
		/// Focuses on the element.
		/// </summary>
		public abstract void Focus();

		/// <summary>
		/// Gets an attribute value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute to read.</param>
		/// <returns>The attribute value.</returns>
		public abstract string GetAttributeValue(string name);

		/// <summary>
		/// Gets an attribute style value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute style to read.</param>
		/// <returns>The attribute style value.</returns>
		public abstract string GetStyleAttributeValue(string name);

		/// <summary>
		/// Sets an attribute value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute to write.</param>
		/// <param name="value">The value to be written.</param>
		public abstract void SetAttributeValue(string name, string value);

		/// <summary>
		/// Sets an attribute style value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute style to write.</param>
		/// <param name="value">The style value to be written.</param>
		public abstract void SetStyleAttributeValue(string name, string value);

		/// <summary>
		/// Type text into the element.
		/// </summary>
		/// <param name="value">The value to be typed.</param>
		public abstract void TypeText(string value);

		/// <summary>
		/// Process values to ensure values for specific attributes to the correct value.
		/// </summary>
		/// <param name="attributeName">The attribute to be set.</param>
		/// <param name="value">The value to set the attribute to.</param>
		/// <returns>The value to be used to set the attribute.</returns>
		protected string ProcessAttributeValue(string attributeName, string value)
		{
			// selected is attribute of Option
			// checked is attribute of RadioButton and CheckBox
			if (attributeName == "selected" || attributeName == "checked")
			{
				value = bool.Parse(value) ? "true" : "";
			}

			return value;
		}
		
		protected Dictionary<string, string> GetKeyCodeEventProperty(char character)
		{
			return new Dictionary<string, string>
			{
				{ "keyCode", ((int) character).ToString() },
				{ "charCode", ((int) character).ToString() },
				{ "which", ((int) character).ToString() },
			};
		}
		
		#endregion
	}
}
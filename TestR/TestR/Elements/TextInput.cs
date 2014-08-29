#region References

using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Linq;

#endregion

namespace TestR.Elements
{
	/// <summary>
	/// Represents a browser text input element.
	/// </summary>
	public class TextInput : Element
	{
		#region Constructors

		/// <summary>
		/// Initializes an instance of a TextInput browser element.
		/// </summary>
		/// <param name="element">The browser element this is for.</param>
		/// <param name="browser">The browser this element is associated with.</param>
		/// <param name="collection">The collection this element is associated with.</param>
		public TextInput(JToken element, Browser browser, ElementCollection collection)
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

		/// <summary>
		///  Gets the delay (in milliseconds) between each character.
		/// </summary>
		public int TypingDelay
		{
			get { return Browser.SlowMotion ? 50 : 1; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Type text into the element.
		/// </summary>
		/// <param name="value">The value to be typed.</param>
		public void TypeText(string value)
		{
			Focus();
			Highlight(true);

			if (!Browser.SlowMotion)
			{
				SetAttributeValue("value", GetAttributeValue("value", true) + value);
				FireEvent("onChange", new Dictionary<string, string>());
			}
			else
			{
				foreach (var character in value)
				{
					var eventProperty = GetKeyCodeEventProperty(character);
					FireEvent("keyDown", eventProperty);
					FireEvent("keyPress", eventProperty);
					FireEvent("keyUp", eventProperty);

					var newValue = GetAttributeValue("value", true) + character;
					SetAttributeValue("value", newValue);
					Thread.Sleep(TypingDelay);
				}
			}

			Highlight(false);
			TriggerElement();
		}

		#endregion
	}
}
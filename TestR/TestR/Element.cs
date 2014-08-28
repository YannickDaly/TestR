#region References

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NLog;

#endregion

namespace TestR
{
	/// <summary>
	/// Represents an element for a browser.
	/// </summary>
	public class Element
	{
		#region Fields

		/// <summary>
		/// Properties that need to be renamed when requested.
		/// </summary>
		public static readonly Dictionary<string, string> PropertiesToRename = new Dictionary<string, string>
		{
			{ "class", "className" }
		};

		private readonly Browser _browser;
		private readonly dynamic _element;
		private readonly string _highlightColor;
		private readonly string _orginalColor;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes an instance of an Internet Explorer browser element.
		/// </summary>
		/// <param name="element">The browser element this is for.</param>
		/// <param name="browser">The browser this element is associated with.</param>
		public Element(dynamic element, Browser browser)
		{
			_element = element;
			_browser = browser;
			_orginalColor = GetStyleAttributeValue("backgroundColor") ?? "";
			_highlightColor = "yellow";
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the browser this element is currently associated with.
		/// </summary>
		public Browser Browser
		{
			get { return _browser; }
		}

		/// <summary>
		/// Gets the class value of the element.
		/// </summary>
		public string Class
		{
			get { return GetAttributeValue("className", true); }
		}

		/// <summary>
		/// Gets or sets the type of the element.
		/// </summary>
		public ElementType ElementType { get; set; }

		/// <summary>
		/// Gets the ID of the element.
		/// </summary>
		public string Id
		{
			get { return _element.id; }
		}

		/// <summary>
		/// Gets the name of the element.
		/// </summary>
		public string Name
		{
			get { return _element.name; }
		}

		/// <summary>
		/// Gets the tag name of the element.
		/// </summary>
		public string TagName
		{
			get { return _element.tagName; }
		}

		/// <summary>
		/// Gets the text of the element.
		/// </summary>
		public string Text
		{
			get
			{
				var name = ElementType == ElementType.TextInput ? "value" : "innerText";
				return GetAttributeValue(name, true);
			}
		}

		/// <summary>
		///  Gets the delay (in milliseconds) between each character.
		/// </summary>
		public int TypingDelay
		{
			get { return Browser.SlowMotion ? 50 : 1; }
		}

		/// <summary>
		/// Gets or sets the value for the text input.
		/// </summary>
		public string Value
		{
			get { return GetAttributeValue("value", true); }
			set { SetAttributeValue("value", value); }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Clicks the element.
		/// </summary>
		public void Click()
		{
			Browser.ExecuteScript("document.getElementById('" + Id + "').click()");
		}

		/// <summary>
		/// Fires an event on the element.
		/// </summary>
		/// <param name="eventName">The events name to fire.</param>
		/// <param name="eventProperties">The properties for the event.</param>
		public void FireEvent(string eventName, Dictionary<string, string> eventProperties)
		{
			var values = eventProperties.Aggregate("", (current, item) => current + ("{ key: '" + item.Key + "', value: '" + item.Value + "'},"));
			if (values.Length > 0)
			{
				values = values.Remove(values.Length - 1, 1);
			}

			var script = "TestR.triggerEvent(document.getElementById('" + Id + "'), '" + eventName.ToLower() + "', [" + values + "]);";
			Browser.ExecuteScript(script);
		}

		/// <summary>
		/// Focuses on the element.
		/// </summary>
		public void Focus()
		{
			Browser.ExecuteScript("document.getElementById('" + Id + "').focus()");
		}

		/// <summary>
		/// Gets an attribute value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute to read.</param>
		/// <param name="refresh">A flag to force the element to refresh.</param>
		/// <returns>The attribute value.</returns>
		public string GetAttributeValue(string name, bool refresh = false)
		{
			name = PropertiesToRename.ContainsKey(name) ? PropertiesToRename[name] : name;
			string value;

			if (refresh)
			{
				var script = "TestR.getElementValue('" + Id + "','" + name + "')";
				value = Browser.ExecuteScript(script);
			}
			else
			{
				value = GetElementAttribute(name);
			}

			if (string.IsNullOrWhiteSpace(value))
			{
				return string.Empty;
			}
			
			if (name.ToLower() == "selected" && value.ToLower() == "selected")
			{
				value = "true";
			}

			SetElementAttributeValue(name, value);
			return value;
		}

		/// <summary>
		/// Gets an attribute style value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute style to read.</param>
		/// <param name="forceRefresh">A flag to force the element to refresh.</param>
		/// <returns>The attribute style value.</returns>
		public string GetStyleAttributeValue(string name, bool forceRefresh = false)
		{
			var styleValue = GetAttributeValue("style", forceRefresh);
			if (styleValue == null)
			{
				return string.Empty;
			}

			var styleValues = styleValue.Split(';')
				.Select(x => x.Split(':'))
				.Where(x => x.Length == 2)
				.Select(x => new KeyValuePair<string, string>(x[0].Trim(), x[1].Trim()))
				.ToList()
				.ToDictionary(x => x.Key, x => x.Value);

			return styleValues.ContainsKey(name) ? styleValues[name] : string.Empty;
		}

		/// <summary>
		/// Highlight or resets the element.
		/// </summary>
		/// <param name="highlight">If true the element is highlight yellow. If false the element is returned to its original color.</param>
		public void Highlight(bool highlight)
		{
			Logger.Write(highlight ? "Adding highlight to element " + Id + "." : "Removing highlight from element " + Id + ".", LogLevel.Trace);
			SetStyleAttributeValue("background-color", highlight ? _highlightColor : _orginalColor);

			if (Browser.SlowMotion && highlight)
			{
				Thread.Sleep(150);
			}
		}

		/// <summary>
		/// Sets an attribute value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute to write.</param>
		/// <param name="value">The value to be written.</param>
		public void SetAttributeValue(string name, string value)
		{
			name = PropertiesToRename.ContainsKey(name) ? PropertiesToRename[name] : name;
			var script = "TestR.setElementValue('" + Id + "','" + name + "','" + value + "')";
			_browser.ExecuteScript(script);
			AddOrUpdateElementAttribute(name, value);
		}

		/// <summary>
		/// Sets an attribute style value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute style to write.</param>
		/// <param name="value">The style value to be written.</param>
		public void SetStyleAttributeValue(string name, string value)
		{
			var styleValue = GetElementAttribute("style") ?? string.Empty;
			var styleValues = styleValue
				.Split(';')
				.Select(x => x.Split(':'))
				.Where(x => x.Length == 2)
				.Select(x => new KeyValuePair<string, string>(x[0], x[1]))
				.ToList()
				.ToDictionary(x => x.Key, x => x.Value);

			if (!styleValues.ContainsKey(name))
			{
				styleValues.Add(name, value);
			}

			if (string.IsNullOrWhiteSpace(value))
			{
				styleValues.Remove(name);
			}
			else
			{
				styleValues[name] = value;
			}

			styleValue = string.Join(";", styleValues.Select(x => x.Key + ":" + x.Value));
			SetAttributeValue("style", styleValue);
		}

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

		/// <summary>
		/// Get the key code event properties for the character.
		/// </summary>
		/// <param name="character">The character to get the event properties for.</param>
		/// <returns>An event properties for the character.</returns>
		protected Dictionary<string, string> GetKeyCodeEventProperty(char character)
		{
			return new Dictionary<string, string>
			{
				{ "keyCode", ((int) character).ToString() },
				{ "charCode", ((int) character).ToString() },
				{ "which", ((int) character).ToString() },
			};
		}

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

		private void AddOrUpdateElementAttribute(string name, string value)
		{
			for (var i = 0; i < _element.attributes.Count; i++)
			{
				string attributeName = _element.attributes[i++].ToString();
				if (attributeName == name)
				{
					_element.attributes[i] = value;
					return;
				}
			}

			_element.attributes.Add(name);
			_element.attributes.Add(value);
		}

		private string GetElementAttribute(string name)
		{
			for (var i = 0; i < _element.attributes.Count; i++)
			{
				string attributeName = _element.attributes[i++].ToString();
				if (attributeName == name)
				{
					return _element.attributes[i].ToString();
				}
			}

			return null;
		}

		private void SetElementAttributeValue(string name, string value)
		{
			for (var i = 0; i < _element.attributes.Count; i++)
			{
				string attributeName = _element.attributes[i++].ToString();
				if (attributeName != name)
				{
					continue;
				}

				_element.attributes[i] = value;
				return;
			}
		}

		/// <summary>
		/// Triggers the element via the Angular function "trigger".
		/// </summary>
		private void TriggerElement()
		{
			if (Browser.JavascriptLibraries.Contains(JavaScriptLibrary.Angular)
				&& Browser.JavascriptLibraries.Contains(JavaScriptLibrary.JQuery))
			{
				Browser.ExecuteScript("angular.element('#" + Id + "').trigger('input');");
			}
		}

		#endregion
	}
}
#region References

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using mshtml;

#endregion

namespace TestR.Browsers
{
	/// <summary>
	/// Represents an element for the Chrome browser.
	/// </summary>
	public class ChromeElement : BrowserElement
	{
		#region Fields

		private readonly ChromeBrowser _browser;
		private readonly dynamic _element;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes an instance of an Internet Explorer browser element.
		/// </summary>
		/// <param name="element">The browser element this is for.</param>
		/// <param name="browser">The browser this element is associated with.</param>
		public ChromeElement(dynamic element, ChromeBrowser browser)
		{
			_element = element;
			_browser = browser;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the browser this element is currently associated with.
		/// </summary>
		public override Browser Browser
		{
			get { return _browser; }
		}

		/// <summary>
		/// Gets the ID of the element.
		/// </summary>
		public override string Id
		{
			get { return GetAttributeValue("id"); }
		}

		/// <summary>
		/// Gets the name of the element.
		/// </summary>
		public override string Name
		{
			get { return GetAttributeValue("name"); }
		}

		/// <summary>
		/// Gets the ID of the node for the current page.
		/// </summary>
		public int NodeId
		{
			get { return (int) _element.nodeId; }
		}

		/// <summary>
		/// Gets the tag name of the element.
		/// </summary>
		public override string TagName
		{
			get { return _element.nodeName.ToString().ToLower(); }
		}

		/// <summary>
		/// Gets the delay between each character.
		/// </summary>
		public override int TypingDelay
		{
			get { return Browser.SlowMotion ? 50 : 1; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Clicks the element.
		/// </summary>
		public override void Click()
		{
			_browser.Connector.ExecuteJavascript("document.getElementById('" + Id + "').click()");
		}

		/// <summary>
		/// Focuses on the element.
		/// </summary>
		public override void Focus()
		{
			_browser.Connector.ExecuteJavascript("document.getElementById('" + Id + "').focus()");
		}

		/// <summary>
		/// Gets an attribute value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute to read.</param>
		/// <returns>The attribute value.</returns>
		public override string GetAttributeValue(string name)
		{
			name = PropertiesToRename.ContainsKey(name)
				? PropertiesToRename[name] : name;

			var attributeValue = UsePropertyInsteadOfAttribute.Contains(name)
				? _browser.Connector.GetProperty(this, name)
				: GetAttributeFromElement(name);

			if (attributeValue == null)
			{
				return string.Empty;
			}

			var value = attributeValue;
			if (name.ToLower() == "selected" && value.ToLower() == "selected")
			{
				value = "true";
			}

			return value;
		}

		/// <summary>
		/// Fires an event on the element.
		/// </summary>
		/// <param name="eventName">The events name to fire.</param>
		/// <param name="eventProperties">The properties for the event.</param>
		public void FireEvent(string eventName, Dictionary<string,string> eventProperties)
		{
			var values = eventProperties.Aggregate("", (current, item) => current + ("{ key: '" + item.Key + "', value: '" + item.Value + "'},"));
			if (values.Length > 0)
			{
				values = values.Remove(values.Length - 1, 1);
			}

			var script = "TestR.triggerEvent(document.getElementById('" + Id + "'), '" + eventName.ToLower() +"', [" + values + "]);";
			Browser.ExecuteJavascript(script);
		}

		/// <summary>
		/// Gets an attribute style value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute style to read.</param>
		/// <returns>The attribute style value.</returns>
		public override string GetStyleAttributeValue(string name)
		{
			var styleValue = GetAttributeValue("style");
			var styleValues = styleValue.Split(';')
				.Select(x => x.Split(':'))
				.Where(x => x.Length == 2)
				.Select(x => new KeyValuePair<string, string>(x[0].Trim(), x[1].Trim()))
				.ToList()
				.ToDictionary(x => x.Key, x => x.Value);

			return styleValues.ContainsKey(name) ? styleValues[name] : string.Empty;
		}

		/// <summary>
		/// Sets an attribute value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute to write.</param>
		/// <param name="value">The value to be written.</param>
		public override void SetAttributeValue(string name, string value)
		{
			if (UsePropertyInsteadOfAttribute.Contains(name))
			{
				var script = "document.getElementById('" + Id + "')." + name + " = '" + value + "'";
				_browser.Connector.ExecuteJavascript(script);
			}
			else
			{
				_browser.Connector.SetAttribute(this, name, value);
			}
			AddOrUpdateElementAttribute(name, value);
		}

		/// <summary>
		/// Sets an attribute style value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute style to write.</param>
		/// <param name="value">The style value to be written.</param>
		public override void SetStyleAttributeValue(string name, string value)
		{
			var styleValue = GetAttributeFromElement("style");
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
		public override void TypeText(string value)
		{
			if (!Browser.SlowMotion)
			{
				SetAttributeValue("value", value);
				FireEvent("onChange", new Dictionary<string, string>());
				return;
			}

			foreach (var character in value)
			{
				var eventProperty = GetKeyCodeEventProperty(character);
				FireEvent("keyDown", eventProperty);
				FireEvent("keyPress", eventProperty);
				FireEvent("keyUp", eventProperty);
				
				var newValue = GetAttributeValue("value") + character;
				SetAttributeValue("value", newValue);

				//Thread.Sleep(TypingDelay);
			}
		}

		private void FireKeyEvents(int keyCode)
		{
			var upperCode = (int) char.ToUpper((char) keyCode);
			var script = "TestR.triggerKeyboardEvent(document.getElementById('" + Id + "'), 'keydown', " + upperCode + ");";
			Browser.ExecuteJavascript(script);
			script = "TestR.triggerKeyboardEvent(document.getElementById('" + Id + "'), 'keypress', " + keyCode + ");";
			Browser.ExecuteJavascript(script);
			script = "TestR.triggerKeyboardEvent(document.getElementById('" + Id + "'), 'keyup', " + upperCode + ");";
			Browser.ExecuteJavascript(script);
		}

		internal void Initialize()
		{
			// Make sure all things have IDs.
			if (string.IsNullOrWhiteSpace(Id))
			{
				SetAttributeValue("id", Guid.NewGuid().ToString());
			}
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

		private string GetAttributeFromElement(string name)
		{
			for (var i = 0; i < _element.attributes.Count; i++)
			{
				string attributeName = _element.attributes[i++].ToString();
				if (attributeName == name)
				{
					return _element.attributes[i].ToString();
				}
			}

			return string.Empty;
		}

		#endregion
	}
}
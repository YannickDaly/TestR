#region References

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Expando;
using System.Threading;
using mshtml;

#endregion

namespace TestR.Browsers
{
	/// <summary>
	/// Represents an element for the Internet Explorer browser.
	/// </summary>
	public class InternetExplorerElement : BrowserElement
	{
		#region Fields

		private readonly InternetExplorerBrowser _browser;
		private readonly IHTMLElement _element;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes an instance of an Internet Explorer browser element.
		/// </summary>
		/// <param name="element">The browser element this is for.</param>
		/// <param name="browser">The browser this element is associated with.</param>
		public InternetExplorerElement(IHTMLElement element, InternetExplorerBrowser browser)
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
		/// Gets the tag name of the element.
		/// </summary>
		public override string TagName
		{
			get { return _element.tagName.ToLower(); }
		}

		/// <summary>
		/// Gets the delay between each character.
		/// </summary>
		public override int TypingDelay
		{
			get { return Browser.SlowMotion ? 150 : 1; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Clicks the element.
		/// </summary>
		public override void Click()
		{
			_element.click();
		}

		/// <summary>
		/// Fires an event on the element.
		/// </summary>
		/// <param name="eventName">The events name to fire.</param>
		/// <param name="eventProperties">The properties for the event.</param>
		public void FireEvent(string eventName, Dictionary<string, string> eventProperties)
		{
			var element = (IHTMLElement3) _element;
			var result = eventProperties == null
				? element.FireEvent(eventName)
				: element.FireEvent(eventName, eventProperties);

			if (!result)
			{
				throw new Exception("Failed to fire the event?");
			}
		}

		/// <summary>
		/// Focuses on the element.
		/// </summary>
		public override void Focus()
		{
			((IHTMLElement2) _element).focus();
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
				? GetExpandoValue(name)
				: _element.getAttribute(name);

			if (attributeValue == null)
			{
				return string.Empty;
			}

			var value = attributeValue.ToString();
			if (name.ToLower() == "selected" && value.ToLower() == "selected")
			{
				value = "true";
			}

			return value;
		}

		/// <summary>
		/// Gets an attribute style value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute style to read.</param>
		/// <returns>The attribute style value.</returns>
		public override string GetStyleAttributeValue(string name)
		{
			return _element.style.getAttribute(name);
		}

		/// <summary>
		/// Sets an attribute value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute to write.</param>
		/// <param name="value">The value to be written.</param>
		public override void SetAttributeValue(string name, string value)
		{
			value = ProcessAttributeValue(name, value);
			_element.setAttribute(name, value, 0);
		}

		/// <summary>
		/// Sets an attribute style value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute style to write.</param>
		/// <param name="value">The style value to be written.</param>
		public override void SetStyleAttributeValue(string name, string value)
		{
			_element.style.setAttribute(name, value);
		}

		/// <summary>
		/// Type text into the element.
		/// </summary>
		/// <param name="value">The value to be typed.</param>
		public override void TypeText(string value)
		{
			foreach (var character in value)
			{
				var eventProperty = GetKeyCodeEventProperty(character);
				FireEvent("onKeyDown", eventProperty);
				FireEvent("onKeyPress", eventProperty);
				FireEvent("onKeyUp", eventProperty);

				var newValue = GetAttributeValue("value") + character;
				SetAttributeValue("value", newValue);

				Thread.Sleep(TypingDelay);
			}
		}

		private object GetExpandoValue(string attributeName)
		{
			var expando = (IExpando) _element;
			var property = expando.GetProperty(attributeName, BindingFlags.Default);
			if (property == null)
			{
				return null;
			}

			try
			{
				return property.GetValue(expando, null);
			}
			catch (COMException)
			{
				return null;
			}
		}

		#endregion
	}
}
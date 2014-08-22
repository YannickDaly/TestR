#region References

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Expando;
using mshtml;

#endregion

namespace TestR.Browsers
{
	/// <summary>
	/// Represents an element for the Internet Explorer browser.
	/// </summary>
	public class InternetExplorerElement : IBrowserElement
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
			"nodeType", "innerHTML", "baseURI", "src", "href", "rowIndex", "cellIndex"
		};

		private readonly IHTMLElement _element;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes an instance of an Internet Explorer browser element.
		/// </summary>
		/// <param name="element">The browser element this is for.</param>
		/// <param name="browser">The browser this element is associated with.</param>
		public InternetExplorerElement(IHTMLElement element, Browser browser)
		{
			_element = element;
			Browser = browser;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the browser this element is currently associated with.
		/// </summary>
		public Browser Browser { get; private set; }

		/// <summary>
		/// Gets the ID of the element.
		/// </summary>
		public string Id
		{
			get { return GetAttributeValue("id"); }
		}

		/// <summary>
		/// Gets the tag name of the element.
		/// </summary>
		public string TagName
		{
			get { return _element.tagName.ToLower(); }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Clicks the element.
		/// </summary>
		public void Click()
		{
			_element.click();
		}

		/// <summary>
		/// Fires an event on the element.
		/// </summary>
		/// <param name="eventName">The events name to fire.</param>
		/// <param name="eventProperties">The properties for the event.</param>
		public void FireEvent(string eventName, NameValueCollection eventProperties = null)
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
		public void Focus()
		{
			((IHTMLElement2) _element).focus();
		}

		/// <summary>
		/// Gets an attribute value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute to read.</param>
		/// <returns>The attribute value.</returns>
		public string GetAttributeValue(string name)
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
				value = "True";
			}

			return value;
		}

		/// <summary>
		/// Gets an attribute style value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute style to read.</param>
		/// <returns>The attribute style value.</returns>
		public string GetStyleAttributeValue(string name)
		{
			return _element.style.getAttribute(name);
		}

		/// <summary>
		/// Sets an attribute value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute to write.</param>
		/// <param name="value">The value to be written.</param>
		public void SetAttributeValue(string name, string value)
		{
			value = HandleAttributesWhichHaveNoValue(name, value);
			_element.setAttribute(name, value, 0);
		}

		/// <summary>
		/// Sets an attribute style value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute style to write.</param>
		/// <param name="value">The style value to be written.</param>
		public void SetStyleAttributeValue(string name, string value)
		{
			_element.style.setAttribute(name, value);
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

		#region Static Methods

		private static string HandleAttributesWhichHaveNoValue(string attributeName, string value)
		{
			// selected is attribute of Option
			// checked is attribute of RadioButton and CheckBox
			if (attributeName == "selected" || attributeName == "checked")
			{
				value = bool.Parse(value) ? "true" : "";
			}

			return value;
		}

		#endregion
	}
}
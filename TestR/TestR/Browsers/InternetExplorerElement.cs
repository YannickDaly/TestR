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
	public class InternetExplorerElement : IBrowserElement
	{
		#region Fields

		public static readonly Dictionary<string, string> PropertiesToRename = new Dictionary<string, string>
		{
			{ "class", "className" }
		};

		public static readonly IList<string> UsePropertyInsteadOfAttribute = new[]
		{
			"selected", "textContent", "className", "checked", "readOnly", "multiple", "value",
			"nodeType", "innerHTML", "baseURI", "src", "href", "rowIndex", "cellIndex"
		};

		private readonly IHTMLElement _element;

		#endregion

		#region Constructors

		public InternetExplorerElement(IHTMLElement element, Browser browser)
		{
			_element = element;
			Browser = browser;
		}

		#endregion

		#region Properties

		public Browser Browser { get; private set; }

		public string Id
		{
			get { return GetAttributeValue("id"); }
		}

		public string TagName
		{
			get { return _element.tagName.ToLower(); }
		}

		#endregion

		#region Methods

		public void Click()
		{
			_element.click();
		}

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

		public void Focus()
		{
			((IHTMLElement2) _element).focus();
		}

		public string GetAttributeValue(string attributeName)
		{
			attributeName = PropertiesToRename.ContainsKey(attributeName)
				? PropertiesToRename[attributeName] : attributeName;

			var attributeValue = UsePropertyInsteadOfAttribute.Contains(attributeName)
				? GetExpandoValue(attributeName)
				: _element.getAttribute(attributeName);

			if (attributeValue == null)
			{
				return string.Empty;
			}

			var value = attributeValue.ToString();
			if (attributeName.ToLower() == "selected" && value.ToLower() == "selected")
			{
				value = "True";
			}

			return value;
		}

		public string GetStyleAttributeValue(string attributeName)
		{
			return _element.style.getAttribute(attributeName);
		}

		public void SetAttributeValue(string attributeName, string value)
		{
			value = HandleAttributesWhichHaveNoValue(attributeName, value);
			_element.setAttribute(attributeName, value, 0);
		}

		public void SetStyleAttributeValue(string attributeName, string attributeValue)
		{
			_element.style.setAttribute(attributeName, attributeValue);
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
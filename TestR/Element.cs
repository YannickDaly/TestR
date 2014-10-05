#region References

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;
using TestR.Extensions;
using TestR.Logging;

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
		protected static readonly Dictionary<string, string> PropertiesToRename = new Dictionary<string, string>
		{
			{ "class", "className" }
		};

		private readonly Browser _browser;
		private readonly ElementCollection _collection;
		private readonly dynamic _element;
		private readonly string _highlightColor;
		private readonly string _orginalColor;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes an instance of a browser element.
		/// </summary>
		/// <param name="element">The browser element this is for.</param>
		/// <param name="browser">The browser this element is associated with.</param>
		/// <param name="collection">The collection this element is associated with.</param>
		public Element(JToken element, Browser browser, ElementCollection collection)
		{
			_element = element;
			_browser = browser;
			_collection = collection;
			_orginalColor = GetStyleAttributeValue("backgroundColor") ?? "";
			_highlightColor = "yellow";
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the access key attribute.
		/// </summary>
		/// <remarks>
		/// Specifies a shortcut key to activate/focus an element.
		/// </remarks>
		public string AccessKey
		{
			get { return this["accesskey"]; }
			set { this["accesskey"] = value; }
		}

		/// <summary>
		/// Gets the browser this element is currently associated with.
		/// </summary>
		public Browser Browser
		{
			get { return _browser; }
		}

		/// <summary>
		/// Gets the children for this element.
		/// </summary>
		public ElementCollection Children
		{
			get { return _collection.Where(x => x.ParentId == Id).ToElementCollection(); }
		}

		/// <summary>
		/// Gets the class attribute.
		/// </summary>
		/// <remarks>
		/// Specifies one or more classnames for an element (refers to a class in a style sheet).
		/// </remarks>
		public string Class
		{
			get { return this["class"]; }
			set { this["class"] = value; }
		}

		/// <summary>
		/// Gets or sets the dropzone attribute of the element.
		/// </summary>
		/// <remarks>
		/// HTML5: Specifies whether the dragged data is copied, moved, or linked, when dropped.
		/// </remarks>
		public string DropZone
		{
			get { return this["dropzone"]; }
			set { this["dropzone"] = value; }
		}

		/// <summary>
		/// Gets or sets the hidden attribute of the element.
		/// </summary>
		/// <remarks>
		/// HTML5: Specifies that an element is not yet, or is no longer, relevant.
		/// </remarks>
		public string Hidden
		{
			get { return this["hidden"]; }
			set { this["hidden"] = value; }
		}

		/// <summary>
		/// Gets the ID of the element.
		/// </summary>
		/// <remarks>
		/// Specifies a unique id for an element.
		/// </remarks>
		public string Id
		{
			get { return _element.id; }
		}

		/// <summary>
		/// Gets or sets an attribute or property by name.
		/// </summary>
		/// <param name="name">The name of the attribute or property to read.</param>
		public string this[string name]
		{
			get { return GetAttributeValue(name, true); }
			set { SetAttributeValue(name, value); }
		}

		/// <summary>
		/// Gets or sets the name of the element.
		/// </summary>
		public string Name
		{
			get { return _element.name; }
			set { _element.name = value; }
		}

		/// <summary>
		/// The parent element of this element. Returns null if there is no parent.
		/// </summary>
		public Element Parent
		{
			get
			{
				if (string.IsNullOrWhiteSpace(ParentId) || !_collection.ContainsKey(ParentId))
				{
					return null;
				}

				return _collection[ParentId];
			}
		}

		/// <summary>
		/// Gets the ID of the element's parent.
		/// </summary>
		public string ParentId
		{
			get { return _element.parentId; }
		}

		/// <summary>
		/// Gets the style of the element. 
		/// </summary>
		/// <remarks>
		/// Specifies an inline CSS style for an element
		/// </remarks>
		public string Style
		{
			get { return this["style"]; }
			set { this["style"] = value; }
		}

		/// <summary>
		/// Gets or sets the tab index of the element.
		/// </summary>
		/// <remarks>
		/// Specifies the tabbing order of an element
		/// </remarks>
		public string TabIndex
		{
			get { return this["tabindex"]; }
			set { this["tabindex"] = value; }
		}

		/// <summary>
		/// Gets the tag name of the element.
		/// </summary>
		public string TagName
		{
			get { return _element.tagName; }
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
			string value;

			if (refresh)
			{
				name = PropertiesToRename.ContainsKey(name) ? PropertiesToRename[name] : name;
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
			Trace.WriteLine(highlight ? "Adding highlight to element " + Id + "." : "Removing highlight from element " + Id + ".");
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

		/// <summary>
		/// Triggers the element via the Angular function "trigger".
		/// </summary>
		protected void TriggerElement()
		{
			if (Browser.JavascriptLibraries.Contains(JavaScriptLibrary.Angular)
				&& Browser.JavascriptLibraries.Contains(JavaScriptLibrary.JQuery))
			{
				Browser.ExecuteScript("angular.element('#" + Id + "').trigger('input');");
			}
		}

		/// <summary>
		/// Add or updates the cached attributes for this element.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
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

		/// <summary>
		/// Sets the element attribute value. If the attribute is not found we'll add it.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
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

			_element.attributes.Add(name);
			_element.attributes.Add(value);
		}

		#endregion
	}
}
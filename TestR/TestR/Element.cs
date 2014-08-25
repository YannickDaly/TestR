#region References

using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using NLog;

#endregion

namespace TestR
{
	/// <summary>
	/// Represents an HTML element.
	/// </summary>
	public class Element
	{
		#region Fields

		private readonly string _highlightColor;
		private readonly string _orginalColor;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the Element class.
		/// </summary>
		/// <param name="element"></param>
		public Element(BrowserElement element)
		{
			BrowserElement = element;
			_orginalColor = BrowserElement.GetStyleAttributeValue("backgroundColor") ?? "";
			_highlightColor = "yellow";
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the class attribute value for the element.
		/// </summary>
		public string Class
		{
			get { return GetAttributeValue("class"); }
		}

		/// <summary>
		/// Gets a value indicating whether this Element is enabled.
		/// </summary>
		/// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
		public bool Enabled
		{
			get
			{
				var value = GetAttributeValue("disabled");

				if (string.IsNullOrEmpty(value))
				{
					return true;
				}

				if (value.ToLowerInvariant() == "disabled")
				{
					return false;
				}

				return !bool.Parse(value);
			}
		}

		/// <summary>
		/// Gets the ID of the element.
		/// </summary>
		public string Id
		{
			get { return BrowserElement.Id; }
		}

		/// <summary>
		/// Gets the name of the element.
		/// </summary>
		public string Name
		{
			get { return BrowserElement.Name; }
		}

		/// <summary>
		/// Gets the tag name of the element. Ex. input, a, button, etc.
		/// </summary>
		public string TagName
		{
			get { return BrowserElement.TagName.ToLower(); }
		}

		/// <summary>
		/// Gets the text for the element.
		/// </summary>
		public string Text
		{
			get { return BrowserElement.Text; }
		}

		/// <summary>
		/// Gets the browser element this element is for.
		/// </summary>
		internal BrowserElement BrowserElement { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Fires the blur event on this element.
		/// </summary>
		public void Blur()
		{
			//FireEvent("onBlur");
		}

		/// <summary>
		/// Clicks the element.
		/// </summary>
		public void Click()
		{
			BrowserElement.Focus();
			BrowserElement.Click();
			BrowserElement.Browser.WaitForComplete();
		}

		/// <summary>
		/// Focuses on the element.
		/// </summary>
		public void Focus()
		{
			BrowserElement.Focus();
		}

		/// <summary>
		/// Gets an attribute value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute to read.</param>
		/// <returns>The attribute value.</returns>
		public string GetAttributeValue(string name)
		{
			return BrowserElement.GetAttributeValue(name);
		}

		/// <summary>
		/// Gets an attribute style value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute style to read.</param>
		/// <returns>The attribute style value.</returns>
		public string GetStyleAttributeValue(string name)
		{
			return BrowserElement.GetStyleAttributeValue(name);
		}

		/// <summary>
		/// Highlight or resets the element.
		/// </summary>
		/// <param name="highlight">If true the element is highlight yellow. If false the element is returned to its original color.</param>
		public void Highlight(bool highlight)
		{
			Logger.Write(highlight ? "Adding highlight to element " + Id + "." : "Removing highlight from element " + Id + ".", LogLevel.Trace);
			BrowserElement.SetStyleAttributeValue("background-color", highlight ? _highlightColor : _orginalColor);

			if (BrowserElement.Browser.SlowMotion && highlight)
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
			BrowserElement.SetAttributeValue(name, value);
		}

		/// <summary>
		/// Sets an attribute style value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute style to write.</param>
		/// <param name="value">The style value to be written.</param>
		public void SetStyleAttributeValue(string name, string value)
		{
			BrowserElement.SetStyleAttributeValue(name, value);
		}

		/// <summary>
		/// Type text into the element.
		/// </summary>
		/// <param name="value">The value to be typed.</param>
		public void TypeText(string value)
		{
			Focus();
			Highlight(true);
			BrowserElement.TypeText(value);
			Highlight(false);
			Blur();
			TriggerElement();
		}

		/// <summary>
		/// Triggers the element via the Angular function "trigger".
		/// </summary>
		private void TriggerElement()
		{
			if (BrowserElement.Browser.JavascriptLibraries.Contains(JavaScriptLibrary.Angular)
				&& BrowserElement.Browser.JavascriptLibraries.Contains(JavaScriptLibrary.JQuery))
			{
				BrowserElement.Browser.ExecuteJavascript("angular.element('#" + Id + "').trigger('input');");
			}
		}

		#endregion
	}
}
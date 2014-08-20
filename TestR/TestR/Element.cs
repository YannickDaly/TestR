#region References

using System;
using System.Collections.Specialized;
using System.Threading;

#endregion

namespace TestR
{
	public class Element
	{
		#region Fields

		private readonly string _highlightColor;
		private readonly string _orginalColor;

		#endregion

		#region Constructors

		public Element(IBrowserElement element)
		{
			BrowserElement = element;
			_orginalColor = BrowserElement.GetStyleAttributeValue("backgroundColor") ?? "";
			_highlightColor = "yellow";
			TypeTextDelay = new TimeSpan(0, 0, 0, 0, 1);
		}

		#endregion

		#region Properties

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

		public string Id
		{
			get { return BrowserElement.Id; }
		}

		public string TagName
		{
			get { return BrowserElement.TagName.ToLower(); }
		}

		public TimeSpan TypeTextDelay { get; set; }

		/// <summary>
		/// Gets the browser element this element is for.
		/// </summary>
		internal IBrowserElement BrowserElement { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Fires the blur event on this element.
		/// </summary>
		public void Blur()
		{
			FireEvent("onBlur");
		}

		public void Click()
		{
			BrowserElement.Focus();
			BrowserElement.Click();
			BrowserElement.Browser.WaitForComplete();
		}

		public void FireEvent(string eventName, NameValueCollection eventProperties = null)
		{
			if (!Enabled)
			{
				throw new Exception("This element is disabled so we cannot fire the event.");
			}

			BrowserElement.FireEvent(eventName, eventProperties);
			BrowserElement.Browser.WaitForComplete();
		}

		public void Focus()
		{
			BrowserElement.Focus();
		}

		public string GetAttributeValue(string attributeName)
		{
			return BrowserElement.GetAttributeValue(attributeName);
		}

		public string GetStyleAttributeValue(string attributeName)
		{
			return BrowserElement.GetStyleAttributeValue(attributeName);
		}

		public void Highlight(bool highlight)
		{
			BrowserElement.SetStyleAttributeValue("backgroundColor", highlight ? _highlightColor : _orginalColor);
		}

		public void SetAttributeValue(string attributeName, string value)
		{
			BrowserElement.SetAttributeValue(attributeName, value);
		}

		public void SetStyleAttributeValue(string attributeName, string value)
		{
			BrowserElement.SetStyleAttributeValue(attributeName, value);
		}

		public void TypeText(string value)
		{
			Focus();

			Highlight(true);

			foreach (var character in value)
			{
				var eventProperty = GetKeyCodeEventProperty(character);
				FireEvent("onKeyDown", eventProperty);
				FireEvent("onKeyPress", eventProperty);
				FireEvent("onKeyUp", eventProperty);

				var newValue = BrowserElement.GetAttributeValue("value") + character;
				BrowserElement.SetAttributeValue("value", newValue);

				Thread.Sleep(TypeTextDelay);
			}

			Highlight(false);

			Blur();
		}

		#endregion

		#region Static Methods

		private static NameValueCollection GetKeyCodeEventProperty(char character)
		{
			return new NameValueCollection { { "keyCode", ((int) character).ToString() }, { "charCode", ((int) character).ToString() } };
		}

		#endregion
	}
}
#region References

using System.Collections.Specialized;

#endregion

namespace TestR
{
	/// <summary>
	/// Represents an element for a browser.
	/// </summary>
	public interface IBrowserElement
	{
		#region Properties

		/// <summary>
		/// Gets the browser this element is currently associated with.
		/// </summary>
		Browser Browser { get; }

		/// <summary>
		/// Gets the ID of the element.
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Gets the tag name of the element.
		/// </summary>
		string TagName { get; }

		#endregion

		#region Methods

		/// <summary>
		/// Clicks the element.
		/// </summary>
		void Click();

		/// <summary>
		/// Fires an event on the element.
		/// </summary>
		/// <param name="eventName">The events name to fire.</param>
		/// <param name="eventProperties">The properties for the event.</param>
		void FireEvent(string eventName, NameValueCollection eventProperties);

		/// <summary>
		/// Focuses on the element.
		/// </summary>
		void Focus();

		/// <summary>
		/// Gets an attribute value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute to read.</param>
		/// <returns>The attribute value.</returns>
		string GetAttributeValue(string name);

		/// <summary>
		/// Gets an attribute style value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute style to read.</param>
		/// <returns>The attribute style value.</returns>
		string GetStyleAttributeValue(string name);

		/// <summary>
		/// Sets an attribute value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute to write.</param>
		/// <param name="value">The value to be written.</param>
		void SetAttributeValue(string name, string value);

		/// <summary>
		/// Sets an attribute style value by the provided name.
		/// </summary>
		/// <param name="name">The name of the attribute style to write.</param>
		/// <param name="value">The style value to be written.</param>
		void SetStyleAttributeValue(string name, string value);

		#endregion
	}
}
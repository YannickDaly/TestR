#region References

using System.Collections.Specialized;

#endregion

namespace TestR
{
	public interface IBrowserElement
	{
		#region Properties

		Browser Browser { get; }
		string Id { get; }
		string TagName { get; }

		#endregion

		#region Methods

		void Click();
		void FireEvent(string eventName, NameValueCollection eventProperties);
		void Focus();
		string GetAttributeValue(string attributeName);
		string GetStyleAttributeValue(string attributeName);
		void SetAttributeValue(string value, string newValue);
		void SetStyleAttributeValue(string attributeName, string attributeValue);

		#endregion
	}
}
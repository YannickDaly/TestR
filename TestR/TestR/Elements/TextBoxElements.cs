namespace TestR.Elements
{
	public class TextBoxElement : Element
	{
		#region Constructors

		public TextBoxElement(IBrowserElement element)
			: base(element)
		{
		}

		#endregion

		#region Properties

		public string Value
		{
			get { return GetAttributeValue("value"); }
			set { SetAttributeValue("value", value); }
		}

		#endregion
	}
}
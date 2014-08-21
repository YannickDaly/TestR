namespace TestR.Elements
{
	public class TextInputElement : Element
	{
		#region Constructors

		public TextInputElement(IBrowserElement element)
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
namespace TestR.Elements
{
	/// <summary>
	/// Represents an element of type Input or TextArea.
	/// </summary>
	public class TextInputElement : Element
	{
		#region Constructors

		/// <summary>
		/// Initializes an instance of the TextInputElement class.
		/// </summary>
		/// <param name="element">The browser element this class represents.</param>
		public TextInputElement(IBrowserElement element)
			: base(element)
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the value for the text input.
		/// </summary>
		public string Value
		{
			get { return GetAttributeValue("value"); }
			set { SetAttributeValue("value", value); }
		}

		#endregion
	}
}
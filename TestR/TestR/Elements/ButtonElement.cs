namespace TestR.Elements
{
	/// <summary>
	/// Represents an element of type Button.
	/// </summary>
	public class ButtonElement : Element
	{
		#region Constructors

		/// <summary>
		/// Initializes an instance of the ButtonElement class.
		/// </summary>
		/// <param name="element">The browser element this class represents.</param>
		public ButtonElement(IBrowserElement element)
			: base(element)
		{
		}

		#endregion
	}
}
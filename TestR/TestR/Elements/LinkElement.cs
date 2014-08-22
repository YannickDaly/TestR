#region References

#endregion

namespace TestR.Elements
{
	/// <summary>
	/// Represents an element of type Link.
	/// </summary>
	public class LinkElement : Element
	{
		#region Constructors

		/// <summary>
		/// Initializes an instance of the LinkElement class.
		/// </summary>
		/// <param name="element">The browser element this class represents.</param>
		public LinkElement(IBrowserElement element)
			: base(element)
		{
		}

		#endregion
	}
}
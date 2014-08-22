#region References

#endregion

namespace TestR.Elements
{
	/// <summary>
	/// Represents an element of type Span.
	/// </summary>
	public class SpanElement : Element
	{
		#region Constructors

		/// <summary>
		/// Initializes an instance of the SpanElement class.
		/// </summary>
		/// <param name="element">The browser element this class represents.</param>
		public SpanElement(IBrowserElement element)
			: base(element)
		{
		}

		#endregion
	}
}
namespace TestR
{
	/// <summary>
	/// The type of elements.
	/// </summary>
	public enum ElementType
	{
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown,

		/// <summary>
		/// Button (button, input[button]
		/// </summary>
		Button,

		/// <summary>
		/// Link (a)
		/// </summary>
		Link,

		/// <summary>
		/// Span (span)
		/// </summary>
		Span,

		/// <summary>
		/// Text Input (input[email, hidden, password, search, text], textarea)
		/// </summary>
		TextInput,
	}
}
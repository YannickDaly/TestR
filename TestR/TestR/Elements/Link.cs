#region References

using Newtonsoft.Json.Linq;

#endregion

namespace TestR.Elements
{
	/// <summary>
	/// Represent a browser link element.
	/// </summary>
	public class Link : Element
	{
		#region Constructors

		/// <summary>
		/// Initializes an instance of a Link browser element.
		/// </summary>
		/// <param name="element">The browser element this is for.</param>
		/// <param name="browser">The browser this element is associated with.</param>
		/// <param name="collection">The collection this element is associated with.</param>
		public Link(JToken element, Browser browser, ElementCollection collection)
			: base(element, browser, collection)
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or set the download attirbute of this link.
		/// </summary>
		/// <remarks>
		/// HTML5: Specifies that the target will be downloaded when a user clicks on the hyperlink
		/// </remarks>
		public string Download
		{
			get { return this["href"]; }
			set { this["href"] = value; }
		}

		/// <summary>
		/// Gets or set the HREF of this link.
		/// </summary>
		public string Href
		{
			get { return this["href"]; }
			set { this["href"] = value; }
		}

		/// <summary>
		/// Gets or set the target of this link.
		/// </summary>
		/// <remarks>
		/// Specifies where to open the linked document.
		/// </remarks>
		public string Target
		{
			get { return this["target"]; }
			set { this["target"] = value; }
		}

		#endregion
	}
}
﻿#region References

using Newtonsoft.Json.Linq;

#endregion

namespace TestR.Elements
{
	/// <summary>
	/// Represent a browser button element.
	/// </summary>
	public class Button : Element
	{
		#region Constructors

		/// <summary>
		/// Initializes an instance of a Button browser element.
		/// </summary>
		/// <param name="element">The browser element this is for.</param>
		/// <param name="browser">The browser this element is associated with.</param>
		/// <param name="collection">The collection this element is associated with.</param>
		public Button(JToken element, Browser browser, ElementCollection collection)
			: base(element, browser, collection)
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the autofocus attribute.
		/// </summary>
		/// <remarks>
		/// HTML5: Specifies that a button should automatically get focus when the page loads.
		/// </remarks>
		public string AutoFocus
		{
			get { return this["autofocus"]; }
			set { this["autofocus"] = value; }
		}

		/// <summary>
		/// Gets or sets the disabled attribute.
		/// </summary>
		/// <remarks>
		/// Specifies that a button should be disabled.
		/// </remarks>
		public string Disabled
		{
			get { return this["disabled"]; }
			set { this["disabled"] = value; }
		}

		/// <summary>
		/// Gets or sets the form attribute.
		/// </summary>
		/// <remarks>
		/// HTML5: Specifies one or more forms the button belongs to.
		/// </remarks>
		public string Form
		{
			get { return this["form"]; }
			set { this["form"] = value; }
		}

		/// <summary>
		/// Gets or sets the form action attribute.
		/// </summary>
		/// <remarks>
		/// HTML5: Specifies where to send the form-data when a form is submitted. Only for type="submit".
		/// </remarks>
		public string FormAction
		{
			get { return this["formaction"]; }
			set { this["formaction"] = value; }
		}

		/// <summary>
		/// Gets or sets the form encoded type attribute.
		/// </summary>
		/// <remarks>
		/// HTML5: Specifies how form-data should be encoded before sending it to a server. Only for type="submit".
		/// </remarks>
		public string FormEncType
		{
			get { return this["formenctype"]; }
			set { this["formenctype"] = value; }
		}

		/// <summary>
		/// Gets or sets the form method attribute.
		/// </summary>
		/// <remarks>
		/// HTML5: Specifies how to send the form-data (which HTTP method to use). Only for type="submit".
		/// </remarks>
		public string FormMethod
		{
			get { return this["formmethod"]; }
			set { this["formmethod"] = value; }
		}

		/// <summary>
		/// Gets or sets the form no validate attribute.
		/// </summary>
		/// <remarks>
		/// HTML5: Specifies that the form-data should not be validated on submission. Only for type="submit".
		/// </remarks>
		public string FormNoValidate
		{
			get { return this["formnovalidate"]; }
			set { this["formnovalidate"] = value; }
		}

		/// <summary>
		/// Gets or sets the form target attribute.
		/// </summary>
		/// <remarks>
		/// HTML5: Specifies where to display the response after submitting the form. Only for type="submit".
		/// </remarks>
		public string FormTarget
		{
			get { return this["formtarget"]; }
			set { this["formtarget"] = value; }
		}

		/// <summary>
		/// Gets or sets the value attribute.
		/// </summary>
		public string Text
		{
			get { return this["value"]; }
			set { this["value"] = value; }
		}

		/// <summary>
		/// Gets or sets the value attribute.
		/// </summary>
		public string Value
		{
			get { return this["value"]; }
			set { this["value"] = value; }
		}

		#endregion
	}
}
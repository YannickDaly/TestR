#region References

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using TestR.Elements;
using TestR.Extensions;

#endregion

namespace TestR
{
	/// <summary>
	/// Represents a collection of specific type of elements.
	/// </summary>
	/// <typeparam name="T">The type of element.</typeparam>
	public class ElementCollection<T> : Collection<T>
		where T : Element
	{
		#region Constructors

		/// <summary>
		/// Initializes an instance of the ElementCollection class.
		/// </summary>
		/// <param name="collection"></param>
		public ElementCollection(IEnumerable<T> collection)
		{
			this.AddRange(collection);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Access an element by the ID.
		/// </summary>
		/// <param name="id">The ID of the element.</param>
		public T this[string id]
		{
			get
			{
				var response = this.FirstOrDefault(x => x.Id == id);
				Assert.IsNotNull(response, "Failed to find the element by the index of " + id + ".");
				return response;
			}
		}

		#endregion
	}

	/// <summary>
	/// Represents a collection of elements.
	/// </summary>
	public class ElementCollection : Collection<Element>
	{
		#region Constructors

		/// <summary>
		/// Initializes an instance of the ElementCollection class.
		/// </summary>
		public ElementCollection()
		{
		}

		/// <summary>
		/// Initializes an instance of the ElementCollection class.
		/// </summary>
		/// <param name="collection">The collection of elements to add to this collection.</param>
		public ElementCollection(IEnumerable<Element> collection)
		{
			this.AddRange(collection);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a list of all button elements.
		/// </summary>
		public ElementCollection<Button> Buttons
		{
			get { return OfType<Button>(); }
		}

		/// <summary>
		/// Access an element by the ID.
		/// </summary>
		/// <param name="id">The ID of the element.</param>
		public Element this[string id]
		{
			get
			{
				var response = this.FirstOrDefault(x => x.Id == id);
				Assert.IsNotNull(response, "Failed to find the element by the index of " + id + ".");
				return response;
			}
		}

		/// <summary>
		/// Gets a list of all link elements.
		/// </summary>
		public ElementCollection<Link> Links
		{
			get { return OfType<Link>(); }
		}

		/// <summary>
		/// Gets a list of all span elements.
		/// </summary>
		public ElementCollection<Span> Spans
		{
			get { return OfType<Span>(); }
		}

		/// <summary>
		/// Gets a list of all text input elements.
		/// </summary>
		public ElementCollection<TextInput> TextInputs
		{
			get { return OfType<TextInput>(); }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds a collection of elements and initializes them as their specific element type.
		/// </summary>
		/// <param name="token">The collection of elements to add.</param>
		/// <param name="browser"></param>
		public void Add(JToken token, Browser browser)
		{
			var element = new Element(token, browser, this);
			switch (element.TagName)
			{
				case "button":
					Add(new Button(token, browser, this));
					return;

				case "input":
					var type = element.GetAttributeValue("type").ToLower();
					switch (type)
					{
						case "button":
						case "submit":
						case "reset":
							Add(new Button(token, browser, this));
							return;

						case "email":
						case "hidden":
						case "password":
						case "search":
						case "text":
							Add(new TextInput(token, browser, this));
							return;

						default:
							Add(element);
							return;
					}

				case "span":
					Add(new Span(token, browser, this));
					return;

				case "textarea":
					Add(new TextInput(token, browser, this));
					return;

				case "a":
					Add(new Link(token, browser, this));
					return;

				default:
					Add(element);
					return;
			}
		}

		/// <summary>
		/// Adds an JArray collection of elements to this collection.
		/// </summary>
		/// <param name="collection">The collection of elements.</param>
		/// <param name="browser">The browser the element belong to.</param>
		public void AddRange(JArray collection, Browser browser)
		{
			collection.ForEach(x => Add(x, browser));
		}

		/// <summary>
		/// Checks to see if the collection contains the provided ID.
		/// </summary>
		/// <param name="id">The ID to check for.</param>
		/// <returns>True if the key is found or false if otherwise.</returns>
		public bool ContainsKey(string id)
		{
			return this.Any(x => x.Id == id);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public ElementCollection<T> OfType<T>() where T : Element
		{
			return this.Where(x => x.GetType() == typeof (T)).Cast<T>().ToElementCollection<T>();
		}

		#endregion
	}
}
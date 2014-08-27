#region References

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json.Linq;
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
		/// Access an element by the ID or Name.
		/// </summary>
		/// <param name="idOrName">The ID or Name of the element.</param>
		public T this[string idOrName]
		{
			get { return this.FirstOrDefault(x => x.Id == idOrName || x.Name == idOrName); }
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
		/// <param name="collection"></param>
		public ElementCollection(IEnumerable<Element> collection)
		{
			AddRange(collection);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a list of all button elements.
		/// </summary>
		public ElementCollection Buttons
		{
			get { return this.Where(x => x.ElementType == ElementType.Button).ToElementCollection(); }
		}

		/// <summary>
		/// Access an element by the ID or Name.
		/// </summary>
		/// <param name="idOrName">The ID or Name of the element.</param>
		public Element this[string idOrName]
		{
			get { return this.FirstOrDefault(x => x.Id == idOrName || x.Name == idOrName); }
		}

		/// <summary>
		/// Gets a list of all link elements.
		/// </summary>
		public ElementCollection Links
		{
			get { return this.Where(x => x.ElementType == ElementType.Link).ToElementCollection(); }
		}

		/// <summary>
		/// Gets a list of all span elements.
		/// </summary>
		public ElementCollection Spans
		{
			get { return this.Where(x => x.ElementType == ElementType.Span).ToElementCollection(); }
		}

		/// <summary>
		/// Gets a list of all text input elements.
		/// </summary>
		public ElementCollection TextInputs
		{
			get { return this.Where(x => x.ElementType == ElementType.TextInput).ToElementCollection(); }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Adds a collection of elements and initializes them as their specific element type.
		/// </summary>
		/// <param name="collection">The collection of elements to add.</param>
		public void AddRange(IEnumerable<Element> collection)
		{
			foreach (var item in collection)
			{
				switch (item.TagName.ToLower())
				{
					case "button":
						item.ElementType = ElementType.Button;
						break;

					case "input":
						var type = item.GetAttributeValue("type").ToLower();
						switch (type)
						{
							case "button":
								item.ElementType = ElementType.Button;
								break;

							case "email":
							case "hidden":
							case "password":
							case "search":
							case "text":
								item.ElementType = ElementType.TextInput;
								break;

							default:
								item.ElementType = ElementType.Unknown;
								break;
						}
						break;

					case "span":
						item.ElementType = ElementType.Span;
						break;

					case "textarea":
						item.ElementType = ElementType.TextInput;
						break;

					case "a":
						item.ElementType = ElementType.Link;
						break;

					default:
						item.ElementType = ElementType.Unknown;
						break;
				}

				Add(item);
			}
		}

		/// <summary>
		/// Adds an JArray collection of elements to this collection.
		/// </summary>
		/// <param name="collection">The collection of elements.</param>
		/// <param name="browser">The browser the element belong to.</param>
		public void AddRange(JArray collection, Browser browser)
		{
			AddRange(collection.Select(x => new Element(x, browser)));
		}

		#endregion
	}
}
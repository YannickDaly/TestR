#region References

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TestR.Elements;
using TestR.Extensions;

#endregion

namespace TestR.Collections
{
	public class ElementCollection<T> : Collection<T>
		where T : Element
	{
		#region Constructors

		public ElementCollection(IEnumerable<T> collection)
		{
			this.AddRange(collection);
		}

		#endregion

		#region Properties

		public T this[string id]
		{
			get { return this.FirstOrDefault(x => x.Id == id); }
		}

		#endregion
	}

	public class ElementCollection : Collection<Element>
	{
		#region Constructors

		public ElementCollection(IEnumerable<Element> collection)
		{
			AddRange(collection);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a list of all button elements.
		/// </summary>
		public ElementCollection<ButtonElement> Buttons
		{
			get { return this.OfType<ButtonElement>().ToElementCollection(); }
		}

		public Element this[string id]
		{
			get { return this.FirstOrDefault(x => x.Id == id); }
		}

		/// <summary>
		/// Gets a list of all link elements.
		/// </summary>
		public ElementCollection<LinkElement> Links
		{
			get { return this.OfType<LinkElement>().ToElementCollection(); }
		}

		/// <summary>
		/// Gets a list of all span elements.
		/// </summary>
		public ElementCollection<SpanElement> Spans
		{
			get { return this.OfType<SpanElement>().ToElementCollection(); }
		}

		/// <summary>
		/// Gets a list of all text input elements.
		/// </summary>
		public ElementCollection<TextInputElement> TextInputs
		{
			get { return this.OfType<TextInputElement>().ToElementCollection(); }
		}

		#endregion

		#region Methods

		private void AddRange(IEnumerable<Element> collection)
		{
			foreach (var item in collection)
			{
				switch (item.TagName.ToLower())
				{
					case "button":
						Add(new ButtonElement(item.BrowserElement));
						continue;

					case "input":
						var type = item.GetAttributeValue("type").ToLower();
						switch (type)
						{
							case "button":
								Add(new ButtonElement(item.BrowserElement));
								continue;

							case "email":
							case "hidden":
							case "password":
							case "search":
							case "text":
								Add(new TextInputElement(item.BrowserElement));
								continue;

							default:
								Add(item);
								continue;
						}

					case "span":
						Add(new SpanElement(item.BrowserElement));
						break;

					case "textarea":
						Add(new TextInputElement(item.BrowserElement));
						continue;

					case "a":
						Add(new LinkElement(item.BrowserElement));
						continue;

					default:
						Add(item);
						continue;
				}
			}
		}

		#endregion
	}
}
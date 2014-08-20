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

		public Element this[string id]
		{
			get { return this.FirstOrDefault(x => x.Id == id); }
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
								Add(new TextBoxElement(item.BrowserElement));
								continue;

							default:
								Add(item);
								continue;
						}

					case "textarea":
						Add(new TextBoxElement(item.BrowserElement));
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
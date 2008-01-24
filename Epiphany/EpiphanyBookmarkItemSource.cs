using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using Do.Addins;
using Do.Universe;

namespace Epiphany
{

	public class EpiphanyBookmarkItemSource : IItemSource
	{
		List<IItem> items;

		public EpiphanyBookmarkItemSource ()
		{
			items = new List<IItem> ();
		}

		public string Name { get { return "Epiphany Bookmarks"; } }
		public string Description { get { return "Indexes your Epiphany bookmarks."; } }
		public string Icon { get { return "gnome-web-browser"; } }

		public Type[] SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (BookmarkItem),
				};
			}
		}

		public ICollection<IItem> Items
		{
			get { return items; }
		}

		public ICollection<IItem> ChildrenOfItem (IItem parent)
		{
			return null;	
		}

		public void UpdateItems ()
		{
			string home = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			string bookmarks_file = "~/.gnome2/epiphany/bookmarks.rdf".Replace ("~", home);


			items.Clear ();
			try {
				using (XmlReader reader = XmlReader.Create (bookmarks_file)) {
					while (reader.ReadToFollowing ("item")) {
						string title, link;
						
						reader.ReadToFollowing ("title");
						title = reader.ReadString ();
						reader.ReadToFollowing ("link");
						link = reader.ReadString ();

						items.Add (new BookmarkItem (title, link));
					}
				}
			} catch (Exception e) {
				Console.Error.WriteLine ("Could not read Epiphany Bookmarks file {0}: {1}",
						bookmarks_file, e.Message);
			}
		}

	}
}


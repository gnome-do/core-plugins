using System;
using System.IO;
using System.Collections.Generic;

using Mono.Unix;

using Do.Platform;
using Do.Universe;
using Do.Universe.Common;

namespace Opera 
{
	public class OperaBookmarkItemSource : ItemSource 
	{
		
		List<Item> items;
		
		public OperaBookmarkItemSource()
		{
			items = new List<Item> ();
		}
		
		public override string Name { 
			get { return Catalog.GetString ("Opera Bookmarks"); } 
		}

		public override string Description { 
			get { return Catalog.GetString ("Indexes your Opera 6 bookmarks"); } 
		}

		public override string Icon { 
			get { return "opera"; } 
		}
		
		public override  IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (BookmarkItem); }
		}
		public override IEnumerable<Item> Items	{
			get { return items; }
		}
		
		public override void UpdateItems ()
		{
			string home = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			string path = "~/.opera/opera6.adr".Replace ("~", home);
			
			items.Clear();
			try {
				using (StreamReader streamReader = new StreamReader (path)) {
					string strName;
					string strURL;
					while((strName = streamReader.ReadLine ()) != null) {
						if (!strName.Contains ("NAME")) continue;

						strURL = streamReader.ReadLine ();

						if (string.IsNullOrEmpty (strURL) || !strURL.Contains ("URL")) continue;

						strName = strName.Replace ("NAME=", "");
						strURL = strURL.Replace ("URL=", "");
						items.Add (new BookmarkItem (strName, strURL));						
					}
				}
			}
			catch (Exception e) {
				Log.Error ("Could not read Opera Bookmarks file {0}: {1}", path, e.Message);
				Log.Debug (e.StackTrace);
			}
		}
	}
}


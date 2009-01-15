//  PidginContactItemSource.cs
//
//  GNOME Do is the legal property of its developers, whose names are too numerous
//  to list here.  Please refer to the COPYRIGHT file distributed with this
//  source distribution.
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections.Generic;

using Mono.Unix;

using Do.Universe;
using Do.Platform;

namespace PidginPlugin
{

	public class PidginContactItemSource : ItemSource
	{

		static readonly string BuddyListFile;
		static readonly string BuddyIconDirectory;
		
		static PidginContactItemSource ()
		{
			string home = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			BuddyListFile = Path.Combine (home, ".purple/blist.xml");
			BuddyIconDirectory = Path.Combine (home, ".purple/icons");
		}
		
		List<Item> buddies;
		
		public PidginContactItemSource ()
		{
			buddies = new List<Item> ();
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ContactItem); }
		}
		
		public override string Name {
			get { return Catalog.GetString ("Pidgin Buddies"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Buddies on your Pidgin buddy list."); } 
		}
		
		public override string Icon {
			get { return "pidgin"; }
		}
		
		public override IEnumerable<Item> Items {
			get { return buddies; }
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			ContactItem buddy = item as ContactItem;
			return buddy.Details
				.Where (d => d.StartsWith ("prpl-"))
				.Select (d => new PidginHandleContactDetailItem (d, buddy [d]))
				.Cast<Item> ();
		}
		
		public override void UpdateItems ()
		{
			XmlDocument blist;
			// Add buddies as they are encountered to this hash so we don't create duplicates.
			Dictionary<ContactItem, bool> buddies_seen;
			
			buddies.Clear ();
			buddies_seen = new Dictionary<ContactItem, bool> ();
			blist = new XmlDocument ();
			try {
				blist.Load (BuddyListFile);

				foreach (XmlNode contact_node in blist.GetElementsByTagName ("contact"))
					foreach (XmlNode buddy_node in contact_node.ChildNodes) {
						ContactItem buddy;		
						
						buddy = ContactItemFromBuddyXmlNode (buddy_node);
						if (buddy == null) continue;
						buddies_seen[buddy] = true;
					}
				
			} catch (Exception e) {
				Log.Error ("Could not read Pidgin buddy list file: {0}", e.Message);
				Log.Debug (e.StackTrace);
			}
			foreach (ContactItem buddy in buddies_seen.Keys) {
				buddies.Add (buddy);
			}
		}
		
		ContactItem ContactItemFromBuddyXmlNode (XmlNode buddy_node)
		{
			ContactItem buddy;
			string proto, name, alias, icon;
			
			try {
				name = alias = icon = null;
				// The messaging protocol (e.g. "prpl-jabber" for Jabber).
				proto = buddy_node.Attributes.GetNamedItem ("proto").Value;
				foreach (XmlNode attr in buddy_node.ChildNodes) {
					switch (attr.Name) {
					// The screen name.
					case "name":
						name = attr.InnerText;
						break;
					// The alias, or real name.
					case "alias":
						alias = attr.InnerText;
						break;
					// Buddy icon image file.
					case "setting":
						if (attr.Attributes.GetNamedItem ("name").Value == "buddy_icon") {
							icon = Path.Combine (BuddyIconDirectory, attr.InnerText);
						}
						break;
					}
				}
			} catch {
				// Bad buddy.
				return null;
			}
			// If crucial details are missing, we can't make a buddy.
			if (name == null || proto == null) return null;
			
			// Create a new buddy, add the details we have.
			buddy = ContactItem.Create (alias ?? name);
			if (icon != null)
				buddy["photo"] = icon;
			buddy[proto] = name;

			return buddy;
		}
	}
}

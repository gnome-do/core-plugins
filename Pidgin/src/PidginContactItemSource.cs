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

		const string iconPrefix = "icon-";
		static readonly string BuddyListFile;
		static readonly string BuddyIconDirectory;
		
		static PidginContactItemSource ()
		{
			string home = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			string[] blist_parts = new string[] {home, ".purple", "blist.xml"};
		 	BuddyListFile = blist_parts.Aggregate ((aggregation, val) => Path.Combine (aggregation, val));
			string[] icondir_parts = new string[] {home, ".purple", "icons"};
			BuddyIconDirectory = icondir_parts.Aggregate ((aggregation, val) => Path.Combine (aggregation, val));
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
		
		private PidginHandleContactDetailItem MakeChildren (ContactItem buddy, string proto, IEnumerable<string> icons)
		{
			return (icons.Contains (iconPrefix+proto)) 
				? new PidginHandleContactDetailItem (proto, buddy[proto], buddy[iconPrefix+proto])
				: new PidginHandleContactDetailItem (proto, buddy[proto]);
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			ContactItem buddy = item as ContactItem;
			
			IEnumerable<string> icons = buddy.Details.Where (d => d.StartsWith (iconPrefix+"prpl-"));

			return buddy.Details
						.Where (d => d.StartsWith ("prpl-")) 
						.Select (d => MakeChildren (buddy, d, icons))
					    .OfType<Item> ();
		}
		
		public override void UpdateItems ()
		{
			XmlDocument blist;
			// Add buddies as they are encountered to this hash so we don't create duplicates.
			Dictionary<ContactItem, bool> buddies_seen;
			
			//remove pidgin related keys from the buddies
			foreach (ContactItem buddy in buddies) {
				foreach (string key in buddy.Details.Where (d => d.Contains ("prpl")).ToArray ())
					buddy[key] = "";
			}

			
			buddies.Clear ();
			buddies_seen = new Dictionary<ContactItem, bool> ();
			//load buddies from xml file
			blist = new XmlDocument ();
			try {
				blist.Load (BuddyListFile);

				foreach (XmlNode contact_node in blist.GetElementsByTagName ("contact")) {
					ContactItem buddy;
					
					buddy = CreateBuddy (contact_node);
					if (buddy == null) continue;
					buddies_seen[buddy] = true;
				}

			} catch (Exception e) {
				Log<PidginContactItemSource>.Error ("Error reading Pidgin buddy list file: {0}", e.Message);
				Log<PidginContactItemSource>.Debug (e.StackTrace);
			}
			//if Pidgin is running, find bonjour buddies from dbus
			//link-local (bonjour) buddies do not get added to the xml file
			try {
				int bonjourAccount = Pidgin.GetAccountID ("","prpl-bonjour");
				if (bonjourAccount != -1) {
					foreach (int buddyID in Pidgin.FindBuddies (bonjourAccount, "")) {
						ContactItem buddy;
						
						//Console.WriteLine ("Bonjour buddy: {0}.", buddyID);
						//Console.WriteLine ("Alias for {0}: {1}", buddyID, Pidgin.GetBuddyServerAlias(buddyID));
						//Console.WriteLine ("Icon for {0}: {1}", buddyID, Pidgin.GetBuddyIconPath (buddyID));
						buddy = CreateBuddy (buddyID);
						buddies_seen [buddy] = true;
					}			
				}
					/*
					proto = prpl.PurpleAccountGetProtocolName (account);
					name = prpl.PurpleAccountGetUsername (account);
					items.Add (new PidginAccountItem (name, proto, account));
					*/
			} catch (Exception e) { 
				Log<PidginContactItemSource>.Error ("Could not get Pidgin accounts: {0}", e.Message);
				Log<PidginContactItemSource>.Debug (e.StackTrace);
			}
			foreach (ContactItem buddy in buddies_seen.Keys) {
				buddies.Add (buddy);
			}
		}
		
		//We're only using this to get link-local (bonjour) buddies
		ContactItem CreateBuddy (int buddyID)
		{
			ContactItem buddy;
			string alias, proto, icon, accountAlias;
			
			alias = Pidgin.GetBuddyServerAlias(buddyID);
			accountAlias = Pidgin.GetBuddyLocalAlias (buddyID);
			icon = Pidgin.GetBuddyIconPath (buddyID);
			proto = "prpl-bonjour";
			buddy = ContactItem.Create (alias);
			
			//if for some reason this buddy has multiple prpl-bonjour accounts associated with it
			//make sure we add them all in this fashion: prpl-bonjour, prpl-bonjour-1, etc.
			int similarProtos = buddy.Details.Where (k => k.StartsWith (proto)).Count ();
			if (similarProtos > 0)
				proto = string.Format ("{0}-{1}", proto, similarProtos.ToString ());

			buddy[proto] = accountAlias;
			if (!string.IsNullOrEmpty (icon))
				buddy[iconPrefix+proto] = icon;
			if (string.IsNullOrEmpty (buddy["photo"]))
			    buddy["photo"] = icon;

			return buddy;
		}
		
		
		ContactItem CreateBuddy(XmlNode buddyNode)
		{
			ContactItem buddy;
			string name, alias, proto;
			
			Dictionary<string, string> icons = new Dictionary<string, string> ();
			Dictionary<string, string> protos = new Dictionary<string, string> ();

			alias = name = null;
			
			//we favor aliases in this order: metacontact alias, local alias, server alias
			//metacontact alias
			try {
				alias = buddyNode.Attributes.GetNamedItem ("alias").Value;
			}
			catch {}
			
			foreach (XmlNode node in buddyNode.ChildNodes)
			{
				switch (node.Name) {
				case "buddy":
					proto = node.Attributes.GetNamedItem ("proto").Value;
					//for metacontacts, add similar protocol keys like this:
					// prpl-msn, prpl-msn-1, prpl-msn-2 etc.
					int similarProtos = protos.Keys.Where (k => k.StartsWith (proto)).Count ();
					if (similarProtos > 0)
						proto = string.Format ("{0}-{1}", proto, similarProtos.ToString ());
					foreach (XmlNode attr in node.ChildNodes) {
						switch (attr.Name) {
						// The screen name.
						case "name":
							protos[proto] = attr.InnerText;
							break;
						// The alias, or real name, only if one isn't set yet.
						case "alias":
							if (string.IsNullOrEmpty (alias))
							    alias = attr.InnerText;
							break;
						// Buddy icon image file.
						case "setting":
							if (attr.Attributes.GetNamedItem ("name").Value == "buddy_icon") {
								icons[iconPrefix+proto] = Path.Combine (BuddyIconDirectory, attr.InnerText);
								if (!icons.Keys.Contains ("default"))
									icons["default"] = icons[iconPrefix+proto];
							}
							break;
						}
					}
					//if the alias is still null, let's try to get the server alias
					if (string.IsNullOrEmpty (alias))
					    alias = Pidgin.GetBuddyServerAlias (protos[proto]) ?? null;
					break;
				//let's pick up the custom icon as the metacontact's icon
				case "setting":
					if (node.Attributes.GetNamedItem ("name").Value == "custom_buddy_icon") {
						icons["default"] = Path.Combine (BuddyIconDirectory, node.InnerText);
					}
					break;
				}
			}
			
			//in case we don't have an alias, take one of the proto values for the name
			name = alias ?? protos.Values.FirstOrDefault ();

			// If crucial details are missing, we can't make a buddy.
			if (name == null || protos.Values.Count () <= 0) return null;
			
			// Create a new buddy, add the details we have.
			buddy = ContactItem.Create (alias ?? name);
			
			//assign the default buddy icon as the ContactItem's photo
			if (icons.Keys.Contains ("default"))
				buddy["photo"] = icons["default"];

			//add all of the protocol handles we found for this buddy
			//skip bonjour keys, they'll get picked up later
			foreach (string k in protos.Keys.Where (k => !k.Contains ("bonjour")))
				buddy[k] = protos[k];
			
			//add the icons keys to create individual icons for childitems
			foreach (string k in icons.Keys.Where (k => k != "default" && !k.Contains ("bonjour")))
				buddy[k] = icons[k];

			return buddy;
		}
	}
}
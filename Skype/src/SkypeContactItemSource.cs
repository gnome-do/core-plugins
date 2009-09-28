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
using System.Linq;
using System.Collections.Generic;

using Mono.Addins;

using Do.Platform;
using Do.Universe;
using Do.Universe.Common;

namespace Skype
{

	public class SkypeContactItemSource : ItemSource
	{
		
		List<Item> contacts;
		
		public SkypeContactItemSource ()
		{
			contacts = new List<Item> ();
		}

		
		public override IEnumerable<Type> SupportedItemTypes {
			get { 
				yield return typeof (ContactItem); 
				yield return typeof (IApplicationItem);
				yield return typeof (SkypeBrowseBuddyItem);
			}
		}
		
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Skype Buddies"); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Buddies on your Skype buddy list."); } 
		}
		
		public override string Icon {
			get { return "skype"; }
		}
		
		public override IEnumerable<Item> Items {
			get { return contacts; }
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			if (Skype.IsSkype (item)) {
				yield return new SkypeBrowseBuddyItem ();
			} else if (item is SkypeBrowseBuddyItem) {
				foreach (ContactItem contact in contacts)
					yield return contact;
			} else if (item is ContactItem) {
				ContactItem contact = item as ContactItem;
				
				foreach (string detail in contact.Details) {
					if (!detail.EndsWith (".skype"))
						continue;
					
					if (detail.StartsWith ("handle"))
						yield return new SkypeContactDetailItem (contact, contact ["handle.skype"]);
					else if (detail.StartsWith ("phone"))
						yield return new PhoneContactDetailItem (contact, detail);
				}
			}
			yield break;
		}
		
		public override void UpdateItems ()
		{
			foreach (ContactItem buddy in contacts) {
				foreach (string key in buddy.Details.Where (d => d.Contains ("skype")).ToArray ())
					buddy[key] = "";
			}
			
			contacts.Clear ();
			
			foreach (string handle in Skype.ContactHandles) {
				ContactItem contact = CreateContact (handle);
				if (contact != null) {
					contacts.Add (contact);
				}
			}
		}
		
		private ContactItem CreateContact (string handle)
		{
			string contactName;
			
			contactName = (string.IsNullOrEmpty (Skype.ContactDisplayName (handle))) ? 
				(string.IsNullOrEmpty (Skype.ContactFullName (handle))) ? 
					handle : Skype.ContactFullName (handle) :
				Skype.ContactDisplayName (handle);

			ContactItem contact = ContactItem.Create (contactName);
			
			MaybeAddDetail (contact, "handle", handle);
		
			MaybeAddDetail (contact, "phone.home", Skype.ContactHomePhone (handle));
			MaybeAddDetail (contact, "phone.mobile",  Skype.ContactMobilePhone (handle));
			MaybeAddDetail (contact, "phone.work", Skype.ContactOfficePhone (handle));
			
			return contact;
		}
		
		//ContactItem extension method for Skype to conditionally add details
		private void MaybeAddDetail (ContactItem contact, string key, string detail)
		{
			if (!string.IsNullOrEmpty (detail))
				contact [key + ".skype"] = detail;
		}
	}
}
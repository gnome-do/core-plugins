//  ContactItemSource.cs
//
//  GNOME Do is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this
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
using System.Collections.Generic;

using Do.Universe;

namespace Evolution
{
	public class ContactItemSource : IItemSource
	{
		List<IItem> contacts;
		List<string> pictureFiles;
		
		public ContactItemSource ()
		{
			contacts = new List<IItem> ();
			pictureFiles = new List<string> ();
			try {
				_UpdateItems ();
			} catch { }
		}
		
		public Type[] SupportedItemTypes {
			get {
				return new Type[] {
					typeof (ContactItem),
				};
			}
		}
		
		public string Name { get { return "Evolution Contacts"; } }
		public string Description { get { return "Evolution Contacts"; } }
		public string Icon { get { return "evolution"; } }
		
		public void UpdateItems ()
		{
			/* // This leaks.
			try {
				_UpdateItems ();
			} catch (Exception e) {
				Console.Error.WriteLine ("Cannot index Evolution contacts: {0}",
					e.Message);
			}
			*/
		}
		
		public ICollection<IItem> Items {
			get { return contacts; }
		}
		
		public ICollection<IItem> ChildrenOfItem (IItem item)
		{
			return null;
		}
		
		void _UpdateItems ()
		{
			SourceList sources;
		
			contacts.Clear ();
			// Clear the temporary contact picture files.
			foreach (string file in pictureFiles) {
				try {
					File.Delete (file);
				} catch { }
			}
			pictureFiles.Clear ();
			
			using (sources = new SourceList ("/apps/evolution/addressbook/sources")) {
				foreach (SourceGroup group in sources.Groups) {
					foreach (Source source in group.Sources) {
						Book address_book;
						Contact[] e_contacts;
						ContactItem contact;
						
						// Only index local address books
						if (!source.IsLocal ()) continue;
						
						using (address_book = new Book (source)) {
							address_book.Open (true);
							e_contacts = address_book.GetContacts (BookQuery.AnyFieldContains (""));
							foreach (Contact e_contact in e_contacts) {
								try {
									contact = CreateEvolutionContactItem (e_contact);
								} catch {
									// bad contact
									continue;
								}
								contacts.Add (contact);
							}
						} // End using address_book.
					} // End foreach Source
				} // End foreach SourceGroup
			} // End using sources
		}
	
		ContactItem CreateEvolutionContactItem (Contact e_contact) {
			ContactItem contact;
						
			contact = ContactItem.Create (e_contact.FullName);
			
			if (e_contact.Email1 != null && e_contact.Email1 != "")
				contact["email"] = e_contact.Email1;
			if (e_contact.Email2 != null && e_contact.Email2 != "")
				contact["email2"] = e_contact.Email2;
			if (e_contact.Email3 != null && e_contact.Email3 != "")
				contact["email3"] = e_contact.Email3;
			
			for (int i = 0; i < e_contact.ImAim.Length; ++i)
				contact["aim" + (i>0?i.ToString ():"")] = e_contact.ImAim[i];
			for (int i = 0; i < e_contact.ImJabber.Length; ++i)
				contact["jabber" + (i>0?i.ToString ():"")] = e_contact.ImJabber[i];
			
			// Been getting some excpetions from g_boxed_copy
			// when I attempt to read contact photos...
			try {
				switch (e_contact.Photo.PhotoType) {
					case ContactPhotoType.Inlined:
						string tmp = Path.GetTempFileName ();
						string photo = tmp  + ".jpg";
						try {
							File.Delete (tmp);
						} catch { }
						try {
							File.WriteAllBytes (photo, e_contact.Photo.Data);
							pictureFiles.Add (photo);
							contact["photo"] = photo;
						} catch { }
						break;
					case ContactPhotoType.Uri:
						if (File.Exists (e_contact.Photo.Uri)) {
							contact["photo"] = e_contact.Photo.Uri;
						}
						break;
				}
			} catch (Exception e) {
				Console.Error.WriteLine (e.StackTrace);
				Console.Error.WriteLine ("Error while loading evolution photo for contact {0}: {1}",
					contact["name"], e.Message);
		   	}
			return contact;
		}
	}
}

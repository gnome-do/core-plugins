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

using Do;
using Do.Universe;

namespace Evolution
{
	public class ContactItemSource : IItemSource
	{
		List<IItem> contacts;
		
		public ContactItemSource ()
		{
			contacts = new List<IItem> ();
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
		
		public ICollection<IItem> Items {
			get { return contacts; }
		}
		
		public ICollection<IItem> ChildrenOfItem (IItem item)
		{
			List<IItem> details = new List<IItem> ();
			ContactItem contact = item as ContactItem;
			foreach (string detail in contact.Details) {
				if (detail.StartsWith ("email") &&
					detail.EndsWith (".evolution"))
					details.Add (new EmailContactDetailItem (contact, detail));
			}
			return details;
		}
		
		public void UpdateItems ()
		{
			// Disallow updating due to memory leak.
			if (contacts.Count > 0) return;
			using (SourceList sources =
				new SourceList ("/apps/evolution/addressbook/sources")) {
				foreach (SourceGroup group in sources.Groups) {
					foreach (Source source in group.Sources) {
						// Only index local address books
						if (!source.IsLocal ()) continue;
						using (Book book = new Book (source)) {
							book.Open (true);
							foreach (Contact c in book.GetContacts (
								BookQuery.AnyFieldContains (""))) {
								try {
									contacts.Add (
										CreateEvolutionContactItem (c));
								} catch { /* bad contact */ }
							}
						}
					}
				}
			}
		}
	
		ContactItem CreateEvolutionContactItem (Contact e_contact) {
			ContactItem contact;
						
			contact = ContactItem.Create (e_contact.FullName);
			
			if (e_contact.Email1 != null && e_contact.Email1 != "")
				contact["email.evolution"] = e_contact.Email1;
			if (e_contact.Email2 != null && e_contact.Email2 != "")
				contact["email2.evolution"] = e_contact.Email2;
			if (e_contact.Email3 != null && e_contact.Email3 != "")
				contact["email3.evolution"] = e_contact.Email3;
			
			for (int i = 0; i < e_contact.ImAim.Length; ++i)
				contact["aim" + (i>0?i.ToString ():"") + ".evolution"] =
					e_contact.ImAim[i];
			for (int i = 0; i < e_contact.ImJabber.Length; ++i)
				contact["jabber" + (i>0?i.ToString ():"") + ".evolution"] =
					e_contact.ImJabber[i];
			
			// Been getting some exceptions from g_boxed_copy
			// when I attempt to read contact photos...
			if (string.IsNullOrEmpty (contact ["photo.evolution"])) try {
				switch (e_contact.Photo.PhotoType) {
					case ContactPhotoType.Inlined:
						string photo = Paths.GetTemporaryFilePath () + ".jpg";
						try {
							File.WriteAllBytes (photo, e_contact.Photo.Data);
							contact["photo"] = contact["photo.evolution"] =
								photo;
						} catch { }
						break;
					case ContactPhotoType.Uri:
						if (File.Exists (e_contact.Photo.Uri)) {
							contact["photo"] = contact["photo.evolution"] =
								e_contact.Photo.Uri;
						}
						break;
				}
			} catch (Exception e) {
				Console.Error.WriteLine (
					"Error while loading evolution photo for contact {0}: {1}",
					contact["name"], e.Message);
				Console.Error.WriteLine (e.StackTrace);
		   	}
			return contact;
		}
	}
}

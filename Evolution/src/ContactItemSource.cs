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

using Mono.Unix;

namespace Evolution
{
	public struct ContactAttribute
	{
		string detail, key;
		public ContactAttribute(string k, string d)
		{
			this.key = k;
			this.detail = d;
		}
		public string Detail { get { return detail; } }
		public string Key { get { return key; } }
	}
	
	public class ContactItemSource : ItemSource
	{
		List<Item> contacts;
		
		public ContactItemSource ()
		{
			contacts = new List<Item> ();
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] {
					typeof (ContactItem),
				};
			}
		}
		
		public override string Name { get { return Catalog.GetString ("Evolution Contacts"); } }
		public override string Description { get { return Catalog.GetString ("Evolution Contacts"); } }
		public override string Icon { get { return "evolution"; } }
		
		public override IEnumerable<Item> Items {
			get { return contacts; }
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			List<Item> details = new List<Item> ();
			ContactItem contact = item as ContactItem;
			foreach (string detail in contact.Details) {
				if (!detail.EndsWith (".evolution")) continue;
				
				if (detail.StartsWith ("email"))
					details.Add (new EmailContactDetailItem (contact, detail));
				else if (detail.StartsWith ("phone"))
					details.Add (new PhoneContactDetailItem (contact, detail));
				else if (detail.StartsWith ("url.home"))
					details.Add (new BookmarkItem ("Homepage", contact [detail]));
				else if (detail.StartsWith ("url.blog"))
					details.Add (new BookmarkItem ("Blog", contact [detail]));
				else if (detail.StartsWith("address"))
					details.Add (new AddressContactDetailItem (contact, detail));
			}
			return details;
		}
		
		public override void UpdateItems ()
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
								} catch (Exception e) {
									/* bad contact */ 
									Console.WriteLine(e.Message.ToString());
								}
							}
						}
					}
				}
			}
		}
	
		ContactItem CreateEvolutionContactItem (Contact eContact) {
			ContactItem contact;
			if (!(string.IsNullOrEmpty(eContact.FullName)))
				contact = ContactItem.Create(eContact.FullName);
			else //(!(string.IsNullOrEmpty(eContact.FileAs)))
				contact = ContactItem.Create(eContact.FileAs);
			List<ContactAttribute> ContactDetails = new List<ContactAttribute>();
			ContactDetails.Add(new ContactAttribute("email.home", eContact.Email1));
			ContactDetails.Add(new ContactAttribute("email.work", eContact.Email2));
			ContactDetails.Add(new ContactAttribute("email.other", eContact.Email3));
			ContactDetails.Add(new ContactAttribute("phone.mobile", eContact.MobilePhone));
			ContactDetails.Add(new ContactAttribute("phone.home", eContact.HomePhone));
			ContactDetails.Add(new ContactAttribute("phone.home2",eContact.HomePhone2));
			ContactDetails.Add(new ContactAttribute("phone.work", eContact.CompanyPhone));
			ContactDetails.Add(new ContactAttribute("phone.work2",eContact.BusinessPhone));
			ContactDetails.Add(new ContactAttribute("phone.work3",eContact.BusinessPhone2));
			ContactDetails.Add(new ContactAttribute("phone", eContact.PrimaryPhone));                                                                      
			ContactDetails.Add(new ContactAttribute("url.home", eContact.HomepageUrl));
			ContactDetails.Add(new ContactAttribute("url.blog", eContact.BlogUrl));
			//for address, need to take out the \n characters, otherwise do is not happy
			if (!string.IsNullOrEmpty(eContact.AddressLabelHome))
				ContactDetails.Add(new ContactAttribute("address.home",eContact.AddressLabelHome.Replace('\n',' ')));
			if (!string.IsNullOrEmpty(eContact.AddressLabelWork))
				ContactDetails.Add(new ContactAttribute("address.work",eContact.AddressLabelWork.Replace('\n',' ')));
			if (!string.IsNullOrEmpty(eContact.AddressLabelOther))
				ContactDetails.Add(new ContactAttribute("address.other",eContact.AddressLabelOther.Replace('\n',' ')));
			
			// Been getting some exceptions from g_boxed_copy
			// when I attempt to read contact photos...
			if (string.IsNullOrEmpty (contact ["photo.evolution"])) try {
				switch (eContact.Photo.PhotoType) {
					case ContactPhotoType.Inlined:
						string photo = Paths.GetTemporaryFilePath () + ".jpg";
						try {
							File.WriteAllBytes (photo, eContact.Photo.Data);
							contact["photo"] = contact["photo.evolution"] =
								photo;
						} catch { }
						break;
					case ContactPhotoType.Uri:
						if (File.Exists (eContact.Photo.Uri)) {
							contact["photo"] = contact["photo.evolution"] =
								eContact.Photo.Uri;
						}
						break;
				}
			} catch (Exception e) {
				Console.Error.WriteLine (
					"Error while loading evolution photo for contact {0}: {1}",
					contact["name"], e.Message);
				Console.Error.WriteLine (e.StackTrace);
		   	}
			//add the details to the contact
			foreach (ContactAttribute c in ContactDetails)
			{
				MaybeAddDetail(contact,c.Key,c.Detail);
			}
			return contact;
		}
	
		private void MaybeAddDetail (ContactItem contact, string key, string detail)
		{
			if (!string.IsNullOrEmpty (detail))
				contact [key + ".evolution"] = detail;
		}
	}
}

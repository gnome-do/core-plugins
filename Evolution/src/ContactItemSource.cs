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
using System.Linq;
using System.Collections.Generic;

using Do.Universe;
using Do.Universe.Common;
using Do.Platform;

using Mono.Addins;

namespace Evolution
{
	public struct ContactAttribute
	{
		public string Detail { get; private set; }
		public string Key { get; private set; }
		
		public ContactAttribute (string key, string detail) : this ()
		{
			Key = key;
			Detail = detail;
		}
	}
	
	public class ContactItemSource : ItemSource
	{
		IEnumerable<ContactItem> contacts;
		
		public ContactItemSource ()
		{
			contacts = Enumerable.Empty<ContactItem> ();
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ContactItem); }
		}
		
		public override string Name { get { return AddinManager.CurrentLocalizer.GetString ("Evolution Contacts"); } }
		public override string Description { get { return AddinManager.CurrentLocalizer.GetString ("Evolution Contacts"); } }
		public override string Icon { get { return "evolution"; } }
		
		public override IEnumerable<Item> Items {
			get { return contacts.OfType<Item> (); }
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			ContactItem contact = item as ContactItem;
			foreach (string detail in contact.Details) {
				if (!detail.EndsWith (".evolution")) continue;
				
				if (detail.StartsWith ("email"))
					yield return new EmailContactDetailItem (contact, detail);
				else if (detail.StartsWith ("phone"))
					yield return new PhoneContactDetailItem (contact, detail);
				else if (detail.StartsWith ("url.home"))
					yield return new BookmarkItem ("Homepage", contact [detail]);
				else if (detail.StartsWith ("url.blog"))
					yield return new BookmarkItem ("Blog", contact [detail]);
				else if (detail.StartsWith("address"))
					yield return new AddressContactDetailItem (contact, detail);
			}
		}
		
		public override void UpdateItems ()
		{
			// Disallow updating due to memory leak.
			if (contacts.Any ()) return;
			
			using (SourceList sources = new SourceList ("/apps/evolution/addressbook/sources"))
				contacts = GetContactItems (sources).ToArray ();
		}

		IEnumerable<ContactItem> GetContactItems (SourceList sources)
		{
			foreach (SourceGroup group in sources.Groups)
				foreach (ContactItem item in GetContactItems (group))
					yield return item;
		}
		
		IEnumerable<ContactItem> GetContactItems (SourceGroup group)
		{
			foreach (Source source in group.Sources)
				foreach (ContactItem item in GetContactItems (source))
					yield return item;
		}
		
		IEnumerable<ContactItem> GetContactItems (Source source)
		{
			if (!source.IsLocal ())
				return Enumerable.Empty<ContactItem> ();
			
			using (Book book = new Book (source))
				return GetContactItems (book);
		}

		IEnumerable<ContactItem> GetContactItems (Book book)
		{
			foreach (Contact contact in book.GetContacts (BookQuery.AnyFieldContains (""))) {
				ContactItem item;
				try {
					item = GetContactItem (contact);
				} catch (Exception e) {
					Log.Error ("Error reading Evolution contact: {0}", e.Message);
					Log.Debug (e.StackTrace);
					continue;
				}
				yield return item;
			}
		}
	
		ContactItem GetContactItem (Contact eContact) {
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
						string photo = Services.Paths.GetTemporaryFilePath () + ".jpg";
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
			// add the details to the contact
			foreach (ContactAttribute c in ContactDetails)
				MaybeAddDetail(contact,c.Key,c.Detail);
			return contact;
		}
	
		private void MaybeAddDetail (ContactItem contact, string key, string detail)
		{
			if (!string.IsNullOrEmpty (detail))
				contact [key + ".evolution"] = detail;
		}
	}
}

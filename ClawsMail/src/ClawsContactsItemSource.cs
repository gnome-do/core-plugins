// ClawsContactsItemSource.cs created with MonoDevelop
// User: Karol Będkowski at 20:03 2008-10-09

// Copyright Karol Będkowski 2008

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using Do.Universe;
using Do.Platform;
using Mono.Unix;


namespace Claws
{

	/// <summary>
	/// Items source for Claws Mail - indexes Claws contacts saved in ~/.claws/addrbook-*.xml. 
	/// </summary>
	public class ClawsContactsItemSource: ItemSource {
		
		List<Item> items;
		
		public ClawsContactsItemSource () {
			items = new List<Item> ();
		}
		
		public override string Name {
			get { return Catalog.GetString ("Claws Mail contacts"); }
		}

		public override string Description {
			get { return Catalog.GetString ("Contacts from ClawsMail address book"); }
		}

		public override string Icon {
			get { return "claws-mail";}
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ContactItem); }
		}

		public override IEnumerable<Item> Items {
			get { return items;}
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			ContactItem contact = item as ContactItem;
			foreach (string detail in contact.Details) {
				if (detail.Contains (".claws")) {
					yield return new ClawsContactDetailItem (detail, contact[detail]);
				} else if (detail.Contains (".email")) { // other emails?	
					yield return new ClawsContactDetailItem (detail, contact[detail]);
				}
			}
		}

		public override void UpdateItems () 
		{
			items.Clear ();

			// get list of address book files 
			foreach (string book in Claws.GetAddressBookFiles ()) {				
				try {
					// read adress book
					using (StreamReader reader = new StreamReader (book)) {

						XmlDocument xmldoc = new XmlDocument ();
						xmldoc.Load (reader);
						XmlNode addresBookNode = xmldoc.SelectSingleNode ("address-book");
						XmlNodeList people = addresBookNode.SelectNodes ("person");
						
						foreach (XmlNode person in people) {						
							// contact name from "cn" attribute
							string personCn = person.Attributes["cn"].InnerText;
							if (string.IsNullOrEmpty (personCn)) {
								continue;
							}
							
							// load emails
							XmlNode addressListNode = person.SelectSingleNode ("address-list");
							XmlNodeList addresses = addressListNode.SelectNodes ("address");
							
							if (addresses.Count == 0) { // no childs = no emails
								continue;
							}

							ContactItem buddy = ContactItem.CreateWithName (personCn);
							int emailCounter = 0;						
							
							foreach (XmlNode address in addresses) {
								string email = address.Attributes["email"].InnerText;
								if (!String.IsNullOrEmpty (email)) {
									string id;
									if (emailCounter == 0) { // first email
										id = "email.claws";
									} else { // next email
										id = "email.claws." + emailCounter; 
									}									
									buddy[id] = email;
									emailCounter++;
								}
							}
							
							if (emailCounter > 0) {
								items.Add (buddy);
							}
						}
					}					

				} catch (Exception e) {
					Log.Error ("ClawsContactsItemSource: file:{1} error:{0}", e.Message, book);
					Log.Debug ("ClawsContactsItemSource: file:{0}: {1}", book, e.StackTrace);
				}
			}
		}


	}
}

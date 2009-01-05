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
using Mono.Unix;
using Do.Universe;
using Do.Platform;


namespace Claws
{

	/// <summary>
	/// Items source for Claws Mail - indexes Claws contacts saved in ~/.claws/addrbook-*.xml. 
	/// </summary>
	public class ClawsContactsItemSource: ItemSource {

		const string ClawsHome = ".claws-mail/addrbook";
		const string ClawsAddrBookIndex = "addrbook--index.xml";

		readonly static string ClawsHomePath;
		readonly static string ClawsIndexFilename;
		
		List<Item> items;

		#region std properties
		
		public override string Name {
			get { return Catalog.GetString ("ClawsMail contacts"); }
		}

		public override string Description {
			get { return Catalog.GetString ("Contacts in ClawsMail address book"); }
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

		#endregion


		static ClawsContactsItemSource ()
		{
			ClawsHomePath =  Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), 
				                                 ClawsHome);
			ClawsIndexFilename = Path.Combine (ClawsHomePath, ClawsAddrBookIndex);
		}
		
		
		public ClawsContactsItemSource () 
		{
			items = new List<Item> ();
		}
		
		
		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			ContactItem contact = item as ContactItem;
			foreach (string detail in contact.Details) {
				if (detail.Contains (".claws")) {
					yield return new ClawsContactDetailItem (detail, contact[detail]);
				}
			}
		}

		
		public override void UpdateItems () 
		{
			items.Clear ();

			// iterate over address book files 
			foreach (string addressBook in AddressBookFiles) {				
				try {
					// read adress book by StreamReader - direct == error:Encoding name '...' not supported
					using (StreamReader reader = new StreamReader (addressBook)) {

						XmlDocument xmldoc = new XmlDocument ();
						xmldoc.Load (reader);
						XmlNode addresBookNode = xmldoc.SelectSingleNode ("address-book");
						XmlNodeList people = addresBookNode.SelectNodes ("person");
						
						foreach (XmlNode person in people) {						
							// contact name from "cn" attribute
							string personCn = person.Attributes ["cn"].InnerText;
							if (string.IsNullOrEmpty (personCn)) {
								continue;
							}
							
							// load emails
							XmlNode addressListNode = person.SelectSingleNode ("address-list");
							XmlNodeList addresses = addressListNode.SelectNodes ("address");
							
							if (addresses.Count == 0) { // no childs == no emails -> skip
								continue;
							}

							ContactItem buddy = ContactItem.CreateWithName (personCn);
							int emailCounter = 0;						
							
							foreach (XmlNode address in addresses) {
								string email = address.Attributes ["email"].InnerText;
								if (!string.IsNullOrEmpty (email)) {
									string remarks = address.Attributes ["remarks"].InnerText;
									string id = "email.claws." + emailCounter + "." + remarks;
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
					Log.Error ("ClawsContactsItemSource: file:{1} error:{0}", e.Message, addressBook);
					Log.Debug ("ClawsContactsItemSource: file:{0}: {1}", addressBook, e.StackTrace);
				}
			}
		}


		/// <summary>
		/// Get address book files from Claws config.
		/// </summary>
		/// <returns>
		/// List of file names.<see cref="List`1"/>
		/// </returns>
		private static List<string> AddressBookFiles {
			get {
				List<string> result = new List<string> ();
	
				if (!File.Exists (ClawsIndexFilename)) {
					return result;
				}
				
				try {
					// open $HOME/.claws-mail/addrbook--index.xml
					XmlDocument xmldoc = new XmlDocument ();
					xmldoc.Load (ClawsIndexFilename);
					XmlNode adressbook = xmldoc.SelectSingleNode ("addressbook");
					XmlNode booklist = adressbook.SelectSingleNode ("book_list");
					XmlNodeList books = booklist.SelectNodes ("book");				
					foreach (XmlNode book in books) {
						string fileName = book.Attributes ["file"].InnerText;
						if (String.IsNullOrEmpty (fileName)) {
							continue;
						}
	
						string filePath = Path.Combine (ClawsHomePath, fileName);
						if (File.Exists (filePath)) {
							result.Add (filePath);
						}
					}				
				} catch (Exception e) {
					Log.Error("ClawsContactsItemSource.AddressBookFiles error: {0}", e.Message);
					Log.Debug("ClawsContactsItemSource.AddressBookFiles: {0}", e.StackTrace);
				}
				
				return result;
			}
		}

	}
}

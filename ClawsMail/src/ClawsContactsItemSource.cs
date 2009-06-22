/* ClawsContactsItemSource.cs 
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */


using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using Mono.Addins;
using Do.Universe;
using Do.Platform;


namespace Claws
{

	/// <summary>
	/// Items source for Claws Mail - indexes Claws contacts saved in ~/.claws/addrbook/addrbook-*.xml. 
	/// </summary>
	public class ClawsContactsItemSource: ItemSource {

		readonly static string ClawsHome = ".claws-mail/addrbook";
		readonly static string ClawsAddrBookIndex = "addrbook--index.xml";
		
		readonly static string ClawsKeyPrefix = "email.claws.";
		public readonly static string ClawsPrimaryEmailPrefix = ClawsKeyPrefix + "0.";

		readonly static string ClawsHomePath;
		readonly static string ClawsIndexFilename;
		
		List<Item> items;

		#region std properties
		
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("ClawsMail contacts"); }
		}

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Contacts in ClawsMail address book"); }
		}

		public override string Icon {
			get { return "claws-mail"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ContactItem); }
		}

		public override IEnumerable<Item> Items {
			get { return items; }
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
				if (detail.StartsWith (ClawsKeyPrefix)) {
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
					XmlDocument xmldoc = new XmlDocument ();
					// read adress book by StreamReader. Without: error:Encoding name '...' not supported
					using (StreamReader reader = new StreamReader (addressBook)) {
						xmldoc.Load (reader);
					}
					
					XmlNodeList people = xmldoc.SelectNodes ("/address-book/person");
					if (people == null) {
						continue;
					}
					
					foreach (XmlNode person in people) {						
						// contact name from "cn" attribute
						string personCn = person.Attributes ["cn"].InnerText;
						if (string.IsNullOrEmpty (personCn)) {
							continue;
						}
						
						// load emails
						XmlNodeList addresses = person.SelectNodes ("address-list/address");						
						if ((addresses == null) || (addresses.Count == 0)) { // no childs == no emails -> skip
							continue;
						}

						ContactItem buddy = ContactItem.CreateWithName (personCn);
						int emailCounter = 0;						
						
						foreach (XmlNode address in addresses) {
							string email = address.Attributes ["email"].InnerText;
							if (!string.IsNullOrEmpty (email)) {
								string remarks = address.Attributes ["remarks"].InnerText;
								string id = ClawsKeyPrefix + emailCounter + "." + remarks;
								buddy [id] = email;
								emailCounter++;
							}
						}
						
						if (emailCounter > 0) {
							items.Add (buddy);
						}
					}			

				} catch (Exception e) {
					Log.Error ("ClawsContactsItemSource: file:{0} error:{1}", addressBook, e.Message);
					Log.Debug ("ClawsContactsItemSource: file:{0}: {1}", addressBook, e.ToString ());
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
					XmlNodeList books = xmldoc.SelectNodes ("/addressbook/book_list/book");
					if (books == null) {
						return result;
					}
					
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
					Log.Debug("ClawsContactsItemSource.AddressBookFiles: {0}", e.ToString ());
				}
				
				return result;
			}
		}

	}
}

// GContact.cs
// 
// Copyright (C) 2008 [name of author]
//
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
//

using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using Gnome.Keyring;

using Google.GData.Contacts;
using Google.GData.Client;
using Google.GData.Extensions;

using Do.Universe;

namespace GContacts
{
	public sealed class GContact
	{
		private static ContactsService service;
		private static List<IItem> contacts;
		private static string username, password;
		private static DateTime lastUpdated;
		
		static GContact()
		{
			contacts = new List<IItem> ();
			SetCredentials ();
			lastUpdated = new DateTime (1979, 1, 1);
			try {
				GContact.Connect (username, password);
			} catch (Exception e) {
				Console.Error.WriteLine (e.Message);
			}
		}
		
		public static List<IItem> Contacts {
			get { return contacts; }
		}
		
		public static void Connect (string username, string password) 
		{
			try {
				service = new ContactsService ("alexLauni-gnomeDoGCalPlugin-1");
				service.setUserCredentials (username, password);
			} catch (Exception e) {
				Console.Error.WriteLine (e.Message);
			}
		}
		
		public static void UpdateContacts () 
		{
			if (!Monitor.TryEnter (contacts)) return;
			ContactsQuery query = new ContactsQuery(ContactsQuery.CreateContactsUri ("default"));
			query.NumberToRetrieve = 1000;
			Console.Error.WriteLine (lastUpdated);
			query.StartDate = lastUpdated;
			
			try {
				ContactsFeed feed = service.Query(query);
				lastUpdated = feed.Updated;
				
				ContactItem buddy;
				foreach (ContactEntry entry in feed.Entries)
				{
					if (!String.IsNullOrEmpty (entry.Title.Text)) {
						Console.Error.WriteLine (entry.Title.Text);
						buddy = ContactItem.CreateWithName (entry.Title.Text);					
						foreach (PostalAddress postal in entry.PostalAddresses)
							buddy["address"] = postal.Value.Replace('\n', ' ');
						foreach (PhoneNumber phone in entry.Phonenumbers)
							buddy["phone"] = phone.Value.Replace('\n', ' ');
						int i = 0;
						foreach (EMail email in entry.Emails)
						{
							
							if (email.Primary) {
								buddy ["email"] = email.Address;
							}
							else {
								buddy ["email." + i] = email.Address;
								i++;
							}
						}
						if (entry.Deleted)
							contacts.Remove (buddy);
						else
							contacts.Add (buddy);
					}
				}
			} catch (Exception e) {
				Console.Error.WriteLine (e.Message);
			} finally {
				Monitor.Exit (contacts);
			}
		}
				
		private static void SetCredentials ()
		{
			string keyring_item_name = "Google Account";
			Hashtable request_attributes = new Hashtable();
			request_attributes ["name"] = keyring_item_name;
			try {
				foreach(ItemData result in Ring.Find(ItemType.GenericSecret, request_attributes)) {
					if(!result.Attributes.ContainsKey("name") || !result.Attributes.ContainsKey("username") ||
						(result.Attributes ["name"] as string) != keyring_item_name) 
						continue;
					
					username = (result.Attributes ["username"] as string);
					password = result.Secret;
					//Console.Error.WriteLine ("{0} : {1}",username,password);
					if (username == null || username == String.Empty || password == null || password == String.Empty)
						throw new ApplicationException ("Invalid username/password in keyring");
				}
			} catch (Exception e) {
				Console.Error.WriteLine (e.Message);
				string account_info = Environment.GetEnvironmentVariable ("HOME")
					+ "/.config/gnome-do/google-name";
				try {
					StreamReader reader = File.OpenText(account_info);
					username = reader.ReadLine ();
					password = reader.ReadLine ();
					WriteAccount ();
				} catch { 
					Console.Error.WriteLine ("[ERROR] " + account_info + " cannot be read!");
				} 
			}
		}
		
		private static void WriteAccount ()
		{
			string keyring_item_name = "Google Account";
			string keyring;
			try {
				keyring = Ring.GetDefaultKeyring();
			} catch {
				username = null;
				password = null;
				return;
			}
			Hashtable update_request_attributes = new Hashtable();
			update_request_attributes ["name"] = keyring_item_name;
			update_request_attributes ["username"] = username;

			try {
				Ring.CreateItem(keyring, ItemType.GenericSecret, keyring_item_name,
				                update_request_attributes, password, true);
			} catch (Exception e) {
				Console.WriteLine(e.Message);
			}
		}
	}
}

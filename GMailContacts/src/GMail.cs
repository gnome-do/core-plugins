/*
 * GMail.cs
 * 
 * GNOME Do is the legal property of its developers, whose names are too numerous
 * to list here.  Please refer to the COPYRIGHT file distributed with this
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
using System.Net;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using Gnome.Keyring;
using Do.Universe;

using Google.GData.Client;
using Google.GData.Contacts;
using Google.GData.Extensions;

namespace GMailContacts
{
	public static class GMail
	{
		private static List<IItem> contacts;
		private static DateTime lastUpdated;
		private static ContactsService service;
		
		static GMail()
		{
			contacts = new List<IItem> ();
			lastUpdated = new DateTime ();
			service = new ContactsService ("alexLauni-gnomeDoGMailPlugin-1");
			string username, password;
			GetAccountDataFromKeyring (out username, out password);
			try {
				service.setUserCredentials (username, password);
			} catch { }
		}
		
		public static List<IItem> Contacts {
			get { return contacts; }
		}
		
		public static void UpdateContacts ()
		{
			if (!Monitor.TryEnter (contacts)) return;
			ContactsQuery query = new ContactsQuery(ContactsQuery.CreateContactsUri ("default"));
			query.NumberToRetrieve = 1000;
			query.StartDate = lastUpdated;
			query.ShowDeleted = true;
			
			try {
				ContactsFeed feed = service.Query(query);
				lastUpdated = feed.Updated;
				
				ContactItem buddy;
				foreach (ContactEntry entry in feed.Entries)
				{
					if (String.IsNullOrEmpty (entry.Title.Text)) continue;
					/*
					if (entry.Deleted) {
						ContactItem enemy = ContactItem.Create (entry.Title.Text);
						Console.Error.WriteLine("Has {0} been deleted? {1}",entry.Title.Text,
						                        contacts.Contains (enemy));
						contacts.Remove (enemy);
						continue;
					}
					*/
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
					contacts.Add (buddy);
				}
			} catch (Exception e) {
				Console.Error.WriteLine ("GMailContacts Error: {0}",e.Message);
			} finally {
				Monitor.Exit (contacts);
			}
		}
		
		private static string GetBuddyPhotoFileLocation (string url)
		{
			string buddyIconDir;
			buddyIconDir = "~/.config/gnome-do/plugins/GCalendar/icons";
			buddyIconDir.Replace ("~", Environment.GetEnvironmentVariable("HOME"));
			
			//WebClient client = new WebClient ();
			//Client.DownloadFile(url, buddyIconDir + );
			return null;
		}
		
		private static void GetAccountDataFromKeyring (out string username, out string password)
		{
			string keyring_item_name = "Google Account";
			Hashtable request_attributes = new Hashtable();
			request_attributes ["name"] = keyring_item_name;
			
			username = password = "";
			try {
				foreach(ItemData result in Ring.Find(ItemType.GenericSecret, request_attributes)) {
					if(!result.Attributes.ContainsKey("name") || !result.Attributes.ContainsKey("username") ||
						(result.Attributes ["name"] as string) != keyring_item_name) 
						continue;
					
					username = (result.Attributes ["username"] as string);
					password = result.Secret;
					if (username == null || username == String.Empty || password == null || password == String.Empty)
						throw new ApplicationException ("Invalid username/password in keyring");
				}
			} catch (Exception e) {
				Console.Error.WriteLine ("GMailContacts Error: {0}",e.Message);
				string account_info = Environment.GetEnvironmentVariable ("HOME")
					+ "/.config/gnome-do/google-name";
				try {
					StreamReader reader = File.OpenText(account_info);
					username = reader.ReadLine ();
					password = reader.ReadLine ();
					WriteAccount (username, password,keyring_item_name);
				} catch { 
					Console.Error.WriteLine ("Could not find file! ~/.config/gnome-do/google-name");
				} 
			}
		}
		
		private static void WriteAccount(string username, string password,
		                                 string keyringItemName)
		{
			string keyring;
			keyring = Ring.GetDefaultKeyring();
			Hashtable update_request_attributes = new Hashtable();
			update_request_attributes ["name"] = keyringItemName;
			update_request_attributes ["username"] = username;

			try {
				Ring.CreateItem(keyring, ItemType.GenericSecret, keyringItemName,
				                update_request_attributes, password, true);
			} catch (Exception e) {
				Console.WriteLine("GMailContacts Error: {0}",e.Message);
			}
		}
	}
}

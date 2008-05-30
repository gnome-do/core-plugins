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
		private static string gAppName = "alexLauni-gnomeDoGMailPlugin-1";
		private static List<IItem> contacts;
		private static DateTime lastUpdated;
		private static ContactsService service;
		
		static GMail()
		{
			System.Net.ServicePointManager.CertificatePolicy = new CertHandler ();
			contacts = new List<IItem> ();
			lastUpdated = new DateTime ();
			string username, password;
			GetUserAndPassFromKeyring (out username, out password, gAppName);
			Connect (username, password);
		}
		
		public static List<IItem> Contacts {
			get { return contacts; }
		}
		
		public static string GAppName {
			get { return gAppName; }
		}
		
		private static void Connect (string username, string password) 
		{
			try {
				service = new ContactsService (GAppName);
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
		
		public static void GetUserAndPassFromKeyring (out string username, out string password,
		                                              string keyringItemName)
		{
			username = password = "";
			Hashtable ht = new Hashtable ();
			ht ["name"] = keyringItemName;
			
			try {
				foreach (ItemData s in Ring.Find (ItemType.GenericSecret, ht)) {
					if (s.Attributes.ContainsKey ("name") && s.Attributes.ContainsKey ("username")
					    && (s.Attributes ["name"] as string).Equals (keyringItemName)) {
						username = s.Attributes ["username"] as string;
						password = s.Secret;
						return;
					}
				}
			} catch (Exception e) {
				Console.Error.WriteLine ("No account info stored for {0}",
				                         keyringItemName);
			}
		}
				
		public static void WriteAccountToKeyring (string username, string password,
		                                   string keyringItemName)
		{
			string oldUsername, oldPassword, keyring;
			
			try {
				keyring = Ring.GetDefaultKeyring ();
				Hashtable ht = new Hashtable ();
				ht["name"] = keyringItemName;
				ht["username"] = username;
				
				GetUserAndPassFromKeyring (out oldUsername, out oldPassword,
				                           keyringItemName);
				
				Ring.CreateItem (keyring, ItemType.GenericSecret, keyringItemName,
				                 ht, password, true);
			} catch (Exception e) {
				Console.Error.WriteLine (e);
			}
		}
		
		public static bool TryConnect (string username, string password)
		{
			ContactsService test;
			ContactsQuery query; 
			
			test = new ContactsService (GAppName);
			test.setUserCredentials (username, password);
			query = new ContactsQuery(ContactsQuery.CreateContactsUri ("default"));
			
			try {
				test.Query (query);
				Connect (username, password);
			} catch (Exception e) {
				Console.Error.WriteLine (e.Message);
				return false;
			}
			return true;
		}
	}
}

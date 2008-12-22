/* GMail.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this source distribution.
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
using System.Collections.Generic;

using Do.Universe;

using Google.GData.Client;
using Google.GData.Contacts;
using Google.GData.Extensions;

namespace GMailContacts
{
	public static class GMail
	{
		public const string GAppName = "alexLauni-gnomeDoGMailPlugin-1.2";
		private static List<Item> contacts;
		private static object conts_lock;
		private static DateTime last_updated;
		private static ContactsService service;
		
		static GMail()
		{
			ServicePointManager.CertificatePolicy = new CertHandler ();
			contacts = new List<Item> ();
			conts_lock = new object ();
			last_updated = new DateTime (1987, 11, 28);
			
			Connect ( GMailConfig.username, GMailConfig.password);
		}
		
		public static List<Item> Contacts {
			get { return contacts; }
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
			if (!Monitor.TryEnter (conts_lock)) return;
			
			ContactsQuery query = new ContactsQuery (
				ContactsQuery.CreateContactsUri ("default"));
			query.NumberToRetrieve = 1000;
			query.StartDate = last_updated;
			query.ShowDeleted = true;
			
			try {
				ContactsFeed feed = service.Query(query);
				last_updated = feed.Updated;
				
				ContactItem buddy;
				foreach (ContactEntry entry in feed.Entries)
				{
					if (String.IsNullOrEmpty (entry.Title.Text)) continue;
					
					if (entry.Deleted) {
						ContactItem enemy = ContactItem.Create
							(entry.Title.Text);
						contacts.Remove (enemy);
						continue;
					}
					
					buddy = ContactItem.CreateWithName (entry.Title.Text);
					int i = 0;
					foreach (PostalAddress address in entry.PostalAddresses) {
						string detail = "address.gmail." + i;
						if (address.Primary)
							detail = "address.gmail";
						else if (address.Home)
							detail = "address.gmail.home";
						else if (address.Work)
							detail = "address.gmail.work";
						buddy [detail] = address.Value.Replace ('\n',' ');
						i++;
					}
					i = 0;
					foreach (PhoneNumber phone in entry.Phonenumbers) {
						string detail = "phone.gmail." + i;
						if (phone.Primary)
							detail = "phone.gmail";
						else if (phone.Home)
							detail = "phone.gmail.home";
						else if (phone.Work)
							detail = "phone.gmail.work";
						buddy [detail] = phone.Value.Replace ('\n',' ');
						i++;
					}
					i = 0;
					foreach (EMail email in entry.Emails)
					{	
						string detail = "email.gmail." + i;
						if (email.Primary)
							detail = "email.gmail";
						else if (email.Home)
							detail = "email.gmail.home";
						else if (email.Work)
							detail = "email.gmail.work";
						
						buddy [detail] = email.Address;
					}
					contacts.Add (buddy);
				}
			} catch (Exception e) {
				Console.Error.WriteLine ("GMailContacts Error: {0}",e.Message);
			} finally {
				Monitor.Exit (conts_lock);
			}
		}
		
		/* 
		 * `NOT IMPLEMENTED IN GDATA API YET
		private static string GetBuddyPhotoFileLocation (string url)
		{
			string buddyIconDir;
			buddyIconDir = "~/.config/gnome-do/plugins/GCalendar/icons";
			buddyIconDir.Replace ("~", Environment.GetEnvironmentVariable("HOME"));
			
			//WebClient client = new WebClient ();
			//Client.DownloadFile(url, buddyIconDir + );
			return null;
		}
		*/
		
		public static bool TryConnect (string username, string password)
		{
			ContactsService test;
			ContactsQuery query; 
			
			test = new ContactsService (GAppName);
			test.setUserCredentials (username, password);
			query = new ContactsQuery(
				ContactsQuery.CreateContactsUri ("default"));
			query.StartDate = DateTime.Now;
			
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
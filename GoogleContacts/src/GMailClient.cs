/* GMailClient.cs 
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
using System.Net;
using System.Linq;
using System.Collections.Generic;

using Google.GData.Client;
using Google.GData.Contacts;
using Google.GData.Extensions;

using Do.Platform;
using Do.Universe;

namespace GMail
{
	
	public class GMailClient
	{
		#region class consts and fields

		const int MaxContacts = 1000;
		const string GUserId = "default";
		const string GAppName = "alexLauni-gnomeDoGMailPlugin-1.6";

		ContactsService service;
		
		#endregion
		
		public GMailClient (string username, string password)
		{
			service = new ContactsService (GAppName);
			service.setUserCredentials (username, password);

			Contacts = Enumerable.Empty<Item> ();
			ServicePointManager.CertificatePolicy = new CertHandler ();
		}

		public IEnumerable<Item> Contacts { get; private set; }

		public void UpdateContacts ()
		{
			List<Item> contacts = new List<Item> ();
			
			// set up the contacts query
			ContactsQuery query = new ContactsQuery (ContactsQuery.CreateContactsUri (GUserId));
			query.NumberToRetrieve = MaxContacts;
			query.UseSSL = true;
			
			try {
				ContactsFeed feed = service.Query(query);
				
				ContactItem buddy;
				foreach (ContactEntry entry in feed.Entries)
				{
					if (String.IsNullOrEmpty (entry.Title.Text)) continue;
					
					buddy = ContactItem.CreateWithName (entry.Title.Text);

					AddDetails (buddy, entry.Emails);
					AddDetails (buddy, entry.Phonenumbers);
					AddDetails (buddy, entry.PostalAddresses);
					
					contacts.Add (buddy);
				}
				
				Log.Debug ("Retrieved {0} contacts", contacts.Count ());
				Contacts = contacts;
			} catch (Exception e) {
				Log.Error ("GMailContacts Error: {0}",e.Message);
				Log.Debug (e.StackTrace);
			}
		}

		void AddDetails<T> (ContactItem contact, ExtensionCollection<T> extensions) 
			where T : CommonAttributesElement, new ()
		{
			int i;
			string detail;
			string detailBase;

			i = 0;
			foreach (T element in extensions) {
				detailBase = RootDetailForExtension (typeof (T));

				if (element.Primary)
					detail = detailBase;
				else if (element.Home)
					detail = detailBase + ".home";
				else if (element.Work)
					detail = detailBase + ".work";
				else
					detail = detailBase + "." + i;

				// for some reason emails behave differently. what the fuck is that?
				if (element is EMail)
					contact [detail] = (element as EMail).Address;
				else
					contact [detail] = element.Value.Replace ('\n', ' ');
			}
		}

		string RootDetailForExtension (Type extensionType)
		{			
			if (extensionType == typeof (EMail))
				return "email.gmail";
			else if (extensionType == typeof (PhoneNumber))
				return "phone.gmail";
			else if (extensionType == typeof (PostalAddress))
				return "address.gmail";
			else
				return "unknown.gmail";
		}
	}
}

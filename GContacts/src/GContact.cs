/* GContact.cs
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
using System.Xml;
using System.Net;
using System.Web;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

using Do.Addins;
using Do.Universe;
using Gnome.Keyring;

using Google.GData.Client;

namespace GContacts
{	
	public static class GContact
	{
		private static string auth_token, username, password;
		private static List<IItem> items;
		private static Timer ClearContactsTimer;
		const int CacheSeconds  = 350; //cache contacts
		
		static GContact () {
			items = new List<IItem> ();
			SetCredentials ();
			ClearContactsTimer = new Timer (ClearContacts);
		}
		
		public static List<IItem> Contacts {
			get { return items; }
		}
		
		public static string Auth_Token {
			get { return auth_token; }
			set { auth_token = value; }
		}
		
		public static string Username {
			get { return username; }
			set { username = value; }
		}
		
		public static string Password {
			get { return password; }
			set { password = value; }
		}
		
		public static void UpdateContacts ()
		{
			if (Auth_Token == null) return;
			try {
				if (!Monitor.TryEnter (items)) return;
				ClearContactsTimer.Change (CacheSeconds*1000, Timeout.Infinite);

				if (items.Count > 0) {
					Monitor.Exit (items);
					return;
				}
				
				XmlDocument contacts = new XmlDocument ();
				contacts.Load (MakeRequest ().GetResponseStream ());

				ContactItem buddy;
				string key, email, phone, address, name;
				key = email = phone = address = name = "";
				int i = 0;
				Dictionary<string, string> emails = new Dictionary<string,string> ();
				
				foreach (XmlNode contact in contacts.GetElementsByTagName ("entry")) {
					i = 0;
					foreach (XmlNode cont in contact.ChildNodes) {
						switch (cont.Name) {
						case ("title"): name = cont.InnerText; break;
						case ("gd:email"):
							i++;
							email = cont.Attributes.GetNamedItem ("address").Value;
							key = "email." + i;
							try {
								if (cont.Attributes.GetNamedItem ("primary").Value == "true" )
									key = "email";
							} catch {
							}
							if(!emails.ContainsKey (key))
								emails.Add (key, email);
							break;
						case ("gd:phoneNumber") : phone = cont.InnerText; break;
						case ("gd:postalAddress"): address = cont.InnerText; break;
						}
					}
					if (!name.Equals ("") && !email.Equals ("")) {
						buddy = ContactItem.Create (name);
						foreach (KeyValuePair<string, string> kvp in emails) 
							buddy[kvp.Key] = kvp.Value;
						buddy["phone"] = phone;
						buddy["address"] = address;
						//For some reason this is not working automatically, so
						//let's force it to do what we want.
						buddy["description"] = buddy["email"];
						items.Add (buddy);
						//Console.Error.WriteLine(name + ":" + buddy["email"]);
						emails.Clear ();
					}
				}
				Monitor.Exit (items);
			} catch (Exception e) {
				Console.Error.WriteLine (e.Message);
				return;
			}
		}
		
		private static HttpWebResponse MakeRequest ()
		{
			if (auth_token == null) return null;
			string encoded_username = HttpUtility.UrlEncode (username);
			string url = string.Format ("http://www.google.com/m8/feeds/contacts/{0}/base?max-results=5000",
			                            encoded_username);
			HttpWebRequest req = (HttpWebRequest) WebRequest.Create (url);
			WebHeaderCollection auth_header = new WebHeaderCollection ();
			auth_header.Add ("Authorization: GoogleLogin auth="+auth_token);
			req.Headers = auth_header;
			req.Method = "GET";
			 
			return (HttpWebResponse) req.GetResponse ();
		}
		
		private static void ClearContacts (object state)
		{
			lock (items) {
				items.Clear ();
			}		
		}
		
		private static void SetCredentials ()
		{
			string keyring_item_name = "Google Account";
			Hashtable request_attributes = new Hashtable();
			request_attributes["name"] = keyring_item_name;
			try {
				foreach(ItemData result in Ring.Find(ItemType.GenericSecret, request_attributes)) {
					if(!result.Attributes.ContainsKey("name") || !result.Attributes.ContainsKey("username") ||
						(result.Attributes["name"] as string) != keyring_item_name) 
						continue;
					
					username = (string)result.Attributes["username"];
					password = result.Secret;
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
			update_request_attributes["name"] = keyring_item_name;
			update_request_attributes["username"] = username;

			try {
				Ring.CreateItem(keyring, ItemType.GenericSecret, keyring_item_name,
				                update_request_attributes, password, true);
			} catch (Exception e) {
				Console.WriteLine(e.Message);
			}
		}			
	}
}

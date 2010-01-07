//  ThunderbirdContactItemSource.cs
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

// 

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using Do.Universe;

using Beagle.Util;

namespace Do.Addins.Thunderbird
{

	public class ThunderbirdContactItemSource : ItemSource
	{

		public class EmailContactDetail: Item, IContactDetailItem
		{
			readonly string detail;
			readonly ContactItem owner;

			public  EmailContactDetail(ContactItem owner, string detail)
			{
				this.owner  = owner;
				this.detail = detail;
			}

			public override string Name 
			{
				get
				{
					return owner[detail];
					// return AddinManager.CurrentLocalizer.GetString ("Email");
				}
			}

			public override string Description
			{
				get { return Value; }
			}

			public override string Icon
			{
				get { return "thunderbird"; }
			}

			public string Key 
			{
				get { return detail; }
			}

			public string Value
			{
				get { return owner[detail]; }
			}
		}
		
		const string BeginProfileName    = "Path=";
		const string BeginDefaultProfile = "Name=default";
		const string EMAIL_COUNTER       = "thunderbird.counter";
		const string THUNDERBIRD_EMAIL   = "email.thunderbird";
		
		Dictionary<string, Item> contacts; // name => ContactItem
		Dictionary<string, List<string> > emails; // name => list of emails
		
		public ThunderbirdContactItemSource ()
		{
		    contacts = new Dictionary<string, Item> ();
			emails   = new Dictionary<string, List<string>> ();
			// UpdateItems ();
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] {
					typeof (ContactItem),
				};
			}
		}
		
		public override string Name { get { return "Thunderbird Contacts"; } }
		public override string Description { get { return "Thunderbird Contacts"; } }
		public override string Icon { get { return "thunderbird"; } }
		
		public override void UpdateItems ()
		{
			try {
				_UpdateItems ();
			} catch (Exception e) {
				Console.Error.WriteLine ("Cannot index Thunderbird contacts because a {0} was thrown: {1}", e.GetType (), e.Message);
				return;
			}
		}
		
		public override IEnumerable<Item> Items {
			get { return contacts.Values; }
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			ContactItem contact = item as ContactItem;
			Console.Error.WriteLine("ParentItem: {0}[\"email\"]/{1}", contact["name"], contact["email"]);
			foreach (string detail in contact.Details)
			{
				Console.Error.WriteLine("{0} = {1}", detail, contact[detail]);
			}

			foreach (string detail in contact.Details)
			{
				if (detail.StartsWith(THUNDERBIRD_EMAIL))// && detail != "email")
				{
					Console.Error.WriteLine("ChildItem: {0}[{1}]={2}", contact["name"], detail, contact[detail]);
					yield return new EmailContactDetail(contact, detail);
				}
			}
			yield break;
		}
		
		void _UpdateItems ()
		{
 		    Console.Error.WriteLine("_UpdateItems");
		    MorkDatabase database, history;

			database = new MorkDatabase (GetThunderbirdAddressBookFilePath ());
			database.Read ();
			database.EnumNamespace = "ns:addrbk:db:row:scope:card:all";

			history = new MorkDatabase (GetThunderbirdHistoryFilePath ());
			history.Read ();
			history.EnumNamespace = "ns:addrbk:db:row:scope:card:all";

			contacts.Clear ();
			addContacts(history);
			addContacts(database);
			foreach (ContactItem item in contacts.Values)
			{
				foreach (string detail in item.Details)
				{
					Console.Error.WriteLine("{0} = {1}", detail, item[detail]);
				}
			}
		}

		void addContacts(MorkDatabase database)
		{
			foreach (string id in database) {
				Hashtable contact_row;
				ContactItem contact;
				
				contact_row = database.Compile (id, database.EnumNamespace);
				contact = CreateThunderbirdContactItem (contact_row);
				if (contact == null)
					continue;
				
				string name  = contact["name"];
				string email = contact["email"];
				if (!contacts.ContainsKey(name.ToLower()))
				{
					contacts.Add(name.ToLower(), contact);
				}
				Console.Error.WriteLine("Added: {0}[{1}]/{2}", name, contact[EMAIL_COUNTER], email);
			}
		}
	
		ContactItem CreateThunderbirdContactItem (Hashtable row) {
			ContactItem contact;
			string name, email;
			
//			foreach (object o in row.Keys)
//				Console.WriteLine ("\t{0} --> {1}", o, row[o]);
			
			// I think this will detect deleted contacts... Hmm...
			if (row["table"] == null || row["table"] as string == "C6")
				return null;
			
			// Name
			name = row["DisplayName"] as string;
			if (name == null || name == string.Empty)
				name = string.Format ("{0} {1}", row["FirstName"], row["LastName"]);
			
			// Email
			email = row["PrimaryEmail"] as string;
			
			if (name == null || name.Trim() == string.Empty)
			    name = email;

			if (string.IsNullOrEmpty(email))
			    return null;

			contact = ContactItem.Create (name);
			if (!emails.ContainsKey(name))
			{
				emails[name] = new List<string> ();
			}
			if (!emails[name].Contains(email))
			{
				int i = Convert.ToUInt16(contact[EMAIL_COUNTER]) + 1;
				contact[EMAIL_COUNTER] = i.ToString();
				string detail = THUNDERBIRD_EMAIL + "." + i;

				contact[detail] = email;
				emails[name].Add(email);

				Console.Error.WriteLine("Added {0}/{1}, num_email={2}",
										name, email, contact[EMAIL_COUNTER]);
			}
			
			return contact;
		}

		string GetThuderbirdDefaultProfilePath()
		{
			string home, path, profile;
			StreamReader reader;

			profile = null;
			home = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
			path = System.IO.Path.Combine (home, ".thunderbird/profiles.ini");
			try {
				reader = System.IO.File.OpenText (path);
			} catch {
				return null;
			}
			
			bool got_default = false;
			for (string line = reader.ReadLine (); line != null; line = reader.ReadLine ()) {
				if (got_default && line.StartsWith (BeginProfileName)) {
					line = line.Trim ();
					line = line.Substring (BeginProfileName.Length);
					profile = line;
					break;
				}
				else if (line.StartsWith (BeginDefaultProfile)) {
					got_default = true;
				}
			}
			reader.Close ();
			return profile;
		}

		string GetThunderbirdFilePath (string filename)
		{
			string path, home, profile;
			home = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
			profile = GetThuderbirdDefaultProfilePath();
			if (profile == null) {
				return null;
			}
			path = System.IO.Path.Combine (home, ".thunderbird");
			path = System.IO.Path.Combine (path, profile);
			path = System.IO.Path.Combine (path, filename);
			return path;
		}
		
		string GetThunderbirdHistoryFilePath ()
		{
			return GetThunderbirdFilePath("history.mab");
		}

		string GetThunderbirdAddressBookFilePath ()
		{
			return GetThunderbirdFilePath("abook.mab");
		}
	}
}

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

		class EmailContactDetail: Item, IContactDetailItem
		{
			readonly string		 detail, description;
			readonly ContactItem owner;

			public	EmailContactDetail (ContactItem owner, string detail)
			{
				this.owner	= owner;
				this.detail = detail;
				description = string.IsNullOrEmpty (owner["name"]) 
							  ? owner[detail]
							  : owner["name"];
			}

			public override string Name 
			{
				get	{ return owner[detail]; }
			}

			public override string Description
			{
				get { return description; }
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

		class EmailList
		{
			private Dictionary<string, uint> set;

			public EmailList () { set = new Dictionary<string, uint> (); }

			public void Add (string email, uint popularity)
			{
				if (!set.ContainsKey (email))
				{
					set.Add (email, popularity);
				}
				else
				{
					set[email] += popularity;
				}
			}

			public bool Contains (string email)
			{
				return set.ContainsKey (email);
			}

			public uint this [string email]
			{
				get { return set[email]; }
			}

			public int Count {
				get { return set.Count; }
			}

			public ICollection<string> Keys {
				get { return set.Keys; }
			}
		}

		class ThunderbirdEmail
		{
			public readonly string email;
			public readonly uint   popularity;

			public ThunderbirdEmail (string email, uint popularity)
			{
				this.email		= email;
				this.popularity = popularity;
			}
		}


		const string BeginProfileName	 = "Path=";
		const string BeginDefaultProfile = "Name=default";
		const string THUNDERBIRD_EMAIL	 = "email.thunderbird";

		Dictionary<string, Item> contacts; // name => ContactItem
		Dictionary<string, EmailList> emails; // name => list of emails
		
		public ThunderbirdContactItemSource ()
		{
			contacts = new Dictionary<string, Item> ();
			emails	 = new Dictionary<string, EmailList> ();
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
			Console.Error.WriteLine ("ParentItem: {0}[\"email\"]/{1}", contact["name"], contact["email"]);
			foreach (string detail in contact.Details)
			{
				Console.Error.WriteLine ("{0} = {1}", detail, contact[detail]);
			}

			foreach (string detail in contact.Details)
			{
				if (detail.StartsWith (THUNDERBIRD_EMAIL))
				{
					Console.Error.WriteLine ("ChildItem: {0}[{1}]={2}", contact["name"], detail, contact[detail]);
					yield return new EmailContactDetail (contact, detail);
				}
			}
			yield break;
		}
		
		void _UpdateItems ()
		{
			Console.Error.WriteLine ("_UpdateItems");
			MorkDatabase abook, history;

			abook = new MorkDatabase (GetThunderbirdAddressBookFilePath ());
			abook.Read ();
			abook.EnumNamespace = "ns:addrbk:db:row:scope:card:all";

			history = new MorkDatabase (GetThunderbirdHistoryFilePath ());
			history.Read ();
			history.EnumNamespace = "ns:addrbk:db:row:scope:card:all";

			contacts.Clear ();
			emails.Clear ();

			Console.WriteLine ("adding history");
			addEmails (history);
			Console.WriteLine ("adding address book");
			addEmails (abook);

			Console.WriteLine ("adding contacts");
			foreach (string name in emails.Keys)
			{
				CreateThunderbirdContactItem (name, emails[name]);
			}

			foreach (ContactItem item in contacts.Values)
			{
				foreach (string detail in item.Details)
				{
					Console.Error.WriteLine ("{0} = {1}", detail, item[detail]);
				}
			}
		}

		void addEmails (MorkDatabase database)
		{
			foreach (string id in database) {
				Hashtable contact_row;
				
				contact_row = database.Compile (id, database.EnumNamespace);

				Console.WriteLine ("mork row:");
				foreach (var key in contact_row.Keys)
				{
					Console.Write ("{0}={1}, ", key, contact_row[key]);
				}
				Console.WriteLine ();

				AddThunderbirdEmail (contact_row);
			}
		}
	
		void AddThunderbirdEmail (Hashtable row)
		{
			string name, email;
			uint popularity;
			
			// I think this will detect deleted contacts... Hmm...
			if (row["table"] == null || row["table"] as string == "C6")
				return;
			
			// Name
			name = row["DisplayName"] as string;
			if (name == null || name == string.Empty)
				name = string.Format ("{0} {1}", row["FirstName"], row["LastName"]);
			
			// Email
			email	 = row["PrimaryEmail"] as string;
			string p = row["PopularityIndex"] as string;
			try {
				popularity =  UInt32.Parse (p, System.Globalization.NumberStyles.HexNumber);
			}
			catch (Exception) {
				popularity = 0;
			}
			
			if (name == null || name.Trim () == string.Empty)
				name = email;

			if (string.IsNullOrEmpty (email))
				return;

			if (!emails.ContainsKey (name))
			{
				emails[name] = new EmailList ();
			}
			emails[name].Add (email, popularity);
		}

		void CreateThunderbirdContactItem (string name, EmailList emails)
		{
			int emailCount = emails.Count;
			ThunderbirdEmail[] sortedEmails = new ThunderbirdEmail[emailCount];

			int i = 0;
			foreach (string key in emails.Keys)
			{
				sortedEmails[i] = new ThunderbirdEmail (key, emails[key]);
				i++;
			}
			Array.Sort (sortedEmails, (x, y) => (int) (y.popularity - x.popularity));

			ContactItem contact = ContactItem.Create (name);
			for (i = 0; i < emailCount; i++)
			{
				string detail	= THUNDERBIRD_EMAIL + "." + i;
				contact[detail] = sortedEmails[i].email;

				Console.Error.WriteLine ("Added {0}[{1}]={2}, popularity={3}",
										 name, detail, contact[detail],
										 sortedEmails[i].popularity);
			}

			if (!contacts.ContainsKey (name.ToLower ()))
			{
				contacts.Add (name.ToLower (), contact);
			}
		}

		string GetThuderbirdDefaultProfilePath ()
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
			profile = GetThuderbirdDefaultProfilePath ();
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
			return GetThunderbirdFilePath ("history.mab");
		}

		string GetThunderbirdAddressBookFilePath ()
		{
			return GetThunderbirdFilePath ("abook.mab");
		}
	}
}

//  EmpathyContactItemSource.cs
//  
//  Author:
//       Xavier Calland <xavier.calland@gmail.com>
//  
//  Copyright (c) 2010 
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Mono.Addins;
using Do.Universe;
using Do.Platform;
using Do.Platform.ServiceStack;

namespace EmpathyPlugin
{

	public class EmpathyContactItemSource : ItemSource
	{
		const string iconPrefix = "icon-";
		
		List<Item> contacts;
		
		public EmpathyContactItemSource ()
		{
			contacts = new List<Item> ();
		}
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get
			{ 
				yield return typeof (ContactItem); 
				yield return typeof (IApplicationItem);
				yield return typeof (EmpathyBrowseBuddyItem);
			}
		}
		
		public override string Name
		{
			get { return AddinManager.CurrentLocalizer.GetString ("Empathy Contacts"); }
		}
		
		public override string Description
		{
			get { return AddinManager.CurrentLocalizer.GetString ("Contacts on your Empathy contact alist."); } 
		}
		
		public override string Icon
		{
			get { return "empathy"; }
		}
		
		public override IEnumerable<Item> Items 
		{
			get { return contacts; }
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			if (EmpathyPlugin.IsTelepathy (item))
			{
				yield return new EmpathyBrowseBuddyItem ();
			}
			else if (item is EmpathyBrowseBuddyItem)
			{
				foreach (ContactItem contact in contacts)
				{
					yield return contact;
				}
			}
		}

		public void ForceUpdateItems ()
		{
			Console.WriteLine("EmpathyContactItemSource.ForceUpdateItems");
			if (EmpathyPlugin.InstanceIsRunning)
			{
				contacts.Clear ();
				try
				{
						
					foreach (Contact contact in EmpathyPlugin.GetAllContacts)
					{
							ContactItem contactItem = ContactItem.Create (contact.Alias);
							contactItem["email"] = contact.ContactId;
						contactItem["is-empathy"] = "true";
						if(contact.AvatarToken != null && contact.AvatarToken != "") 
						{
							string[] elts = new string[]{Environment.GetFolderPath (Environment.SpecialFolder.Personal), EmpathyPlugin.AVATAR_PATH, contact.Account.cm, contact.Account.proto, contact.AvatarToken};
							contactItem["photo"] = elts.Aggregate((aggregation, val) => Path.Combine (aggregation, val));
						}
						contacts.Add (contactItem);	
					}
				}
				catch (Exception e)
				{ 
					Log<EmpathyContactItemSource>.Error ("Could not get Empathy contacts: {0}", e.Message);
					Log<EmpathyContactItemSource>.Error (e.StackTrace);
				}
			}
		}
		
		public override void UpdateItems ()
		{	
			ForceUpdateItems();
		}
	}
}
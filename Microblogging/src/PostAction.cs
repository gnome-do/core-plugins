/*
 * PostAction.cs
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
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using Mono.Unix;

using Do.Platform;
using Do.Universe;
using Do.Platform.Linux;

namespace Microblogging
{		
	public sealed class PostAction : Act, IConfigurable
	{
		const int MaxMessageLength = 140;
		
		public PostAction ()
		{
		}
		
		public override string Name {
			get { return string.Format (Catalog.GetString ("Post to {0}"), Microblog.Preferences.MicroblogService); }
		}
		
		public override string Description {
			get { return string.Format (Catalog.GetString ("Update {0} status"), Microblog.Preferences.MicroblogService); }
		}
		
		public override string Icon {
			get { return "microblogging.svg@" + GetType ().Assembly.FullName; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
            get { yield return typeof (ITextItem); }
        }

        public override bool SupportsItem (Item item) 
        {
            return (item as ITextItem).Text.Length <= MaxMessageLength;
        }
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
            get { 
				yield return typeof (FriendItem);
				yield return typeof (MicroblogStatus);
			}
        }

        public override bool ModifierItemsOptional {
            get { return true; }
        }
                        
        public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
        {
			ITextItem message = items.First () as ITextItem;
			string buddyName = GetContactNameFromItem (modItem);

        	// make sure we dont go over 140 chars with the contact screen name
        	return !string.IsNullOrEmpty (buddyName) && message.Text.Length + buddyName.Length < MaxMessageLength;
        }

		public override IEnumerable<Item> DynamicModifierItemsForItem(Item item)
		{
			return Microblog.Friends.OfType<Item> ();
		}

        
        public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
        {
        	string status;
			MicroblogStatusReply reply;
        	
        	status = (items.First () as ITextItem).Text;
			if (modItems.Any ()) {
				status = BuildTweet (status, modItems);
			
				if (modItems.First () is FriendItem) {
					reply = new MicroblogStatusReply (null, status);
				} else {
					MicroblogStatus s = modItems.First () as MicroblogStatus;
					reply = new MicroblogStatusReply (s.Id, status);
				}
			} else {
				reply = new MicroblogStatusReply (null, status);
			}
			
			Thread updateRunner = new Thread (new ParameterizedThreadStart (Microblog.UpdateStatus));
			updateRunner.IsBackground = true;
			updateRunner.Start (reply);
			
			return null;
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new GenConfig ();
		}

		string BuildTweet(string status, IEnumerable<Item> modItems)
		{
			string tweet = "";
			string buddyName;
			
			//Handle situations without a contact
			if (modItems.Count () == 0) return status;
			
			buddyName =  GetContactNameFromItem (modItems.First ());
			
			// Direct messaging starts with "d "
			if (status.Substring (0,2).Equals ("d ")) {
				tweet = "d " + buddyName + " " +	status.Substring (2);
					
			// Tweet replying
			} else {
				foreach (Item contact in modItems) {
					tweet += "@" + GetContactNameFromItem (contact) + " " ;
				}
				
				tweet += status;
			}
			
			return tweet;
		}
		
		string GetContactNameFromItem (Item modItem)
		{
			return (modItem is FriendItem) ? (modItem as FriendItem).Name : (modItem as MicroblogStatus).Owner;
		}
	}
}

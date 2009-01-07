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
			get { return "twitter-icon.svg@" + GetType ().Assembly.FullName; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
            get { yield return typeof (ITextItem); }
        }

        public override bool SupportsItem (Item item) 
        {
            return (item as ITextItem).Text.Length <= MaxMessageLength;
        }
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
            get { yield return typeof (ContactItem); }
        }

        public override bool ModifierItemsOptional {
            get { return true; }
        }
                        
        public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
        {
			ITextItem message = items.First () as ITextItem;
			ContactItem buddy = modItem as ContactItem;
			string buddyName = buddy [MicroblogClient.ContactKeyName] ?? "";

        	// make sure we dont go over 140 chars with the contact screen name
        	return message.Text.Length + buddyName.Length < MaxMessageLength;
        }

		public override IEnumerable<Item> DynamicModifierItemsForItem(Item item)
		{
			return Microblog.Friends;
		}

        
        public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
        {
        	string status;
        	
        	status = (items.First () as ITextItem).Text;
			if (modItems.Any ())
				status = BuildTweet (status, modItems);
			
			Thread updateRunner = new Thread (new ParameterizedThreadStart (Microblog.UpdateStatus));
			updateRunner.IsBackground = true;
			updateRunner.Start (status);
			
			return null;
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new GenConfig ();
		}

		string BuildTweet(string status, IEnumerable<Item> modItems)
		{
			string tweet = "";
			
			//Handle situations without a contact
			if (modItems.Count () == 0) return status;
			
			// Direct messaging
			if (status.Substring (0,2).Equals ("d ")) {
				tweet = "d " + (modItems.First () as ContactItem) [MicroblogClient.ContactKeyName] + " " +	status.Substring (2);
					
			// Tweet replying
			} else {
				foreach (ContactItem contact in modItems) {
					tweet += "@" + contact [MicroblogClient.ContactKeyName] + " " ;
				}
				
				tweet += status;
			}
			return tweet;
		}
	}
}

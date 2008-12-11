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

using Do.Addins;
using Do.Universe;

namespace Microblogging
{		
	public sealed class PostAction : IAction, IConfigurable
	{
		const int MaxLength = 140;
		string active_service;
		
		public PostAction ()
		{
			active_service = Microblog.Preferences.MicroblogService;
		}
		
		public string Name {
			get { return string.Format (Catalog.GetString ("Post to {0}"), active_service); }
		}
		
		public string Description {
			get { return string.Format (Catalog.GetString ("Update {0} status"), active_service); }
		}
		
		public string Icon {
			get { return "twitter-icon.png@" + GetType ().Assembly.FullName; }
		}
		
		public IEnumerable<Type> SupportedItemTypes {
            get {
                return new Type [] {
                    typeof (ITextItem),
                };
            }
        }

        public bool SupportsItem (IItem item) 
        {
            return (item as ITextItem).Text.Length <= MaxLength;
        }
		
		public IEnumerable<Type> SupportedModifierItemTypes {
            get { 
                return new Type [] {
                    typeof (ContactItem),
                };
            }
        }

        public bool ModifierItemsOptional {
            get { return true; }
        }
                        
        public bool SupportsModifierItemForItems (IEnumerable<IItem> items, IItem modItem)
        {
        	//make sure we dont go over 140 chars with the contact screen name
            return (modItem as ContactItem) [Microblog.ContactProperty] != null &&
            	((items.First () as ITextItem).Text.Length + ((modItem as ContactItem) 
            	[Microblog.ContactProperty]).Length < MaxLength);
        }
        
        public IEnumerable<IItem> DynamicModifierItemsForItem (IItem item)
        {
            return Microblog.Friends;
        }

        public IEnumerable<IItem> Perform (IEnumerable<IItem> items, IEnumerable<IItem> modItems)
        {
        	string status;
        	
        	status = (items.First () as ITextItem).Text;
			if (modItems.Any ())
				status = BuildTweet (status, modItems.ToArray ());
			
			Thread updateRunner = new Thread (new ParameterizedThreadStart (Microblog.Post));
			updateRunner.Start (status);
			
			return null;
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new GenConfig ();
		}
		
		private string BuildTweet(string status, IEnumerable<IItem> modItems)
		{
			string tweet = "";
			
			//Handle situations without a contact
			if (modItems.Count () == 0) return status;
			
			// Direct messaging
			if (status.Substring (0,2).Equals ("d "))
				tweet = "d " + (modItems.First () as ContactItem) [Microblog.ContactProperty] + " " +	status.Substring (2);
					
			// Tweet replying
			else {
				foreach (ContactItem contact in modItems) {
					tweet += "@" + contact [Microblog.ContactProperty] + " " ;
				}
				
				tweet += status;
			}
			return tweet;
		}
	}
}
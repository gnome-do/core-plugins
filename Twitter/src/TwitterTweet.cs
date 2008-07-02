/*
 * TwitterTweet.cs
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
using System.Threading;
using System.Collections.Generic;

using Do.Universe;
using Do.Addins;
using Twitterizer.Framework;

namespace DoTwitter
{		
	public sealed class TweetAction : IAction, IConfigurable
	{
		public string Name {
			get { return "Tweet"; }
		}
		
		public string Description {
			get { return "Update Twitter status"; }
		}
		
		public string Icon {
			get { return "twitter-icon.png@" + GetType ().Assembly.FullName; }
		}
		
		public Type [] SupportedItemTypes {
            get {
                return new Type [] {
                    typeof (ITextItem),
                };
            }
        }

        public bool SupportsItem (IItem item) 
        {
            return (item as ITextItem).Text.Length < 140;
        }
		
		public Type [] SupportedModifierItemTypes {
            get { 
                return new Type [] {
                    typeof (ContactItem),
                };
            }
        }

        public bool ModifierItemsOptional {
            get { return true; }
        }
                        
        public bool SupportsModifierItemForItems (IItem [] items, IItem modItem)
        {
            return (modItem as ContactItem) ["twitter.screenname"] != null;
        }
        
        public IItem [] DynamicModifierItemsForItem (IItem item)
        {
            return null;
        }

        public IItem[] Perform (IItem [] items, IItem [] modItems)
        {			
			TwitterAction.Status = (items [0] as ITextItem).Text;
			if (modItems.Length > 0)
				TwitterAction.Status = BuildTweet (items [0], modItems [0]);
			
			Thread updateRunner = new Thread (new ThreadStart (TwitterAction.Tweet));
			updateRunner.Start ();
			
			return null;
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new GenConfig ();
		}
		
		private string BuildTweet(IItem t, IItem c)
		{
			ITextItem text = t as ITextItem;
			ContactItem contact = c as ContactItem;
			string tweet = "";
			
			//Handle situations without a contact
			if (contact == null) return text.Text;
			
			// Direct messaging
			if (text.Text.Substring (0,2).Equals ("d "))
				tweet = "d " + contact ["twitter.screenname"] +
					" " +	text.Text.Substring (2);
			// Tweet replying
			else
				tweet = "@" + contact ["twitter.screenname"] + " " + text.Text;
			
			return tweet;
		}
	}
}
/*
 * TwitterFriendSource.cs
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
using Twitterizer.Framework;

namespace DoTwitter
{
	public sealed class TwitterFriendSource : IItemSource
	{
		private List<IItem> items;
		public TwitterFriendSource()
		{
			items = new List<IItem> ();
			UpdateItems ();
		}
		
		public string Name {
			get { return "Twitter friends"; }
		}
		
		public string Description {
			get { return "Indexes your Twitter friends"; }
		}
		
		public string Icon {
			get { return "system-users"; }
		}
		
		public Type [] SupportedItemTypes {
			get {
				return new Type [] {
					typeof (ContactItem),
				};
			}
		}
		
		public ICollection<IItem> Items {
			get { return items; }
		}
		
		public ICollection<IItem> ChildrenOfItem (IItem parent)
		{
			return null;
		}
		
		public void UpdateItems ()
		{	
			Thread updateRunner = new Thread (new ThreadStart (TwitterAction.UpdateFriends));
			updateRunner.Start ();
			items = TwitterAction.Friends;
		}
	}
}
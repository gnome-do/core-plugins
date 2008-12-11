// TwitterFriendSource.cs
// 
// GNOME Do is the legal property of its developers, whose names are too
// numerous to list here.  Please refer to the COPYRIGHT file distributed with
// this source distribution.
// 
// This program is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free Software
// Foundation, either version 3 of the License, or (at your option) any later
// version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
// details.
// 
// You should have received a copy of the GNU General Public License along with
// this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Mono.Unix;

using Do.Addins;
using Do.Universe;
using Do.Platform;
using Twitterizer.Framework;

namespace Twitter
{
	public sealed class TwitterFriendSource : IItemSource, IConfigurable
	{
	
		const int StatusUpdateTimeout = 30 * 1000;
		
		public TwitterFriendSource()
		{
			GLib.Timeout.Add (StatusUpdateTimeout, GetUpdates);
		}
		
		public string Name {
			get { return Catalog.GetString ("Twitter friends"); }
		}
		
		public string Description {
			get { return Catalog.GetString ("Indexes your Twitter friends"); }
		}
		
		public string Icon {
			get { return "twitter_items.png@" + GetType ().Assembly.FullName; }
		}
		
		public IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type [] { };
			}
		}
		
		public IEnumerable<IItem> Items {
			get { return Enumerable.Empty<IItem> (); }
		}
		
		public IEnumerable<IItem> ChildrenOfItem (IItem parent)
		{
			return null;
		}
		
		public void UpdateItems ()
		{	
			Thread updateRunner = new Thread (new ThreadStart (Twitter.UpdateFriends));
			updateRunner.IsBackground = true;
			updateRunner.Start ();
		}
		
		public Gtk.Bin GetConfiguration () {
			return new Configuration ();
		}
		
		public bool GetUpdates ()
		{
			Thread updateRunner = new Thread (new ThreadStart (Twitter.UpdateTweets));
			updateRunner.Start ();
			return true;
		}
	}
}

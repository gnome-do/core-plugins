// FriendSource.cs
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

namespace Microblogging
{
	public sealed class FriendSource : ItemSource, IConfigurable
	{
	
		const int StatusUpdateTimeout = 30 * 1000;
		string active_service;
		
		public FriendSource()
		{
			//GLib.Timeout.Add (StatusUpdateTimeout, GetUpdates);
			Microblog.Connect (Microblog.Preferences.Username, Microblog.Preferences.Password);
			active_service = Microblog.Preferences.MicroblogService;
		}
		
		public override string Name {
			get { return string.Format (Catalog.GetString ("{0} friends"), active_service); }
		}
		
		public override string Description {
			get { return string.Format (Catalog.GetString ("Indexes your {0} friends"), active_service); }
		}
		
		public override string Icon {
			get { return "twitter_items.png@" + GetType ().Assembly.FullName; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return Enumerable.Empty<Type> ();
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
			/*
			Thread updateRunner = new Thread (new ThreadStart (Microblog.UpdateFriends));
			updateRunner.IsBackground = true;
			updateRunner.Start ();
			*/
		}
		
		public Gtk.Bin GetConfiguration () {
			return new Configuration ();
		}
		
		public bool GetUpdates ()
		{
			/*
			Thread updateRunner = new Thread (new ThreadStart (Microblog.UpdateTimeline));
			updateRunner.Start ();
			*/
			return true;
			
		}
	}
}

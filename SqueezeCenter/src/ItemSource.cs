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

using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;


using Do.Universe;
using Do.Platform.Linux;

namespace SqueezeCenter
{
	
	public class ItemSource : Do.Universe.ItemSource,  IConfigurable
	{
		List<Item> items;		
		List<AlbumMusicItem> albums;
		List<ArtistMusicItem> artists;
		
		public ItemSource ()
		{	
			items = new List<Item> ();
			albums = new List<AlbumMusicItem>();
			artists = new List<ArtistMusicItem>();
			UpdateItems ();
		}
		
		public Gtk.Bin GetConfiguration ()
		{			
			return new Configuration ();
		}

		public override string Name { get { return "SqueezeCenter"; } }
		public override string Description { get { return "Artists, albums and radio."; } }
		public override string Icon { get { return "SB on.png@" + this.GetType ().Assembly.FullName; } }		

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] {
					typeof (MusicItem),
					typeof (RadioItem),
					typeof (BrowseMusicItem),
					typeof (IApplicationItem),
				};
			}
		}

		public override IEnumerable<Item> Items 
		{ 
			get 
			{			
				return items; 
			} 
		}

		public override IEnumerable<Item> ChildrenOfItem (Item parent)			
		{
			List<Item> children = new List<Item> ();
			
			if (parent is IApplicationItem && parent.Name == this.Name) {
				children.Add (new BrowseAlbumsMusicItem ());
				children.Add (new BrowseArtistsMusicItem ());
			}
			else if (parent is ArtistMusicItem) {
				foreach (AlbumMusicItem album in albums)
					if(album.Artist == parent)
						children.Add (album);
			}
			else if (parent is BrowseAlbumsMusicItem) {
				foreach (AlbumMusicItem album in albums)
					children.Add (album);
			}
			else if (parent is BrowseArtistsMusicItem) {
				foreach (ArtistMusicItem album in artists)
					children.Add (album);
			}
			else if (parent is RadioItem) {
				children.AddRange ((parent as RadioItem).Children);
			}
			
			return children;
		}

		public override void UpdateItems ()
		{
			items.Clear ();

			// Add artists
			albums.Clear();
			albums.AddRange (Server.Instance.GetAlbums ());

			// Add albums
			artists.Clear();
			artists.AddRange (Server.Instance.GetArtists ());
						
			// Add radios and all children
			foreach (RadioSuperItem r in Server.Instance.GetRadios ()) {
#if VERBOSE_OUTPUT
				Console.WriteLine ("SQC: Adding radio:" + r.Name + "  Children: " + r.GetChildrenRecursive ().Length);
#endif
				// items.Add (r);
				items.AddRange (r.GetChildrenRecursive ());
			}			

			// Add players
			items.AddRange (Server.Instance.GetConnectedPlayersAsItem ());

			// Add browse features
			items.Add (new BrowseAlbumsMusicItem ());
			items.Add (new BrowseArtistsMusicItem ());

										
			// Add artists and albums to items
			items.Capacity = Math.Max (items.Capacity, items.Count + albums.Count + artists.Count);
			foreach (Item album in albums) items.Add (album);
			foreach (Item artist in artists) items.Add (artist);
		}	
	}
}

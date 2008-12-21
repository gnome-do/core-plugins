// TorrentStatusAction.cs
//
//GNOME Do is the legal property of its developers. Please refer to the
//COPYRIGHT file distributed with this
//source distribution.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
//

using System;
using System.Collections.Generic;

using MonoTorrent.Client;

using Do.Universe;


namespace Do.Riptide
{
	
	
	public class TorrentStatusAction : Act
	{
		
		public override string Name {
			get {
				return "Torrent Status";
			}
		}

		public override string Description {
			get {
				return "Check Status of All Torrents";
			}
		}

		public override string Icon {
			get {
				return "stock_internet";
			}
		}

		public override bool SupportsItem (Item item)
		{
			return ((item as ITextItem).Text.Length == 0);
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { return new Type[] {typeof (ITextItem)}; }
		}


		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			List<Item> outitems = new List<Item> ();
			foreach (TorrentManager t in TorrentClientManager.Managers) {
				outitems.Add (new ActiveTorrentItem (t));
			}
			
			return outitems.ToArray ();
		}

		
		public TorrentStatusAction()
		{
		}

	}
}

// ResumeTorrentAction.cs
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
using MonoTorrent.Client;
using MonoTorrent.Common;

using Do.Addins;
using Do.Universe;

namespace Do.Riptide
{
	
	
	public class ResumeTorrentAction : AbstractAction
	{
		
		public override string Name {
			get { return "Resume Torrent"; }
		}
		
		public override string Description {
			get { return "Resume a Torrent"; }
		}

		public override string Icon {
			get { return "gtk-media-play"; }
		}

		public override Type[] SupportedItemTypes {
			get { return new Type[] { typeof (ITorrentItem), }; }
		}
		
		public override bool SupportsItem (IItem item)
		{
			if (!(item is ITorrentItem)) return false;
			
			TorrentManager tor = (item as ITorrentItem).ActiveTorrent;
			
			return (tor.State == TorrentState.Paused);
		}


		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			foreach (IItem item in items) {
				if (!(item is ITorrentItem)) continue;
				(item as ITorrentItem).ActiveTorrent.Start ();
			}
			
			return null;
		}

	}
}


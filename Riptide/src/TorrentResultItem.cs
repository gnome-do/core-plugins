// TorrentResultItem.cs
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

using Do.Universe;

namespace Do.Riptide
{
	public class TorrentResultItem : IItem, IComparable
	{
		private string name, url, size;
		private int seeds, leechers;
		
		public string Name {
			get { return name; }
		}

		public string Description {
			get {
				return "Seeds: " + Seeds + ", Leechers: " + Leechers + ", Size: " + Size;
			}
		}

		public string Icon {
			get { return "gtk-save-as"; }
		}
		
		public string URL {
			get { return url; }
			set { url = value; }
		}
		
		public string Size {
			get { return size; }
			set { size = value; }
		}
		
		public int Seeds {
			get { return seeds; }
			set { seeds = value; }
		}
		
		public int Leechers {
			get { return leechers; }
			set { leechers = value; }
		}
			
		public TorrentResultItem(string name)
		{
			this.name = name;
		}

		public int CompareTo (object obj)
		{
			if (obj is TorrentResultItem) {
				return this.Seeds.CompareTo ((obj as TorrentResultItem).Seeds) * -1;
			} else {
				throw new ArgumentException ("Object is not a TorrentResultItem");
			}
		}
	}
}

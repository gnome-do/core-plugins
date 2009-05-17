// WindowItem.cs
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
using System.Linq;
using Wnck;

using Mono.Unix;

using Do.Universe;
using Do.Interface.Wink;

namespace WindowManager
{	
	public class WindowItem : Item, IWindowItem
	{
		List<Wnck.Window> windows;
		string icon;
		
		public override string Name {
			get {
				return windows.First ().Name;
			}
		}

		public override string Description {
			get {
				return windows.First ().Name;
			}
		}
		
		public IEnumerable<Wnck.Window> Windows {
			get {
				return windows;
			}
		}

		public override string Icon {
			get {
				return icon;
			}
		}
		
		public WindowItem(Window w, string icon)
		{
			this.icon = icon;
			windows = new List<Window> ();
			windows.Add (w);
		}
		
		public WindowItem(IEnumerable<Window> w, string icon)
		{
			this.icon = icon;
			windows = new List<Window> (w);
		}
	}
}

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
using Wnck;

using Do.Universe;

namespace WindowManager
{	
	public class WindowItem : Item, IWindowItem
	{
		Wnck.Window window;
		string icon;
		
		public override string Name {
			get {
				return window.Name;
			}
		}

		public override string Description {
			get {
				return window.Name;
			}
		}
		
		public Wnck.Window Window {
			get {
				return window;
			}
		}

		public override string Icon {
			get {
				return icon;
			}
		}
		
		public WindowItem(Window w, string icon)
		{
			this.window = w;
			this.icon = icon;
		}
		
	}
	
	public class ScreenItem : Item, IScreenItem
	{
		string name, description, icon;
		
		public override string Name {
			get {
				return name;
			}
		}

		public override string Description {
			get {
				return description;
			}
		}

		public override string Icon {
			get {
				return icon;
			}
		}

		
		public ScreenItem (string name, string description, string icon) 
		{
			this.name = name;
			this.icon = icon;
			this.description = description;
		}
	}
}

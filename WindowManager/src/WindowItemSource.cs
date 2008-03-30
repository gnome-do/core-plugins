// WindowItemSource.cs
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

using Do.Universe;
using Wnck;

namespace WindowManager
{
	
	
	public class ScreenItemSource : IItemSource
	{
		List<IItem> items;
		
		public string Name {
			get {
				return "Window Screen Items";
			}
		}

		public string Description {
			get {
				return "Actions you can do to your screens.";
			}
		}

		public string Icon {
			get {
				return "desktop"; //fixme
			}
		}

		public Type[] SupportedItemTypes {
			get {
				return new Type[] {
					typeof (IScreenItem) };
			}
		}

		public ICollection<IItem> Items {
			get {
				items.Add (new ScreenItem ("Current Viewport", 
				                           "Everything on the Current Viewport",
				                           "desktop"));
				
				return items;
			}
		}

		
		public ScreenItemSource()
		{
			items = new List<IItem> ();
		}

		public ICollection<IItem> ChildrenOfItem (IItem item)
		{
			return null;
		}

		public void UpdateItems ()
		{
		}
	}
}

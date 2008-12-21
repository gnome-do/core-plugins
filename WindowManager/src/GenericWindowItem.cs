// GenericWindowItem.cs
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
	public enum GenericWindowType
	{
		CurrentWindow = 0,
		PreviousWindow = 1,
		CurrentApplication = 2,
		PreviousApplication = 3,
	}
	
	public class GenericWindowItem : Item
	{
		string name, description, icon;
		GenericWindowType windowType;
		
		public GenericWindowItem (string name, string description, string icon, 
		                          GenericWindowType windowType) 
		{
			this.name = name;
			this.description = description;
			this.icon = icon;
			this.windowType = windowType;
		}
		
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

		public GenericWindowType WindowType {
			get {
				return windowType;
			}
		}
		
	}
}

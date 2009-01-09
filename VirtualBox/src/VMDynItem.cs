// VMDynItm.cs
//
//  GNOME Do is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this
//  source distribution.
//
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
using Do.Universe;

namespace VirtualBox
{
	public class VMDynItm : Item
	{
		public VMDynItm(string n, string d, string i, VMState m)
		{
			name = n;
			desc = d;
			icon = i;
			mode = m;
		}	
		private string name, desc, icon;
		private VMState mode;
		
		public VMState Mode { get { return mode; } }
		public override string Name { get { return name; } }
		public override string Description { get { return desc; } }
		public override string Icon { get { return icon; } }	
	}
}

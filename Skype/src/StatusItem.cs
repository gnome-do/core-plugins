//  StatusItem.cs
//
//  GNOME Do is the legal property of its developers, whose names are too numerous
//  to list here.  Please refer to the COPYRIGHT file distributed with this
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

namespace Skype
{
	
	
	public class StatusItem : Item
	{
		string name, icon;
		public StatusItem (string name, string code, string icon)
			: this (name, code, icon, true)
		{
		}		
		
		public StatusItem (string name, string code, string icon, bool show)
		{
			this.name = name;
			this.icon = icon;
			this.Code = code;
			this.Showable = show;
		}
		
		public override string Name {
			get { return name; }
		}
		
		public override string Description {
			get { return name; }
		}

		public override string Icon {
			get { return string.Format ("{0}@{1}", icon, typeof (Skype).Assembly.FullName); }
		}
		
		public string Code { get; private set; }
		
		public bool Showable { get; private set; }
	}
}

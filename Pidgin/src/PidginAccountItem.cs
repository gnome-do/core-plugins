// PidginAccountItem.cs
// 
// Copyright (C) 2008 [Alex Launi]
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using Do.Universe;

namespace Do.Addins.Pidgin
{
	public class PidginAccountItem : Item
	{
		string name, proto;
		int id;
		public PidginAccountItem(string name, string proto, int id)
		{
			this.name = name;
			this.proto = proto.ToLower ();
			if (proto.Equals ("XMPP"))
			    this.proto = "jabber";
			this.id = id;
		}
		
		public override string Name {
			get {
				return name;
			}
		}
		
		public override string Description {
			get {
				return proto;
			}
		}
		
		public override string Icon {
			get {
				string icon_base = "/usr/share/pixmaps/pidgin/protocols/48/";
				string proto_icon = proto;
				string icon = icon_base + proto_icon + ".png";

				if (System.IO.File.Exists (icon))
					return icon;
				else
					return "internet-group-chat";
			}
		}
		
		public int ID {
			get {
				return id;
			}
		}
	}
}

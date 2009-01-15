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
using System.IO;

using Do.Universe;
using Do.Platform;

namespace PidginPlugin
{

	public class PidginAccountItem : Item
	{
		string name;

		public PidginAccountItem (string name, string proto, int id)
		{
			proto = proto.ToLower ();

			this.name = name;
			Id = id;
			Proto = proto.Equals ("XMPP") ? "jabber" : proto;
		}

		public int Id { get; protected set; }
		public string Proto { get; protected set; }
		
		public override string Name {
			get { return name; }
		}
		
		public override string Description {
			get { return Proto; }
		}
		
		public override string Icon {
			get {
				string icon = Path.Combine (
					"/usr/share/pixmaps/pidgin/protocols/48", Proto + ".png");
				return File.Exists (icon) ? icon : "internet-group-chat";
			}
		}
		
	}
}

// PidginHandleContactDetailItem.cs
// 
// GNOME Do is the legal property of its developers, whose names are too numerous
// to list here.  Please refer to the COPYRIGHT file distributed with this
// source distribution.
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
using Mono.Unix;

using Do.Universe;

namespace PidginPlugin
{

	class PidginHandleContactDetailItem : Item, IContactDetailItem
	{

		string proto, handle, custom_icon;
		
		public PidginHandleContactDetailItem (string proto, string handle) : this (proto, handle, null)
		{
		}
		
		public PidginHandleContactDetailItem (string proto, string handle, string custom_icon)
		{
			this.proto = proto;
			this.handle = handle;
			this.custom_icon = custom_icon;
		}

		public override string Name {
			get {
				return string.Format ("{0} ({1})", handle, ReadableProto (proto));
			}
		}

		public override string Description {
			get {
				string online = (Pidgin.BuddyIsOnline (handle)) 
					? Catalog.GetString ("Online") 
					: Catalog.GetString ("Offline");
				
				return string.Format ("{0} {1} ({2})", 
				                      ReadableProto (proto), 
				                      Catalog.GetString ("Handle"),
				                      online);
			}
		}

		public override string Icon {
			get { return custom_icon ?? Pidgin.GetProtocolIcon (proto); }
		}
		
		public string Key {
			get { return proto; }
		}

		public string Value {
			get { return handle; }
		}

		string ReadableProto (string proto)
		{
			string[] parts = proto.Split ('-');
			return char.ToUpper (parts[1][0]) + parts[1].Substring(1);
		}
	}
}

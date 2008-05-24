/*
 * PidginHandleContactDetailItem.cs
 * 
 * GNOME Do is the legal property of its developers, whose names are too numerous
 * to list here.  Please refer to the COPYRIGHT file distributed with this
 * source distribution.
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;

using Do.Universe;

namespace Do.Addins.Pidgin
{
	class PidginHandleContactDetailItem : IContactDetailItem
	{
		string proto, handle;

		public PidginHandleContactDetailItem (string proto, string handle)
		{
			this.proto = proto;
			this.handle = handle;
		}

		public string Name
		{
			get {
				return string.Format ("{0} ({1})", handle, ReadableProto (proto));
			}
		}

		public string Description
		{
			get {
				return ReadableProto (proto) + " " + "Handle" ;
			}
		}

		public string Icon
		{
			get {
				string icon_base = "/usr/share/pixmaps/pidgin/protocols/48/";
				string proto_icon = proto.Substring (proto.IndexOf ("-")+1);
				string icon = icon_base + proto_icon + ".png";

				if (System.IO.File.Exists (icon))
					return icon;
				else
					return "internet-group-chat";
			}
		}

		public string Key { get { return proto; } }
		public string Value { get { return handle; } }

		string ReadableProto (string proto)
		{
			switch (proto) {
			case "prpl-aim":		return "AIM";
			case "prpl-jabber":		return "Jabber";
			default:				return proto;
			}
		}
	}
}

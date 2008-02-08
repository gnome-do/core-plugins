//  PidginChatAction.cs (requires package libpurple-bin to use purple-remote)
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
using System.Threading;
using System.Diagnostics;

using Do.Universe;

namespace Do.Addins.Pidgin
{
	public class PidginChatAction : AbstractAction
	{
		public PidginChatAction ()
		{
		}
		
		public override string Name
		{
			get { return "Chat"; }
		}
		
		public override string Description
		{
			get { return "Send an instant message to a friend."; }
		}
		
		public override string Icon
		{
			get { return "internet-group-chat"; }
		}
		
		public override Type[] SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (ContactItem),
					typeof (PidginHandleContactDetailItem),
				};
			}
		}

		public override bool SupportsItem (IItem item)
		{
			if (item is ContactItem) {
				foreach (string detail in (item as ContactItem).Details) {
					if (detail.StartsWith ("prpl-")) return true;
				}
				return false;
			} else if (item is PidginHandleContactDetailItem) {
				return true;
			}
			return false;
		}
		
		public override IItem[] Perform (IItem[] items, IItem[] modifierItems)
		{
			IItem item = items[0];
			string protocol, screenname, dbus_instruction;

			protocol = screenname = "";
			if (item is ContactItem) {
				// Just grab the first protocol we see.
				ContactItem contact = item as ContactItem;
				foreach (string detail in contact.Details) {
					if (detail.StartsWith ("prpl-")) {
						protocol = detail;
						screenname = contact[detail];
						break;
					}
				}
			} else if (item is PidginHandleContactDetailItem) {
				protocol = (item as PidginHandleContactDetailItem).Key;
				screenname = (item as PidginHandleContactDetailItem).Value;
			}
			if (null != protocol && null != screenname) {
				new Thread ((ThreadStart) delegate {
					if (!Pidgin.InstanceIsRunning ()) {
						Process.Start ("pidgin");
						Thread.Sleep (4 * 1000);
					}

					if (protocol.Contains ("-"))
						protocol = protocol.Substring (protocol.IndexOf ("-")+1);
					dbus_instruction = string.Format ("\"{0}:goim?screenname={1}\"", protocol, screenname);
					Process.Start ("purple-remote", dbus_instruction);
				}).Start ();
			}
			return null;
		}
		
	}
}

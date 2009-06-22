// PuttyAction.cs
//
// Copyright Karol Będkowski 2008
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

using System;
using System.Diagnostics;
using System.Collections.Generic;

using Mono.Addins;

using Do.Universe;

namespace Putty
{

	/// <summary>
	/// PuTTY action - open saved session or entered host.
	/// </summary>
	public class PuttyAction: Act
	{
		
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Connect with PuTTY"); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Create new conenction with PuTTY"); }
		}
		
		public override string Icon {
			get { return "network-server"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
            get {
				yield return typeof (ITextItem);
				yield return typeof (PuttySession);
			}
		}

		void StartPuttySession (string session)
		{
			Process.Start ("putty", "-load " + session);
		}
		
		void ConnectToHost (string session)
		{
			Process.Start ("putty", session);
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			foreach (Item item in items) {
				 if (item is ITextItem)
	                ConnectToHost ((item as ITextItem).Text);              
	            else if (item is PuttySession)
	                StartPuttySession ((item as PuttySession).Session);
			}
			yield break;
		}
	}
}

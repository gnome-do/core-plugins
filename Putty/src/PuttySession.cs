// PuttySession.cs
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
using System.Collections.Generic;

using Mono.Addins;

using Do.Universe;

namespace Putty {
	
	/// <summary>
	/// PuTTY session.
	/// </summary>
	public class PuttySession: Item {
		private string name, session, host;
		
		public PuttySession (string session, string host) {
			this.session = session;
			this.name = System.Web.HttpUtility.UrlDecode(session);
			//this.name = session.Replace("%20", " ");
			this.host = host;
		}
		
		public override string Name {
			get { return name; }
		}

		public override string Description {
			get { return String.Format (AddinManager.CurrentLocalizer.GetString ("Start new PuTTY session (host {0})"), host); }
		}
		
		public string Session {
			get { return session; }
		}

		public override string Icon {
			get { return "gnome-globe";	}
		}
	
	}
}

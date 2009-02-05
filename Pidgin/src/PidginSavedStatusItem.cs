// PidginSavedStatusItem.cs
//
// GNOME Do is the legal property of its developers, whose names are too
// numerous to list here.  Please refer to the COPYRIGHT file distributed with
// this source distribution.
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
using System.Text.RegularExpressions;

using Do.Universe;

namespace PidginPlugin
{

	public sealed class PidginSavedStatusItem : Item
	{

		int status, id;
		string name, message, iconBase;

		public PidginSavedStatusItem (string name, string message, int id, int status)
		{
			this.name = name;
			this.message = message;
			this.status = status;
			this.id = id;
			this.iconBase = "/usr/share/pixmaps/pidgin/status/48/";
		}
		
		public override string Name {
			get { return name; }
		}
		
		public override string Description {
			get { return StripHTML (message); }
		}

		public int Status {
			get { return status; }
		}

		public int ID {
			get { return id; }
		}

		public override string Icon { 
			get  { 
				switch (status) {
				case 2: return iconBase + "available.png";
				case 3: return iconBase + "busy.png";
				//there is not a 48px invisible icon.
				case 4: return "/usr/share/pixmaps/pidgin/status/32/invisible.png";
				case 5: return iconBase + "away.png";
				default: return "pidgin";
				}
			}
		}
		
		string StripHTML (string message)
		{
			return Regex.Replace(message, @"<(.|\n)*?>", string.Empty);
		}
	}

}

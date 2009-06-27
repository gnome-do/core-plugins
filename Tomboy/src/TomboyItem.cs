// NoteItem.cs
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

using Mono.Addins;

using Do.Universe;

namespace Tomboy
{

	public class NoteItem : Item, IOpenableItem
	{

		string title;
		
		/// <summary>
		/// The Tomboy Item only has one property, the title
		/// </summary>
		/// <param name="note_title">
		/// A <see cref="System.String"/>
		/// </param>
		public NoteItem (string title)
		{
			this.title = title;
		}
		
		public override string Name {
			get { return title; }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Tomboy note"); } 
		}
		
		public override string Icon {
			get { return "tomboy"; }
		}
		
		/// <summary>
		/// This is because we implement IOpenableItem
		/// We use this method to run this instance of the item
		/// </summary>
		public void Open ()
		{
			// This action will start Tomboy if it is not
			// already running.
			new TomboyDBus ().OpenNote (title);
		}
	}
}

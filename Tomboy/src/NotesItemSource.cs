// NotesItemSource.cs
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
using System.Collections;
using System.Collections.Generic;

using Mono.Addins;

using Do.Universe;

namespace Tomboy
{	

	public class NotesItemSource : ItemSource
	{

		private List<Item> notes;

		/// <summary>
		/// When creating an instance of this item source get the initial
		/// list of tomboy notes
		/// </summary>
		public NotesItemSource ()
		{
			notes = new List<Item> ();			
		}
		
		/// <summary>
		/// This name is used during startup to descript the name 
		/// of this item source parser
		/// </summary>
		/// <param name="item">
		/// A <see cref="Item"/>
		/// </param>
		/// <returns>
		/// A <see cref="ICollection`1"/>
		/// </returns>
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Tomboy Notes"); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Loads Tomboy notes for searching."); }
		}
		
		public override string Icon {
			get { return "tomboy"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { 
				yield return typeof (NoteItem); 
				yield return typeof (IApplicationItem);
				yield return typeof (TomboyBrowseNotesItem);
			}
		}

		/// <summary>
		/// The Items in this case is the list of notes we've loaded
		/// </summary>
		/// <param name="item">
		/// A <see cref="Item"/>
		/// </param>
		/// <returns>
		/// A <see cref="ICollection`1"/>
		/// </returns>
		public override IEnumerable<Item> Items {
			get { return notes; }
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			if (IsTomboy (item)) {
				yield return new TomboyBrowseNotesItem ();
			} else if (item is TomboyBrowseNotesItem) {
				foreach (NoteItem note in Items)
					yield return note;
			}
		}
		
		bool IsTomboy (Item item) 
		{
			return item.Equals (Do.Platform.Services.UniverseFactory.MaybeApplicationItemFromCommand ("tomboy"));
		}
		
		/// <summary>
		/// This method run in the constructor to find the notes we can get
		/// ahold of.
		/// </summary>
		public override void UpdateItems ()
		{
			TomboyDBus tb = new TomboyDBus();
			// Only query Tomboy if it is already running.
			// Do not start it prematurely.
			if (!tb.Connected && !tb.NotesUpdated)
				return;
				
			notes.Clear ();
			foreach(string title in tb.GetAllNoteTitles ())
				notes.Add (new NoteItem (title));
			tb.NotesUpdated = false;
		}
	}
}

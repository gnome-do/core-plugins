//  NotesItemSource.cs
//
//  GNOME Do is the legal property of its developers, whose names are too
//  numerous to list here.  Please refer to the COPYRIGHT file distributed with
//  this source distribution.
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
using System.Collections;
using System.Collections.Generic;

using Do.Universe;

namespace Tomboy
{	
	public class NotesItemSource : IItemSource
	{
		private const string name = "Tomboy Note Indexer";
		private const string desc = "Loads up Tomboy notes for searching";
		private const string icon = "tomboy";
		private static Type [] supportedTypes = new Type [] {
			typeof (NoteItem) };
		
		private List<IItem> notes;

		/// <summary>
		/// When creating an instance of this item source get the initial
		/// list of tomboy notes
		/// </summary>
		public NotesItemSource ()
		{
			notes = new List<IItem> ();			
		}
		
		/// <summary>
		/// This name is used during startup to descript the name 
		/// of this item source parser
		/// </summary>
		/// <param name="item">
		/// A <see cref="IItem"/>
		/// </param>
		/// <returns>
		/// A <see cref="ICollection`1"/>
		/// </returns>
		public string Name {
			get {
				return name;
			}
		}
		
		public string Description {
			get {
				return desc;
			}
		}
		
		public string Icon {
			get {
				return icon;
			}
		}
		
		public Type[] SupportedItemTypes {
			get {
				return supportedTypes;
			}
		}

		/// <summary>
		/// The Items in this case is the list of notes we've loaded
		/// </summary>
		/// <param name="item">
		/// A <see cref="IItem"/>
		/// </param>
		/// <returns>
		/// A <see cref="ICollection`1"/>
		/// </returns>
		public ICollection<IItem> Items {
			get { return notes; }
		}
		
		public ICollection<IItem> ChildrenOfItem (IItem item)
		{
			return null;
		}
		
		/// <summary>
		/// This method run in the constructor to find the notes we can get
		/// ahold of.
		/// </summary>
		public void UpdateItems ()
		{
			TomboyDBus tb = new TomboyDBus();
			// Only query Tomboy if it is already running.
			// Do not start it prematurely.
			if (!tb.Connected)
				return;
			foreach(string title in tb.GetAllNoteTitles ())
				notes.Add (new NoteItem (title));
		}
	}
	
}

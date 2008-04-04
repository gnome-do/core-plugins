//  TomboyItems.cs
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
using System.Collections;
using System.Collections.Generic;

using Do.Universe;

namespace Tomboy
{	
	public class TomboyItemSource : IItemSource
	{
		List<IItem> notes;	

		/// <summary>
		/// When creating an instance of this item source get the initial
		/// list of tomboy notes
		/// </summary>
		public TomboyItemSource ()
		{
			notes = new List<IItem> ();			
			UpdateItems ();
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
				return "Tomboy Note Indexer";
			}
		}
		
		public string Description {
			get {
				return "Loads up tomboy notes for searching";
			}
		}
		
		public string Icon {
			get {
				return "tomboy";
			}
		}
		
		public Type[] SupportedItemTypes {
			get {
				return new Type[] {
					typeof (TomboyItem),
				};
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
			get {
				return notes;
			}
		}
		
		public ICollection<IItem> ChildrenOfItem (IItem item)
		{
			return null;
		}
		
		/// <summary>
		/// This method run in the constructor to find the notes we can get a hold of
		/// </summary>
		public void UpdateItems ()
		{			
			try {
				TomboyDBus tb = new TomboyDBus();
				foreach(string title in tb.GetAllNoteTitles ()) {
					notes.Add (new TomboyItem (title));
				}
			} catch (Exception e) {
				Console.Error.WriteLine ("Cannot index Tomboy Notes: {0}", e.Message);
			}
		}
	}
	
}

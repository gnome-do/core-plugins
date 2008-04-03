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
using System.Threading;

using Do.Addins;
using Do.Universe;

namespace Do.Addins.Tomboy
{
	
	public class TomboyItem : IOpenableItem
	{
		string title;
		long changed_date;
		/// <summary>
		/// The Tomboy Item only has one property, the title
		/// </summary>
		/// <param name="note_title">
		/// A <see cref="System.String"/>
		/// </param>
		public TomboyItem(TomboyItemSource.NoteStruct note)
		{
			this.title = note.title;
			this.changed_date = note.changed_date;
		}
		
		public string Name { get { return title; } }
		public string Description {
			get { 
				// This is an example of a UNIX timestamp for the date/time 11-04-2005 09:25.
				// First make a System.DateTime equivalent to the UNIX Epoch.
				System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);

				// Add the number of seconds in UNIX timestamp to be converted.
				dateTime = dateTime.AddSeconds(changed_date);

				// The dateTime now contains the right date/time so to format the string,
				// use the standard formatting methods of the DateTime object.
				string printDate = dateTime.ToShortDateString() +" "+ dateTime.ToShortTimeString();

				// Print the date and time
				string desc = "Last changed at: " + printDate; 
				return desc; 
			} 
		}
		public string Icon { get { return "tomboy"; } }
		
		/// <summary>
		/// This is because we implement IRunnableItem
		/// We use this method to run this instance of the item
		/// </summary>
		public void Open () {
			TomboyDBus tomboy_instance = new TomboyDBus();
			tomboy_instance.OpenNote(title);
		}
	}
}

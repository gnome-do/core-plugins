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
using NDesk.DBus;
using org.freedesktop.DBus;
using Do.Addins;

namespace Do.Addins.Tomboy
{
	[Interface("org.gnome.Tomboy.RemoteControl")]
	public interface ITBoy
	{
	    string CreateNote();
		string CreateNamedNote(string linked_title);
		bool DisplayNote(string uri);
		void DisplaySearch();	
		string FindNote(string title);
		long GetNoteChangeDate(string uri);
		string GetNoteTitle(string uri);
		string[] ListAllNotes();
		void DisplaySearchWithText(string search_text);
		string Version();
	}
	
	class TomboyDBus	
	{
		private const string OBJECT_PATH = "/org/gnome/Tomboy/RemoteControl";
		private const string BUS_NAME = "org.gnome.Tomboy";
		
		private static ITBoy TomboyInstance = null;

		static private ITBoy FindInstance() {
	        if (!Bus.Session.NameHasOwner(BUS_NAME))
	            throw new Exception(String.Format("Name {0} has no owner", BUS_NAME));

	        return Bus.Session.GetObject<ITBoy>(BUS_NAME, new ObjectPath (OBJECT_PATH));
	    }
		
		public TomboyDBus() {
			
			try {
	            TomboyInstance = FindInstance();
	        } catch(Exception) {
	            Console.Error.WriteLine("Could not locate Tomboy on D-Bus. Perhaps it's not running?");
	        }
	        
	        BusG.Init();
			//Console.WriteLine("Tomboy Version: {0}", TomboyInstance.Version());			
		}
		
		public ArrayList GetAllNoteTitles() {
			string[] AllNotes = null;
			AllNotes = TomboyInstance.ListAllNotes();
			ArrayList AllNoteTitles = new ArrayList();
			
			foreach(string Uri in AllNotes) {
				AllNoteTitles.Add(TomboyInstance.GetNoteTitle(Uri));
			}
			
			return AllNoteTitles;
		}
		
		public long GetNoteChangedDate(string note_title) {
			string note_uri = TomboyInstance.FindNote(note_title);
			try {
				return TomboyInstance.GetNoteChangeDate(note_uri);
			} catch (Exception) {
	            Console.Error.WriteLine("Could not find changed date for: {0}", note_title);
			}
			return 0;
		}
		
		public void OpenNote(string note_title) {
			string note_uri = TomboyInstance.FindNote(note_title);
			try {
				TomboyInstance.DisplayNote(note_uri);
			} catch (Exception) {
	            Console.Error.WriteLine("Could not open the note: {0}", note_title);
			}
		}
		
		/// <summary>
		/// Currently not used
		/// </summary>
		public void OpenSearch() {
			TomboyInstance.DisplaySearch();
		}

		/// <summary>
		/// If there is no title, just create new note then use this method
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		public string CreateNewNote() {
			string uri = TomboyInstance.CreateNote();
			TomboyInstance.DisplayNote(uri);
			return uri;
		}
		
		/// <summary>
		/// If a title is entered then create a new note with this title
		/// </summary>
		/// <param name="note_title">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		public string CreateNewNote(string note_title) {
			string uri = TomboyInstance.CreateNamedNote(note_title);
			TomboyInstance.DisplayNote(uri);
			return uri;
		}
		
		/// <summary>
		/// Open the Tomboy search window with the search string entered
		/// </summary>
		/// <param name="search_text">
		/// A <see cref="System.String"/>
		/// </param>
		public void SearchNotes(string search_text) {
			TomboyInstance.DisplaySearchWithText(search_text);
		}
		
	}
}

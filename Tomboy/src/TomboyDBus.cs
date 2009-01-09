//  TomboyDBus.cs
//
//  GNOME Do is the legal property of its developers, whose names are too numerous
//  to list here.  Please refer to the COPYRIGHT file distributed with this
//  source distribution.
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
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

namespace Tomboy
{

	public delegate void DBusNoteAddedHandler (string note);
	public delegate void DBusNoteRemovedHandler (string note);
	
	[Interface ("org.gnome.Tomboy.RemoteControl")]
	public interface ITBoy
	{
	    string CreateNote ();
		string CreateNamedNote (string linked_title);
		bool DisplayNote (string uri);
		void DisplaySearch ();	
		string FindNote (string title);
		long GetNoteChangeDate (string uri);
		string GetNoteTitle (string uri);
		string[] ListAllNotes ();
		void DisplaySearchWithText (string search_text);
		string Version ();
		bool SetNoteContents (string uri, string text_contents);
		event DBusNoteAddedHandler NoteAdded;
		event DBusNoteRemovedHandler NoteRemoved;
	}
		
	public delegate void NoteAddedHandler (object o, EventArgs e);
	public delegate void NoteRemovedHandler (object o, EventArgs e);
	
	class TomboyDBus	
	{
		#region Constants
		
		private const string OBJECT_PATH = "/org/gnome/Tomboy/RemoteControl";
		private const string BUS_NAME = "org.gnome.Tomboy";
		
		#endregion
		
		#region Static Constructor, Methods, and Fields
		
		private static ITBoy TomboyInstance = null;
		
		static TomboyDBus ()
		{
			// Listen for coming/going of Tomboy service
			org.freedesktop.DBus.IBus sessionBus = Bus.Session.GetObject<org.freedesktop.DBus.IBus> ("org.freedesktop.DBus", new ObjectPath ("/org/freedesktop/DBus"));
			sessionBus.NameOwnerChanged += OnDBusNameOwnerChanged;
		}
		
		private static void OnDBusNameOwnerChanged (string serviceName, string oldOwner, string newOwner)
		{
			if (serviceName != BUS_NAME)
				return;
			
			if (oldOwner == null && newOwner.Length > 0)
				// Service has started
				SetInstance ();
			else
				// Service has ended
				TomboyInstance = null;
		}
				
		private static void SetInstance ()
		{
			TomboyInstance = Bus.Session.GetObject<ITBoy> (BUS_NAME, new ObjectPath (OBJECT_PATH));
		}
		
		#endregion
		
		#region Instance Constructor, Methods, and Properties
		
		public event NoteAddedHandler NoteAdded;
		public event NoteRemovedHandler NoteRemoved;
		
		public TomboyDBus () {
			
			try {
				BusG.Init ();
				if (TomboyInstance == null)
					FindInstance ();
				TomboyInstance.NoteAdded += OnNoteAdded;
				TomboyInstance.NoteRemoved += OnNoteRemoved;
				
			} catch (Exception) {
				Console.Error.WriteLine ("Could not locate Tomboy on D-Bus. Perhaps it's not running?");
			}
		}
		
		private bool notes_updated = true;
		public bool NotesUpdated {
			get { return notes_updated; }
			set { notes_updated = value; }
		}
		
		protected virtual void OnNoteAdded (string note)
		{
			NotesUpdated = true;
		}
		
		protected virtual void OnNoteRemoved (string note)
		{
			NotesUpdated = true;
		}

		private void FindInstance ()
		{
			if (Bus.Session.NameHasOwner (BUS_NAME))
				SetInstance ();
		}
		
		private void EnsureTomboyInstance ()
		{
			if (!Connected) {
				Bus.Session.StartServiceByName (BUS_NAME);
				SetInstance ();
			}
		}
		
		public bool Connected
		{
			get {
				return TomboyInstance != null;
			}
		}
		
		public ArrayList GetAllNoteTitles () {
			if (!Connected)
				return new ArrayList ();
			string[] AllNotes = null;
			AllNotes = TomboyInstance.ListAllNotes ();
			ArrayList AllNoteTitles = new ArrayList ();
			
			foreach (string Uri in AllNotes) {
				AllNoteTitles.Add (TomboyInstance.GetNoteTitle (Uri));
			}
			
			return AllNoteTitles;
		}
		
		public void OpenNote (string note_title) {
			EnsureTomboyInstance ();	
			string note_uri = TomboyInstance.FindNote (note_title);
			try {
				TomboyInstance.DisplayNote (note_uri);
			} catch  (Exception) {
	            Console.Error.WriteLine ("Could not open the note: {0}", note_title);
			}
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
		public string CreateNewNote (string note_title, string note_content) {
			EnsureTomboyInstance ();
			
			string uri = string.Empty;
			
			if (string.IsNullOrEmpty (note_title) &&
			    (!TomboyConfiguration.DeriveTitle || string.IsNullOrEmpty (note_content))) {
				uri = TomboyInstance.CreateNote ();
			} else {
				// Generate a title from the content if no title
				// is provided and prefs say to do so.
				if (string.IsNullOrEmpty (note_title) &&
				    TomboyConfiguration.DeriveTitle &&
				    !string.IsNullOrEmpty (note_content)) {
					note_title = note_content.Trim ().Split ('\n') [0];
					if (note_title.Length > 20)
						note_title = note_title.Substring (0, 18) + "...";
				}
				
				uri = TomboyInstance.CreateNamedNote (note_title);
				int i = 2;
				// In the case of a duplicate note title, append
				// "(i)", increasing i until a unique title is
				// found.  In the future, support for appending
				// content to an existing note would be cool.
				while (string.IsNullOrEmpty (uri)) {
					string title_format = "{0} ({1})";
					uri = TomboyInstance.CreateNamedNote (string.Format (title_format, note_title, i));
					i++;
				}
			}
			
			note_title = TomboyInstance.GetNoteTitle (uri);
			TomboyInstance.DisplayNote (uri);
			
			if (!string.IsNullOrEmpty (note_content))
				TomboyInstance.SetNoteContents (uri, note_title + "\n\n" + note_content);
			return uri;
		}
		
		/// <summary>
		/// Open the Tomboy search window with the search string entered
		/// </summary>
		/// <param name="search_text">
		/// A <see cref="System.String"/>
		/// </param>
		public void SearchNotes (string search_text) {
			EnsureTomboyInstance ();
			TomboyInstance.DisplaySearchWithText (search_text);
		}		
		#endregion
	}
}

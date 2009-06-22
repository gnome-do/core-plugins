// PuttySessionItemSource.cs
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
using System.IO;
using System.Collections.Generic;

using Mono.Addins;

using Do.Universe;
using Do.Platform;

namespace Putty
{
	/// <summary>
	/// Item source - saved PuTTY sessions (loaded from ~/.putty/sessions/*).
	/// </summary>
	public class PuttySessionItemSource: ItemSource {		
		List<Item> items;
		
		public override string Name {
			get {return AddinManager.CurrentLocalizer.GetString("PuTTY sessions"); }
		}

		public override string Description {
			get {return AddinManager.CurrentLocalizer.GetString("PuTTY saved sessions");}
		}

		public override string Icon {
			get {return "putty";}
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (PuttySession); }
		}

		public override IEnumerable<Item> Items	{
			get {return items; }
		}
		
		public PuttySessionItemSource ()
		{
			items = new List<Item> ();
		}

		public override void UpdateItems ()
		{
			string home = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			string sessions_dir = Path.Combine (home, ".putty/sessions");
			
			items.Clear ();
			// check files for "HostName" (discards "Default Settings" and other files)
			foreach (string file in Directory.GetFiles (sessions_dir)) {
				try {
					using (StreamReader reader = File.OpenText (file)) {
						string line;
						while ((line = reader.ReadLine ()) != null) {
							if (line.StartsWith ("HostName=")) { // simpler than re
								// hostname is a part of line after =
								items.Add (new PuttySession (Path.GetFileName (file), line.Substring ("HostName=".Length)));
								break;
							}
						}
					}					
				} catch (Exception e) {
					Log.Error ("PuttySessionItemSource error; file={0}, error={1}", file, e.Message);
					Log.Debug (e.StackTrace);
				}			
			}
		}
	}
}

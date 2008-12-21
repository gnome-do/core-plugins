/* FileItemActions.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Collections.Generic;
using Mono.Unix;

using Do.Universe;

namespace FilePlugin {
	class MoveToTrashAction : Act {

		public override string Name {
			get {
				return Catalog.GetString ("Move to Trash");
			}
		} 

		public override string Description {
			get {
				return Catalog.GetString ("Moves a file or folder to the trash.");
			}
		} 

		public override string Icon {
			get {
				return "user-trash-full";
			}
		} 

		string Trash {
			get { 
				return Do.Paths.Combine (
						Do.Paths.ReadXdgUserDir ("XDG_DATA_HOME", ".local/share"),
						"Trash/files/");
			}
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type [] {
					typeof (FileItem),
				};
			}
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			List<string> seenPaths;

			seenPaths = new List<string> ();
			foreach (FileItem src in items) {
				if (seenPaths.Contains (src.Path)) continue;
				try {
					File.Move (src.Path, Trash + "/" + src.Name);
					seenPaths.Add (src.Path);
					src.Path = Path.Combine (Trash, Path.GetFileName (src.Path));
				} catch (Exception e) {
					Console.Error.WriteLine ("MoveToTrashAction could not move "+
							src.Path + " to the trash: " + e.Message);
				}
			}
			return null;
		}
	}
}
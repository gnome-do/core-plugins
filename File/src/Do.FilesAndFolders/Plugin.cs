/* Plugin.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this source distribution.
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

using Do.Platform;

namespace Do.FilesAndFolders
{

	static class Plugin
	{
		public static FolderIndex FolderIndex { get; private set; }
		public static FilesAndFoldersPreferences Preferences { get; private set; }
		public static ImportantFolders ImportantFolders { get; private set; }
		
		static Plugin ()
		{
			try {
				FolderIndex = new FolderIndex ();
				Preferences = new FilesAndFoldersPreferences ();
				ImportantFolders = new ImportantFolders ();
			} catch (Exception e) {
				Log.Error (e.Message); Log.Debug (e.StackTrace);
			}
		}
	}
}

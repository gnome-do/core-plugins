/* Preferences.cs
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
using System.Linq;
using System.Collections.Generic;

using Do.Platform;

namespace Do.FilesAndFolders
{
	
 	class FilesAndFoldersPreferences
	{

		#region Preference keys and default values.
		const string IncludeHiddenFilesKey = "IncludeHiddenFiles";
		const string IncludeHiddenFilesWhenBrowsingKey = "IncludeHiddenFilesWhenBrowsing";
		const string MaximumFilesIndexedKey = "MaximumFilesIndexed";

		const bool IncludeHiddenFilesDefaultValue = false;
		const bool IncludeHiddenFilesWhenBrowsingDefaultValue = true;
		const int MaximumFilesIndexedDefaultValue = 3000;
		#endregion

		IPreferences Prefs { get; set; }
		
		public bool IncludeHiddenFiles {
			get { return Prefs.Get (IncludeHiddenFilesKey, IncludeHiddenFilesDefaultValue); }
			set { Prefs.Set (IncludeHiddenFilesKey, value); }
		}

		public bool IncludeHiddenFilesWhenBrowsing {
			get { return Prefs.Get (IncludeHiddenFilesWhenBrowsingKey, IncludeHiddenFilesWhenBrowsingDefaultValue); }
			set { Prefs.Set (IncludeHiddenFilesWhenBrowsingKey, value); }
		}

		public int MaximumFilesIndexed {
			get { return Prefs.Get (MaximumFilesIndexedKey, MaximumFilesIndexedDefaultValue); }
			set { Prefs.Set (MaximumFilesIndexedKey, value); }
		}
		
		public FilesAndFoldersPreferences ()
		{
			Prefs = Services.Preferences.Get<FilesAndFoldersPreferences> ();
		}
		
	}
}

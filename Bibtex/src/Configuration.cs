/* Configuration.cs
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
using Do.Platform;

namespace Bibtex
{

	[System.ComponentModel.Category("Bibtex")]
	[System.ComponentModel.ToolboxItem(true)]
	public partial class Configuration : Gtk.Bin
	{
        private static IPreferences prefs;
		
		public Configuration()
		{
			this.Build();

			chooseBibtexFileButton.SetFilename(BibtexFilePath);
			chooseDocsFolderButton.SetCurrentFolder(DocumentLibrary);
		}
		
		static Configuration(){
			prefs = Do.Platform.Services.Preferences.Get<Bibtex.Configuration>();
		}

        public static string BibtexFilePath {
			get { return prefs.Get<string>("bibtexfilepath", System.IO.Path.Combine (System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "bibtex.bib")); }
			set { prefs.Set <string> ("bibtexfilepath", value); }
		}
		
		public static string DocumentLibrary {
			get { return prefs.Get<string>("documentlibrary", System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments)); }
			set { prefs.Set<string>("documentlibrary", value); }
		}

		protected virtual void OnChooseBibtexFileButtonSelectionChanged 
			(object sender, System.EventArgs e)
		{
			Gtk.FileChooserButton window = (Gtk.FileChooserButton)sender;			
            BibtexFilePath = window.Filename;
		}

		protected virtual void OnChooseDocsFolderButtonSelectionChanged 
			(object sender, System.EventArgs e)
		{
			Gtk.FileChooserButton window = (Gtk.FileChooserButton)sender;			
            DocumentLibrary = window.Filename;
		}
	}
}

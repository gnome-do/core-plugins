// Claws.cs 
// User: Karol Będkowski at 15:36 2008-11-09
//
//Copyright Karol Będkowski 2008
//
//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using Do.Platform;
using Do.Universe;


namespace Claws {
		
	public class Claws {

		const string ClawsHome = ".claws-mail";
		const string ClawsAddrBookIndex = "addrbook--index.xml";
		
		/// <summary>
		/// Get address book files from Claws config.
		/// </summary>
		/// <returns>
		/// A <see cref="List`1"/>
		/// </returns>
		public static List<string> GetAddressBookFiles() {
			List<string> result = new List<string> ();		
			string clawsDir =  Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), ClawsHome); // I need clawsDir below
			string indexFile = Path.Combine (clawsDir, ClawsAddrBookIndex);

			//Log.Debug("claws path {0}, {1}", clawsDir, indexFile);

			if (!System.IO.File.Exists (indexFile)) {
				return result;
			}
			
			try {
				// open $HOME/.claws-mail/addrbook--index.xml
				XmlDocument xmldoc = new XmlDocument ();
				xmldoc.Load (indexFile);
				XmlNode adressbook = xmldoc.SelectSingleNode ("addressbook");
				XmlNode booklist = adressbook.SelectSingleNode ("book_list");
				// files contains address book
				XmlNodeList books = booklist.SelectNodes ("book");				
				foreach (XmlNode book in books) {
					string file = book.Attributes["file"].InnerText;
					if (String.IsNullOrEmpty (file)) {
						continue;
					}

					string file_path = Path.Combine (clawsDir, file);
					if (File.Exists (file_path)) {
						result.Add (file_path);
					}
				}				
			} catch (Exception e) {
				Log.Error("Claws.GetAddressBookFiles error: {0}", e.Message);
				Log.Debug("Claws.GetAddressBookFiles error: {0}", e.StackTrace);
			}
			
			return result;
		}
		
	}
}

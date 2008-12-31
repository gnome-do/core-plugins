// Zim.cs 
// User: Karol Będkowski at 18:23 2008-10-21
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

using Do.Platform;

namespace Zim
{
		
	public class Zim {
		
		/// <summary>
		/// Load Zim reposytories (from ~/.config/zim/notebooks.list)
		/// </summary>
		/// <returns>
		/// Dict "repository name" -> "repository path" <see cref="Dictionary`2"/>
		/// </returns>
	
		public static Dictionary<string, string> LoadNotebooks () {
			string home = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			string path = Path.Combine (home, ".config/zim/notebooks.list");			
			Dictionary<string, string> result = new Dictionary<string,string> ();
			
			if (!File.Exists (path)) {
				return result;
			}
				
			try {
				using (StreamReader reader = File.OpenText (path)) {
					string line;					
					while ((line = reader.ReadLine ()) != null) {
						string[] repo = line.Split(":=\t".ToCharArray ());
						if (repo.Length != 2) {
							continue;
						}
						
						if (repo[0] != "_default_") {
							string repopath = repo[1].Replace ("~", home);
							result.Add (repo[0], repopath);
						}
					}					
				}					
			} catch (Exception e) {
				Log.Error ("LoadNotebooks error; error={0}", e.StackTrace);
			}
			
			return result;
		}
	}
}

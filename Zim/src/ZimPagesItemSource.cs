// ZimPagesItemSource.cs 
// User: Karol Będkowski at 17:36 2008-10-19
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

using Mono.Unix;

using Do.Universe;

namespace Zim
{
	
	/// <summary>
	/// Indexes Zim pages in all repositories
	/// </summary>
	public class ZimPagesItemSource: ItemSource {
			
		List<Item>	items;
		
		public override string Name {
			get { return Catalog.GetString("Zim pages"); }
		}

		public override string Description {
			get { return Catalog.GetString("Zim Desktop Wiki pages"); }
		}

		public override string Icon {
			get { return "zim";	}
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof(ZimPage); }
		}

		public override IEnumerable<Item> Items {
			get { return items; }
		}
		
		public ZimPagesItemSource ()
		{
			items = new List<Item>();
		}

		public override void UpdateItems () {			
			items.Clear();
	
			Dictionary<string, string> repos = Zim.LoadNotebooks ();
			foreach (string key in repos.Keys) {
				if (Directory.Exists(repos[key])) {
					foreach (string file in FindFilesInRepository(repos [key])) {
						ZimPage page = new ZimPage(file.Substring(0, file.Length-4).Replace("/", ":"), key);
						items.Add(page);
					}
				}		
			}
			
		}

		/// <summary>
		/// Find all *.txt files in given path. 
		/// </summary>
		/// <param name="path">
		/// Path to search <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// File names list <see cref="List`1"/>
		/// </returns>
		private List<string> FindFilesInRepository(string path) {
			int	pathLen = path.Length;
			List<string> fileResults = new List<string>();
			Stack<string> directoryStack = new Stack<string>();
			directoryStack.Push(path);
			
			while (directoryStack.Count > 0) {
				string currentDir = directoryStack.Pop();
				
				foreach (string fileName in Directory.GetFiles(currentDir, "*.txt")) {
                	fileResults.Add(fileName.Substring(pathLen));
            	}

	            foreach (string directoryName in Directory.GetDirectories(currentDir)) {	            	
	            	if (!directoryName.Substring(directoryName.LastIndexOf("/")+1).StartsWith(".")) {
                		directoryStack.Push(directoryName);
                	}
            	}
        	}
        	
        	return fileResults;
		}
	}
}

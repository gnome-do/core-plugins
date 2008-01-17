//  OpenSearchFileManager.cs
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

using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Do.Plugins.OpenSearch
{
	public static class OpenSearchFileManager
	{
		private static readonly IOpenSearchFileProvider openSearchFileProvider;
		private static readonly string validFilePattern;
		
		static OpenSearchFileManager()
		{
			openSearchFileProvider = new FirefoxOpenSearchFileProvider ();
			validFilePattern = @"^.*\.xml$";
		}
		
		public static List<IOpenSearchFile> GetOpenSearchFiles ()
		{
			List<IOpenSearchFile> openSearchFiles = new List<IOpenSearchFile> ();
			
			foreach(string path in openSearchFileProvider.OpenSearchFilePaths)
			{
				try {
					if(!Directory.Exists(path))
						continue;
					
					string [] filePaths = Directory.GetFiles(path);
					foreach(string filePath in filePaths)
					{
						if (!Regex.IsMatch (filePath,validFilePattern))
							continue;
						
						switch (OpenSearchFileTypeIdentifier.Identify (filePath)) 
						{
						    case OpenSearchFileType.OpenSearch:
								openSearchFiles.Add(new OpenSearchFile (filePath));	
								break;
							case OpenSearchFileType.MozillaSearch:
								openSearchFiles.Add(new MozillaSearchFile (filePath));
								break;
							default:
								break;
						}
					}
				} catch {
					continue;
				}
			}
			
			return openSearchFiles;
		}		
	}
}

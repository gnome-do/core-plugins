//GoogleSearchResult.cs created with MonoDevelop
//Brian Lucas (bcl1713@gmail.com)
//sacul@irc.ubuntu.com/#gnome-do
// 
//GNOME Do is the legal property of its developers. Please refer to the
//COPYRIGHT file distributed with this
//source distribution.
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
//

using System;
using System.Text.RegularExpressions;

namespace InlineGoogleSearch
{
	
	
	public class GoogleSearchResult
	{
		public string unescapedUrl = "";
		public string url = "";
		public string visibleUrl = "";
		public string cacheUrl = "";
		public string title = "";
		public string titleNoFormatting = "";
		public string content = "";
		
		public GoogleSearchResult(string inString)
		{
			inString = inString.Remove(inString.LastIndexOf("\""));
			
			string[] array;
			array = Regex.Split(inString, "\",\"");
			
			int upperBound = array.GetLength(0);
			for (int i = 0; i < upperBound; i++){
				if (array[i].Contains("unescapedUrl\":\"")){
					this.unescapedUrl = array[i].Remove(0,15);
				}
				else if (array[i].Contains("url\":\"")){
					this.url = array[i].Remove(0,6);
				}
				else if (array[i].Contains("visibleUrl\":\"")){
					this.visibleUrl = array[i].Remove(0,13);
				}
				else if (array[i].Contains("cacheUrl\":\"")){
					this.cacheUrl = array[i].Remove(0,11);
				}
				else if (array[i].Contains("title\":\"")){
					this.title = array[i].Remove(0,8);
				}
				else if (array[i].Contains("titleNoFormatting\":\"")){
					this.titleNoFormatting = array[i].Remove(0,20);
				}
				else if (array[i].Contains("content\":\"")){
					this.content = array[i].Remove(0,10);
				}
			}
		}
	}
}

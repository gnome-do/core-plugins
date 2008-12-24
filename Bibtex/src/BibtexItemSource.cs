/* BibtexItemSource.cs
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
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using Do.Universe;
using Do.Platform.Linux;

namespace Do.Addins.Bibtex {

	public class BibtexItemSource : ItemSource, IConfigurable {
		
		Regex fileRegex = new Regex (@"file\s=\s\{(.*?)\}",
		                             RegexOptions.IgnoreCase
		                             | RegexOptions.Singleline);
		
		Regex jabRefFileRegex = new Regex (@".*?:(.*?):.*?",
		                             RegexOptions.IgnoreCase
		                             | RegexOptions.Singleline);	
		
		Regex authorRegex = new Regex (@"author\s+=\s+\{(.*?)\},",
									   RegexOptions.IgnoreCase
									   | RegexOptions.Singleline);
		
		//\b means that we wont match "booktitle" if it appears first in entry
		Regex titleRegex = new Regex (@"\btitle\s+=\s+\{(.*?)\},",
									  RegexOptions.IgnoreCase 
									  | RegexOptions.Singleline);

		Regex citekeyRegex = new Regex (@"@\w+\{(.*?),",
									  RegexOptions.IgnoreCase 
									  | RegexOptions.Singleline);
		
		Regex startOfEntryRegex = new Regex(@"^\s*@");
		Regex endOfEntryRegex = new Regex(@"^\s*\}");

		const int LOOK_FOR_ENTRY = 0;
		const int IN_ENTRY = 1;
		

		string bibtexLibraryFile;
		string bibtexDocumetsFolder;
		
		List<Item> items;

		public BibtexItemSource ()
		{
			items = new List<Item> ();
			UpdateItems ();
		}

		public override string Name { get { return "Bibtex Articles"; } }
		public override string Description { get { return "Indexes a Bibtex file."; } }
		public override string Icon { get { return "gnome-mime-document"; } }

		public override IEnumerable<Type> SupportedItemTypes {
			get {return new Type[] { typeof (BibtexItem) };}}

		public override IEnumerable<Item> Items { get { return items; }}

		public override IEnumerable<Item> ChildrenOfItem (Item parent)
		{
			return null;  
		}
		
		public override void UpdateItems ()
		{

			bibtexLibraryFile = Configuration.BibtexFilePath; 			
			bibtexDocumetsFolder = Configuration.DocumentLibrary;
			
			string currentBibtexEntry = null;
			string lineOfBibtexFile = null;
			
			string file = null;
			string author = null;
			string title = null;
			string citekey = null;
			string path = null;
			Match fileMatch = null;
			Match jabrefMatch = null;
			int mode = LOOK_FOR_ENTRY;
			
			items.Clear ();

			StreamReader sr = null;
			try {
				sr = File.OpenText (bibtexLibraryFile);
			}catch (Exception e) {
				Console.Error.WriteLine ("Could not read Bibtex file: "+ e.Message);
				return;
			}

			while ((lineOfBibtexFile = sr.ReadLine ()) != null){
				if (mode == LOOK_FOR_ENTRY 
					&& startOfEntryRegex.Match (lineOfBibtexFile).Success){

					mode = IN_ENTRY; //we want to have a look inside the entry
					citekey = citekeyRegex.Match (lineOfBibtexFile).Groups[1].Value;

				} else if (mode == IN_ENTRY
						   && endOfEntryRegex.Match (lineOfBibtexFile).Success){
																	
					mode = LOOK_FOR_ENTRY;
					//we have reached the end of an entry so we start looking 
					//for the start of the next one
					
					//first check if it has a "file" attribute
					fileMatch = fileRegex.Match (currentBibtexEntry);
					
					if (fileMatch.Success ){
						file = fileMatch.Groups[1].Value; //could just be a path
						jabrefMatch = jabRefFileRegex.Match( file); 
						if (jabrefMatch.Success) //extract the jabref path
							file = jabrefMatch.Groups[1].Value;
					} else {
						//skip the entry, no file is attached
						currentBibtexEntry = null;
						continue;
					} 				
					
					author = authorRegex.Match (currentBibtexEntry).Groups[1].Value;
					author = Regex.Replace(author, "\\s+"," ");
					//TODO subsitute all accents from here
					//http://www.bibtex.org/SpecialSymbols/
					//author = Regex.Replace(author, @"\{\\'i\}","í");
					
					title = titleRegex.Match (currentBibtexEntry).Groups[1].Value;
					title = Regex.Replace (title, @"\s+"," ");
					title = Regex.Replace (title, @"[\{,\}]","");
					
					path = Path.Combine (bibtexDocumetsFolder, file);
					              
					items.Add (new BibtexItem(title,
											  author,
					                          citekey, 
											  path));
					items.Add (new BibtexAuthorsItem(title,
													 author,
					                                 citekey, 
													 path));
					
					currentBibtexEntry = null;
				} else	currentBibtexEntry += lineOfBibtexFile + "\n";
			}
			sr.Close ();
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new Configuration ();
		}
	}
}

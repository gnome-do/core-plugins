/* BibtexItem.cs
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
using Do.Platform;
using Do.Universe;


namespace Bibtex
{
	public class BibtexItem : Item, IFileItem
	{
		string title;
		string authors;
		string citekey;
		string path;
		string icon;

		public BibtexItem (string title, string authors, string citekey, string path):
		base ()
		{
			this.title = title;
			this.authors = authors;
			this.citekey = citekey;
			this.path = path;
		}

		//Name is text under the icon
		public override string Name { get { return title; } }
		//Description is not searchable
		public override string Description { get { return authors; } }
		//Cite key is used for citations in latex documents
		virtual public string Citekey { get { return citekey; } }
		
		public string Path { get { return path; } }
		
		public string Uri {
			get { return "file://" + Path; }
		}
		
		public override string Icon {		
			get {
				if (null != icon) return icon;
				else
				{ 
					icon = Services.UniverseFactory.NewFileItem (path).Icon;
					return icon;
				}
			}
		}
	}
}

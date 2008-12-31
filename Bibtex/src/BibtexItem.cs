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
using Do.Universe.Linux;
using Do.Platform.Linux;


namespace Do.Addins.Bibtex
{
	public class BibtexItem : Item
	{
		string title;
		string authors;
		string citekey;

		public BibtexItem (string title, string authors, string citekey, string path):
		base ()
		{
			this.title = title;
			this.authors = authors;
			this.citekey = citekey;
		}

		//Name is text under the icon
		public override string Name { get { return title; } }
		//Des is text below the 2 icon displays, is not searchable
		public override string Description { get { return authors; } }
		//Cite key is used for citations in latex documents
		virtual public string Citekey { get { return citekey; } }
		//I dont know where this is displayed. :(
		public string Text { get { return title; } }
		
		public override string Icon {
			get{
				return "stock_dialog_warning";
			}
		}

	}
}

/* CompareAction.cs
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mono.Unix;

using Do.Universe;

namespace FilePlugin
{
	public class CompareAction : Act, IConfigurable
	{	
		public override string Name {
			get { return Catalog.GetString ("Compare with"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("See the differences between two text files"); }
		}
		
		public override string Icon {
			get { return "edit-copy"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type [] {
					typeof (IFileItem),
				};
			}
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get {
				return new Type [] {
					typeof (IFileItem),
				};
			}
		}
		
		public bool ModifierItemsOptional {
			get { return false; }
		}
		
		public bool SupportsItem (Item item)
		{
			if (!(item is IFileItem)) return false;
			return IsTextFile (item as IFileItem);
		}
		
		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			if (!(modItem is IFileItem)) return false;
			return IsTextFile (modItem as IFileItem);
		}
		
		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{
			return null;
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			if (String.IsNullOrEmpty (CompareConfig.DiffTool)) {
				Console.Error.WriteLine ("You must have a diff tool configured "
				 + "to use this action, please set a tool in the config dialog.");
				return null;
			}
			
			string file1 = (items.First () as IFileItem).Path;
			string file2 = (modItems.First () as IFileItem).Path;
			
			Process diff = new Process ();
			if (CompareConfig.RunInTerminal) {
				diff.StartInfo.FileName = CompareConfig.Terminal;
				diff.StartInfo.Arguments = CompareConfig.DiffTool + " "
					+ file1 + " " + file2;
			} else {
				diff.StartInfo.FileName = CompareConfig.DiffTool;
				diff.StartInfo.Arguments = file1 + " " + file2;
			}
			diff.Start ();
			
			return null;
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new CompareConfig ();
		}
		
		bool IsTextFile (IFileItem file)
		{
			return file.MimeType.StartsWith ("text/");
		}
		
	}
}
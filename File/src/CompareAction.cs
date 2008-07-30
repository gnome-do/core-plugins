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
using System.Diagnostics;
using Mono.Unix;
using Do.Addins;
using Do.Universe;

namespace FilePlugin
{
	public class CompareAction : IAction, IConfigurable
	{	
		public string Name {
			get { return Catalog.GetString ("Compare with"); }
		}
		
		public string Description {
			get { return Catalog.GetString ("See the differences between two text files"); }
		}
		
		public string Icon {
			get { return "edit-copy"; }
		}
		
		public Type [] SupportedItemTypes {
			get {
				return new Type [] {
					typeof (FileItem),
				};
			}
		}
		
		public Type [] SupportedModifierItemTypes {
			get {
				return new Type [] {
					typeof (FileItem),
				};
			}
		}
		
		public bool ModifierItemsOptional {
			get { return false; }
		}
		
		public bool SupportsItem (IItem item)
		{
			if (!(item is FileItem)) return false;
			return IsTextFile (item as FileItem);
		}
		
		public bool SupportsModifierItemForItems (IItem [] items, IItem modItem)
		{
			if (!(modItem is FileItem)) return false;
			return IsTextFile (modItem as FileItem);
		}
		
		public IItem [] DynamicModifierItemsForItem (IItem item)
		{
			return null;
		}
		
		public IItem [] Perform (IItem [] items, IItem [] modItems)
		{
			if (String.IsNullOrEmpty (CompareConfig.DiffTool)) {
				Console.Error.WriteLine ("You must have a diff tool configured "
				 + "to use this action, please set a tool in the config dialog.");
				return null;
			}
			
			string file1 = (items [0] as FileItem).Path;
			string file2 = (modItems [0] as FileItem).Path;
			
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
		
		bool IsTextFile (FileItem file)
		{
			return file.MimeType.StartsWith ("text/");
		}
		
	}
}
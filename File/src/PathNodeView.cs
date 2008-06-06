/* PathNodeView.cs
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

using Gtk;

namespace FilePlugin
{	
	//If someone can update this class to use spin buttons,
	//that would be fantastic.
	public class PathNodeView : NodeView
	{
		enum Column {
			Path = 0,
			Depth,
			NumColumns
		}
		
		private TextWriter writer;
		
		public PathNodeView () :
			base ()
		{
			CellRenderer cell;
			RulesHint = true;
			HeadersVisible = true;
			
			Model = new ListStore (typeof (string), typeof (string));
			
			cell = new CellRendererText ();
			(cell as CellRendererText).Edited += OnPathEdited;
			(cell as CellRendererText).Width = 310;
			(cell as CellRendererText).Editable = true;
			(cell as CellRendererText).Ellipsize = Pango.EllipsizeMode.Middle;
			AppendColumn ("Path", cell, "text", Column.Path);
			
			/*
			cell = new CellRendererSpin ();
			((CellRendererSpin) cell).Editable = true;
			((CellRendererSpin) cell).Mode = CellRendererMode.Editable;
			AppendColumn ("Depth", cell, "text", Column.Depth);
			*/

			cell = new CellRendererText ();
			(cell as CellRendererText).Edited += OnDepthEdited;
			(cell as CellRendererText).Editable = true;
			(cell as CellRendererText).Alignment = Pango.Alignment.Right;
			AppendColumn ("Depth", cell, "text", Column.Depth);
						
			AddPaths ();
		}
		
		/*
		private void SpinnerDataFunc (TreeViewColumn column, CellRenderer cell,
			TreeModel model, TreeIter iter)
		{
			CellRendererSpin renderer;

			renderer = (CellRendererSpin) cell;
			renderer.Editable = true;
			renderer.Text = Model.GetValue (iter, (int)Column.Depth) as string;
		}
		*/
		
		private void AddPaths ()
		{
			ListStore store;
			
			store = Model as ListStore;
			store.Clear ();
			
			try {
				string s;
				//SpinButton spin = new SpinButton (-1, 255, 1);
				StreamReader reader = File.OpenText(FileItemSource.ConfigFile);
				while ((s = reader.ReadLine ()) != null) {
					string path = s.Substring (0, s.LastIndexOf (":")).Trim ();
					string depth = s.Substring (s.LastIndexOf (":") + 2).Trim ();
					try {
						if (Int32.Parse (depth) < -1)
							depth = "0";
					} catch {
						depth = "0";
					}						
					store.AppendValues (path, depth);
				}
			} catch (Exception e) {
				Console.Error.WriteLine (e);
			}
		}
		
		protected void OnPathEdited (object o, Gtk.EditedArgs args)
		{
			TreeIter iter;
			ListStore store;
			string newText = FileConfigText ((int)Column.Path, args.NewText);
			
			store = Model as ListStore;
			store.GetIter (out iter, new Gtk.TreePath (args.Path));
			
			store.SetValue (iter, (int)Column.Path, newText);
			WriteConfig ();
		}
		
		protected void OnDepthEdited (object o, Gtk.EditedArgs args)
		{
			TreeIter iter;
			ListStore store;
			
			string newText = FileConfigText ((int)Column.Depth, args.NewText);
			
			store = Model as ListStore;
			store.GetIter (out iter, new Gtk.TreePath (args.Path));
			
			store.SetValue (iter, (int)Column.Depth, newText);
			WriteConfig ();
		}
		
		private string FileConfigText (int type, string text)
		{
			//reformat string to be a valid file path
			if (type == 0) {
				string home = Environment.GetEnvironmentVariable ("HOME");
				return text.Replace ("~",home);
			}
			//reformat string to be a valid int
			if (type == 1) {
				try {
					if (Int32.Parse (text) < -1)
						text = "0";
				} catch {
					text = "0";
				}
				return text;
			}
			return "ERROR";
		}
		
		public void WriteConfig ()
		{
			writer = new StreamWriter (FileItemSource.ConfigFile + ".copy", false);
			this.Model.Foreach (WriteConfigForeachFunc);
			writer.Close ();
			File.Delete (FileItemSource.ConfigFile);
			File.Move (FileItemSource.ConfigFile + ".copy", FileItemSource.ConfigFile);
		}
		
		public bool WriteConfigForeachFunc (TreeModel model, TreePath path, TreeIter iter)
		{
			string pathLine;
			
			pathLine  = model.GetValue (iter, (int)Column.Path) as string;
			pathLine += ": ";
			pathLine += model.GetValue (iter, (int)Column.Depth) as string;
			writer.WriteLine (pathLine);
			return false;
		}
	}
}
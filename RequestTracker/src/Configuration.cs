
using System;
using System.Web;
using Do.Platform;
using Do.Platform.Linux;
using Do.Universe;
using Do.Universe.Common;
using Mono.Unix;

namespace RequestTracker
{
	
	[System.ComponentModel.ToolboxItem(true)]
	public partial class RTPrefs : Gtk.Bin
	{
		Gtk.ListStore rtListStore;
		Gtk.TreeViewColumn nameColumn;
		Gtk.TreeViewColumn urlColumn;
		
		protected virtual void OnRemoveBtnClicked (object sender, System.EventArgs e)
		{
			Gtk.TreeModel model;
			Gtk.TreeIter iter;
			RTTree.Selection.GetSelected (out model, out iter);
			rtListStore.Remove (ref iter);
			
			UpdatePrefs ();
		}
		
		protected virtual void OnAddBtnClicked (object sender, System.EventArgs e)
		{
			Gtk.TreeIter iter = rtListStore.AppendValues ("", "");
			Gtk.TreePath path = rtListStore.GetPath (iter);
			RTTree.SetCursor (path, nameColumn, true);
		}
		
		protected virtual void OnNameCellEdited (object sender, Gtk.EditedArgs args)
		{
			OnCellEdited (sender, args, 0);
		}
		
		protected virtual void OnURLCellEdited (object sender, Gtk.EditedArgs args)
		{
			// Test the URL is valid
			try {
				new System.Uri (args.NewText);
			} catch (System.UriFormatException) {
				return;
			}
			
			OnCellEdited (sender, args, 1);
		}
		
		protected virtual void OnCellEdited (object sender, Gtk.EditedArgs args, int column)
		{
			Gtk.TreeIter iter;
			rtListStore.GetIter (out iter, new Gtk.TreePath (args.Path));
 
			rtListStore.SetValue (iter, column, args.NewText);

			UpdatePrefs ();
		}
		
		public void UpdatePrefs ()
		{
			Gtk.TreeIter iter;
			string URLs = "";
			string name;
			string url;
			int num_children = rtListStore.IterNChildren ();
			
			for (int i = 0; i < num_children; i++) {
				rtListStore.IterNthChild (out iter, i);
				
				name = rtListStore.GetValue (iter, 0).ToString ();
				url = rtListStore.GetValue (iter, 1).ToString ();
				
				if (!string.IsNullOrEmpty (name) && !string.IsNullOrEmpty (url)) {
					if (i > 0) {
						URLs += "|";
					}
					URLs += name.Replace ("|", "");
					URLs += "|";
					URLs += url.Replace ("|", "");
				}
			}
			
			RTPreferences prefs = new RTPreferences ();
			prefs.URLs = URLs;
		}
		
		public RTPrefs()
		{
			RTPreferences prefs = new RTPreferences();
			
			this.Build();
			
			nameColumn = new Gtk.TreeViewColumn ();
			nameColumn.Title = "Name";
			urlColumn = new Gtk.TreeViewColumn ();
			urlColumn.Title = "URL";
			
			RTTree.AppendColumn (nameColumn);
			RTTree.AppendColumn (urlColumn);
			
			rtListStore = new Gtk.ListStore (typeof (string), typeof (string));
			RTTree.Model = rtListStore;
 
			Gtk.CellRendererText nameNameCell = new Gtk.CellRendererText ();
			nameNameCell.Editable = true;
			nameNameCell.Edited += OnNameCellEdited;
			nameColumn.PackStart (nameNameCell, true);
			
			Gtk.CellRendererText urlTitleCell = new Gtk.CellRendererText ();
			urlTitleCell.Editable = true;
			urlTitleCell.Edited += OnURLCellEdited;
			urlColumn.PackStart (urlTitleCell, true);

			nameColumn.AddAttribute (nameNameCell, "text", 0);
			urlColumn.AddAttribute (urlTitleCell, "text", 1);
			
			if (!string.IsNullOrEmpty (prefs.URLs)) {
				string[] urlbits = prefs.URLs.Split('|');
				for (int i = 0; i < urlbits.Length; i++) {
					string name = urlbits[i];
					string uri = urlbits[++i];
					Uri url;
					try {
						url = new System.Uri(uri);
					} catch (System.UriFormatException) {
						continue;
					}
					
					rtListStore.AppendValues (name, url.ToString());
				}
			}
		}

	}
}

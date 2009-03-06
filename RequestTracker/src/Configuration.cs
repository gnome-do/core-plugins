
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
		
		
		protected virtual void OnRemoveBtnClicked (object sender, System.EventArgs e)
		{
		}

		
		
		protected virtual void OnAddBtnClicked (object sender, System.EventArgs e)
		{
		}

		
		public RTPrefs()
		{
			RTPreferences prefs = new RTPreferences();
			
			this.Build();
			
			Gtk.TreeViewColumn nameColumn = new Gtk.TreeViewColumn ();
			nameColumn.Title = "Name";
			Gtk.TreeViewColumn urlColumn = new Gtk.TreeViewColumn ();
			urlColumn.Title = "URL";
			
			RTTree.AppendColumn (nameColumn);
			RTTree.AppendColumn (urlColumn);
			
			Gtk.ListStore rtListStore = new Gtk.ListStore (typeof (string), typeof (string));
			RTTree.Model = rtListStore;
 
			Gtk.CellRendererText nameNameCell = new Gtk.CellRendererText ();
			nameColumn.PackStart (nameNameCell, true);
			Gtk.CellRendererText urlTitleCell = new Gtk.CellRendererText ();
			urlColumn.PackStart (urlTitleCell, true);

			nameColumn.AddAttribute (nameNameCell, "text", 0);
			urlColumn.AddAttribute (urlTitleCell, "text", 1);
			
			if (prefs.URLs != "") {
				string[] urlbits = prefs.URLs.Split('|');
				for (int i = 0; i < urlbits.Length; i++) {
					string name = urlbits[i];
					Uri url = new System.Uri(urlbits[++i]);
					
					rtListStore.AppendValues (name, url.ToString());
				}
			}
		}

	}
}

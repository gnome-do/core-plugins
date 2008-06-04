// Configuration.cs created with MonoDevelop
// User: dave at 9:06 PM 5/28/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using Gtk;
using GConf;

namespace FilePlugin
{	
	public partial class Configuration : Gtk.Bin
	{
		PathNodeView nview;
		GConf.Client gconf;
		
		public Configuration()
		{
			Build();
			
			gconf = new GConf.Client ();
			nview = new PathNodeView ();
			nview.Selection.Changed += new EventHandler (OnPathNodeViewSelectionChange);
			node_scroll.Add (nview);
			
			SetShowHiddenFromGConf ();
			
			remove_btn.Sensitive = false;
		}

		protected virtual void OnAddBtnClicked (object sender, System.EventArgs e)
		{
			ListStore store = nview.Model as ListStore;
			
			FileChooserDialog fc =
				new FileChooserDialog ("Choose a path", new Dialog (),
					FileChooserAction.SelectFolder,
					"Cancel", ResponseType.Cancel,
					"Select", ResponseType.Accept);
			
			if (fc.Run () == (int)ResponseType.Accept)
				store.AppendValues (fc.Filename, "1");
			fc.Destroy ();
			nview.WriteConfig ();
		}

		protected virtual void OnRemoveBtnClicked (object sender, System.EventArgs e)
		{
			TreeIter iter;
			nview.Selection.GetSelected (out iter);
			(nview.Model as ListStore).Remove (ref iter);
			nview.WriteConfig ();
		}
		
		
		protected void OnPathNodeViewSelectionChange (object sender, EventArgs e)
		{
			remove_btn.Sensitive = true;
		}
		
		public void GConfChanged (object sender, NotifyEventArgs args)
		{
			SetShowHiddenFromGConf ();
		}
		
		private void SetShowHiddenFromGConf ()
		{
			// sets the corresponding value in gconf
			try {
				show_hidden_chk.Active = (bool) gconf.Get (FileItemSource.GConfKeyBase
					+ "include_hidden");
			} catch (GConf.NoSuchKeyException) {
				gconf.Set (FileItemSource.GConfKeyBase + "include_hidden", false);
				show_hidden_chk.Active = false;
			}
		}

		protected virtual void OnShowHiddenChkClicked (object sender, System.EventArgs e)
		{
			try {
				gconf.Set (FileItemSource.GConfKeyBase + "include_hidden",
					show_hidden_chk.Active);
			} catch (GConf.NoSuchKeyException) {
				gconf.Set (FileItemSource.GConfKeyBase + "include_hidden",
					false);
			}
		}
	}
}

// Configuration.cs created with MonoDevelop
// User: dave at 9:06 PM 5/28/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;

namespace FilePlugin
{
	
	
	public partial class Configuration : Gtk.Bin
	{
		
		public Configuration()
		{
			this.Build();
		}

		protected virtual void OnAddBtnClicked (object sender, System.EventArgs e)
		{
		}

		protected virtual void OnRemoveBtnClicked (object sender, System.EventArgs e)
		{
		}
	}
}

//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//

using System;

namespace Confluence
{
	[System.ComponentModel.Category("Confluence")]
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ConfluenceConfigWidget : Gtk.Bin
	{
		private ConfluenceSearchAction _confluenceSearch;
		
		public ConfluenceConfigWidget(ConfluenceSearchAction confluenceSearch)
		{
			this.Build();
			_confluenceSearch = confluenceSearch;
			SetFieldsFromConfig( _confluenceSearch.Config );
		}
		
		protected virtual void OnSaveButtonReleased (object sender, System.EventArgs e)
		{
			_confluenceSearch.Log( "Re-init Confluence Plugin with new settings..." );

			_confluenceSearch.Config = GetConfigFromFields();
			
			// Write the config back out to the fields to reflect how we saved it
			SetFieldsFromConfig( _confluenceSearch.Config );
		}

		// Bindings...
		
		private void SetFieldsFromConfig( IConfluenceConfiguration config )
		{
			_entryBaseUrl.Text = config.BaseUrl;
			_entryUsername.Text = config.Username;
			_entryPassword.Text = config.Password;
			_entryMaxSearchResults.Text = Convert.ToString(config.MaxSearchResults);
		}
		
		private IConfluenceConfiguration GetConfigFromFields()
		{
			ConfluenceConfiguration config = new ConfluenceConfiguration();
			config.BaseUrl = _entryBaseUrl.Text;
			config.Username = _entryUsername.Text;
			config.Password = _entryPassword.Text;
			
			try 
			{
				config.MaxSearchResults = Convert.ToInt32(_entryMaxSearchResults.Text);
			} 
			catch (FormatException) 
			{
				_confluenceSearch.Log( "Invalid max search results specified.  Setting to default value." );
				config.MaxSearchResults = config.DefaultMaxSearchResults;
			}
			
			return config;
		}
	}
}

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
using System.Collections.Generic;

namespace JIRA
{
	[System.ComponentModel.Category("JIRA")]
	[System.ComponentModel.ToolboxItem(true)]
	public partial class ConfigWidget : Gtk.Bin
	{	
		private JIRAIssueSource _issueSource;
		
		public ConfigWidget( JIRAIssueSource issueSource )
		{			
			this.Build();
		
			_issueSource= issueSource;
			
			SetFieldsFromConfig( _issueSource.Config );
		}
		
		protected virtual void OnSaveButtonReleased (object sender, System.EventArgs e)
		{
			_issueSource.Log( "Re-init JIRA Plugin with new settings..." );			

			_issueSource.Config= GetConfigFromFields();
			_issueSource.Config.Persist();
			
			// Write the config back out to the fields to reflect how we saved it
			SetFieldsFromConfig( _issueSource.Config );
		}

		// Bindings...
		
		private void SetFieldsFromConfig( IJIRAConfiguration config )
		{
			config.Load();
			
			_entryBaseUrl.Text= config.BaseUrl;
			_entryUsername.Text= config.Username;
			_entryPassword.Text= config.Password;
			_entryProjects.Text= string.Join( ",", config.Projects );
		}
		
		private IJIRAConfiguration GetConfigFromFields()
		{
			JIRAConfiguration config= new JIRAConfiguration();
			config.BaseUrl= _entryBaseUrl.Text;
			config.Username= _entryUsername.Text;
			config.Password= _entryPassword.Text;
			
			// Split up the projects, and make sure they're correct
			List<string> projects= new List<string>();
			foreach( string str in _entryProjects.Text.Split( new char[] { ',', ';', ':', ' ' } ) )
			{
				string project= str.Trim();
				
				if( project.Length> 0 )
				{
					projects.Add( project );
				}
			}
			
			config.Projects= projects.ToArray();
			return config;
		}
	}
}

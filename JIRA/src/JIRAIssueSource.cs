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

using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Collections;
using Do.Universe;
using Do.Platform.Linux;
using Atlassian;
using JIRA.Remote;


namespace JIRA
{
	/// <summary>
	/// Provider of issues that meet our requirements (ie. certain projects, open criteria)
	/// </summary>
	public class JIRAIssueSource : ItemSource, IConfigurable
	{
		/// <summary>
		/// Our configuration for JIRA properties
		/// </summary>
		private IJIRAConfiguration _config;
		
		/// <summary>
		/// Server facade for handling issue download
		/// </summary>
		private IJIRAServerFacade _service;
		
		/// <summary>
		/// True when the plugin has init and updates of items should occur
		/// </summary>
		private bool _hasInit= false;
		
		/// <summary>
		/// Current list of outstanding issues
		/// </summary>
		private readonly List<Item> _items= new List<Item>();
		
		/// <summary>
		/// ProjectIds we're watching
		/// </summary>
		private readonly List<long> _projectIds= new List<long>();
		
		/// <summary>
		/// Statuses that are available within this JIRA installation
		/// </summary>
		private readonly List<int> _projectStatuses= new List<int>();
		
		/// <summary>
		/// Lock object for query thread safety, locks should always be taken in the order 
		/// of query lock, then item lock
		/// </summary>		
		private readonly object _lock= new object();
		
		
		/// <summary>
		/// Constructor (preloads config)
		/// </summary>
		public JIRAIssueSource()
		{			
			_config= new JIRAConfiguration();
			
			// Do the migration if need be
			if( ( (JIRAConfiguration) _config ).DoMigrate() )
			{
				Log( "Migrated credentials to gnome keyring" );
			}
			
			// Run the initial query						
			UpdateItems();
		}
		
		public override string Name { get { return "JIRA Issues"; } }
		public override string Description { get { return "Indexes JIRA Issues from a given repository"; } }
		public override string Icon { get { return "jira.png@"+GetType().Assembly.FullName; } }
		
		public IJIRAConfiguration Config
		{
			get { return _config; }
			set
			{
				_config= value;
				_hasInit= false;
			}
		}
		
		/// <summary>
		/// Create the configuration dialog
		/// </summary>
		public Gtk.Bin GetConfiguration()
		{
			return new ConfigWidget( this );
		}
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get
			{
				return new Type[] { typeof( JIRAIssueItem ) };
			}
		}
		
		public override IEnumerable<Item> Items
		{
			get 
			{
				lock( _items )
				{
					return _items;
				}
			}
		}

		/// <summary>
		/// Not Supported
		/// </summary>
		public override IEnumerable<Item> ChildrenOfItem( Item parent )
		{
			return null;  
		}
		
		/// <summary>
		/// Called semi-regularly such that we can poll the JIRA web service to see
		/// if there are any new issues matching our projects request list. This is called
		/// by gnome-do, and we don't want to block too long!
		/// </summary>
		public override void UpdateItems()
		{				
			// We do all our updates on another thread...
			new Thread( DoUpdateItems ).Start();
		}

		
		private void DoUpdateItems()
		{
			// If we didn't aquire the lock, we won't call update items now 
			if( !Monitor.TryEnter( _lock ) ) return;
			
			try
			{
				// We aquired the lock, but make sure we inited correctly
				if( !_hasInit )
				{
					// Do the init role
					DoInit();
					return;
				}				
				
				// We've previous init, so we're just looking for deltas
				List<JIRAIssueItem> changedIssues= ConvertFromRemoteIssues( _service.GetRecentlyUpdatedIssues( _projectIds ) );
				
				// Find Issues that have become closed... and purge them
				List<string> closedIssues= changedIssues
					.FindAll( delegate( JIRAIssueItem item ) { return item.IsClosed; } )
					.ConvertAll<string>( delegate( JIRAIssueItem item ) { return item.Name; } );

				// Do a quick lock while we remove the items that are now closed
				lock( _items )
				{
					int removedItems= _items.RemoveAll( delegate( Item item ) { return closedIssues.Contains( item.Name ); } );
					
					if( removedItems>0 )
					{
						Log( "Removed: {0} resolved/closed issues", removedItems );
					}
				}
				
				// Find and add the issues that aren't closed, but are new!
				List<string> currentIssues= _items.ConvertAll<string>( delegate( Item item ) { return item.Name; } );
				List<JIRAIssueItem> newIssues= changedIssues.FindAll( delegate( JIRAIssueItem item ) { return !item.IsClosed && !currentIssues.Contains( item.Name ); } );
				
				lock( _items )
				{
					_items.AddRange( newIssues.ToArray() );
				}
				
				// Notify what's changed
				if( newIssues.Count>0 )
				{
					Log( "Added: {0} new issues", newIssues.Count );
				}
			}
			catch( Exception e ) 
			{
				Log( "Unable to update JIRA items: {0}", e );
			}
			finally
			{
				Monitor.Exit( _lock );
			}
		}
		
		/// <summary>
		/// Determine the project ids of the given project codes by contacting the JIRA soap service.
		/// Then do an initial query for issues with these given ids
		/// </summary>
		private void DoInit()
		{
			try
			{
				Monitor.Enter( _lock );
				
				// Pre cleaning...
				_hasInit= true; // by the time this is done, we will have init...
				_projectStatuses.Clear();
				_projectIds.Clear();
				
				lock( _items )
				{
					_items.Clear();
				}
				
				// Check to make sure the config is valid...
				_config.Load();
				
				if( !_config.IsValid() )
				{
					Log( "Configuration for JIRA is not valid, see the plugin settings page" );

					// we failed to init
					_hasInit= false;
					return;
				}
				
				Log( "Initializing repository: "+_config.BaseUrl );				
				
				// init the service connection
				_service= new JIRAServerFacade( _config.BaseUrl, _config.Username, _config.Password );
				
				// Identify the available resolutions for an issue
				foreach( RemoteStatus status in _service.getStatuses() )
				{
					_projectStatuses.Add( int.Parse( status.id ) );

					Log( "Init Status: {0} => {1}", status.name, status.id );
				}
				
				// We want all issues that have a status that isn't 5 or 6 (resolved/closed)
				// we need to do it this way because JIRA doesn't support logical operators
				// for exclusion: see JRA-1560
				List<int> reqStatuses= new List<int>( _projectStatuses );
				reqStatuses.Remove( JIRAIssueItem.STATUS_RESOLVED );
				reqStatuses.Remove( JIRAIssueItem.STATUS_CLOSED );
				
				// Identify the project id's that we're interested in
				foreach( string projectKey in _config.Projects )
				{
					RemoteProject project= _service.getProjectByKey( projectKey );
					long projectId= long.Parse( project.id );
					
					// Use the rss client to discover the issues for this project that are open
					List<JIRAIssueItem> issues= ConvertFromRemoteIssues( 
										_service.GetAllIssuesWithStatus( 
															new List<long>( new long[]{ projectId } ), 
															reqStatuses ) );

					// Store this project and it's issues
					_projectIds.Add( projectId );
					
					lock( _items )
					{
						_items.AddRange( issues.ToArray() );
					}
					
					Log( "Init Project: {0} [{1}=>{2}] with: {3} issues.",
					    project.name, project.key, project.id, issues.Count );
				}

			}
			catch( System.Web.Services.Protocols.SoapException e )
			{
				Log( "Failure (re-)initializating JIRA plugin...\n{0}", e );	
			}			
			finally
			{
				Monitor.Exit( _lock );
			}
		}
		
		
		private List<JIRAIssueItem> ConvertFromRemoteIssues( IList<RemoteIssue> issues )
		{
			List<JIRAIssueItem> result= new List<JIRAIssueItem>();
			
			foreach( RemoteIssue inIssue in issues )
			{
				JIRAIssueItem outIssue= new JIRAIssueItem( inIssue.key, _config.BaseUrl );
				outIssue.setDescription(inIssue.summary);
				outIssue.Status= int.Parse( inIssue.status );
				
				result.Add( outIssue );
			}
			return result;			                                        
		}
		
		/// <summary>
		/// Temporary logging method until it's provided to plugins
		/// </summary>
		public void Log( string message, params object[] args )
		{
			string prefix= string.Format( "[Info {0:00}:{1:00}:{2:00}.{3:000}] [JIRA] ",
			                           DateTime.Now.Hour, 
			                           DateTime.Now.Minute, 
			                           DateTime.Now.Second,
			                           DateTime.Now.Millisecond ); 			
			
			Console.WriteLine( prefix + string.Format( message, args ) );			
		}
	}
}

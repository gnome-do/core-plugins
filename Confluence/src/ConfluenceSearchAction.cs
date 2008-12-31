//
//GNOME Do is the legal property of its developers. Please refer to the
//COPYRIGHT file distributed with this source distribution.
//
//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Linq;
using System.Collections.Generic;
using Do.Universe;
using Do.Platform.Linux;
using Mono.Unix;


/// <summary>
/// Do plug-in that returns search results from a Confluence wiki back to 
/// gnome-do for display and selection by the user
/// </summary>
namespace Confluence
{	
	
	// No longer in Do, have to subclass 
	public class BookmarkItem : Item, IBookmarkItem 
	{
		protected string name, url;
		public BookmarkItem (string name, string url)
		{
			this.name = name;
			this.url = url;
		}
		
		public override string Name
		{
			get { return name; }
		}
		
		public override string Description
		{
			get { return url; }
		}
		
		public override string Icon
		{
			get { return "www"; }
		}
		
		public string Url
		{
			get { return url; }
		}
	}

	public class ConfluenceSearchAction : Act, IConfigurable
	{	
		/// <summary>
		/// Our configuration for Confluence properties
		/// </summary>
		private IConfluenceConfiguration _config;

		/// <summary>
		/// Initializes ConfluenceSearchAction
		/// </summary>
		public ConfluenceSearchAction() 
		{
			_config = new ConfluenceConfiguration();
		}
		
		/// <value>
		/// The name of the plugin
		/// </value>
		public override string Name 
		{
			get { return Catalog.GetString ("Search Confluence"); }
		}
		
		/// <value>
		/// A description of the plugin
		/// </value>
		public override string Description 
		{
			get { return Catalog.GetString ("Searches Confluence and returns results to Do"); }
		}
		
		/// <value>
		/// Confluence icon
		/// </value>
		public override string Icon 
		{
			get { return "confluence.png@"+GetType().Assembly.FullName; }
		}
		
		/// <value>
		/// ITextItem
		/// </value>
		public override IEnumerable<Type> SupportedItemTypes 
		{
			get 
			{
				return new Type[] { typeof (ITextItem) };
			}
		}
		
		/// <value>
		/// false
		/// </value>
		public override bool ModifierItemsOptional 
		{
			get { return false; }
		}
		
		/// <value>
		/// The config class
		/// </value>
		public IConfluenceConfiguration Config
		{
			get { return _config; }
			set { _config= value; }
		}
		
		/// <summary>
		/// Create the configuration dialog
		/// </summary>
		public Gtk.Bin GetConfiguration()
		{
			return new ConfluenceConfigWidget( this );
		}
		
		/// <summary>
		/// Actual code performed when action is executed in Do
		/// </summary>
		/// <param name="items">
		/// Items. ITextItem <see cref="IItem"/>
		/// </param>
		/// <param name="modItems">
		/// Modifier Items. None <see cref="IItem"/>
		/// </param>
		/// <returns>
		/// Array of Bookmark Items. URLs to search results <see cref="IItem"/>
		/// </returns>
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems) {
			
			try 
			{
				ConfluenceSoapServiceService soapService = new ConfluenceSoapServiceService(_config.BaseUrl);
				
				// Only login if username AND password were provided in the config.
				// Otherwise, use anonymous access with a null token.
				String token = null;
				if (_config.Username != null && _config.Username.Trim().Length != 0 && 
				      _config.Password != null && _config.Password.Trim().Length != 0) 
				{
					token = soapService.login(_config.Username, _config.Password);
				}
				
				string query = (items.First () as ITextItem).Text;
				RemoteSearchResult[] results = soapService.search(token, query, _config.MaxSearchResults);
				
				List<Item> retItems = new List<Item> ();
				foreach (RemoteSearchResult result in results) 
				{
					retItems.Add(new BookmarkItem(result.title, result.url));
				}
				
				return retItems.ToArray();
			}
			catch( Exception e ) 
			{
				Log( "Unable to search Confluence: {0}", e );
				return null;
			}
		}
		
		/// <summary>
		/// Temporary logging method until it's provided to plugins
		/// </summary>
		public void Log( string message, params object[] args )
		{
			string prefix= string.Format( "[Info {0:00}:{1:00}:{2:00}.{3:000}] [Confluence] ",
			                           DateTime.Now.Hour, 
			                           DateTime.Now.Minute, 
			                           DateTime.Now.Second,
			                           DateTime.Now.Millisecond ); 	
			
			Console.WriteLine( prefix + string.Format( message, args ) );
		}
	}
}

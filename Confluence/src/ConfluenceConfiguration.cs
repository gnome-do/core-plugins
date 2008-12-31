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
	public interface IConfluenceConfiguration
	{
		string BaseUrl 
		{
			get;
		}
		
		string Username
		{
			get;
		}
		
		string Password 
		{
			get;
		}
		
		int MaxSearchResults
		{
			get;
		}
		
		/// <value>
		/// To be valid, all required fields must be filled out
		/// </value>
		bool IsValid();
	}	
	
	/// <summary>
	/// Configuration that uses gconf to do the magic
	/// </summary>
	public class ConfluenceConfiguration : IConfluenceConfiguration
	{
		/// <summary>
		/// The gnome conf key that contains the base url for our Confluence installation.
		/// ie. http://opensource.atlassian.com/confluence/spring
		/// </summary>
		private const string _sGCONF_KEY= "/apps/gnome-do/plugins/Confluence";
		
		private GConf.Client _gconf;
		
		public ConfluenceConfiguration()
		{
			_gconf= new GConf.Client();
			
			// Set defaults
			MaxSearchResults = DefaultMaxSearchResults;
		}
		
		private void SetConfValue( string key, string val )
		{
			_gconf.Set( _sGCONF_KEY + "/" + key, val );
		}
		
		private string GetConfValue( string key, string fallback )
		{
			try
			{
				return _gconf.Get( _sGCONF_KEY + "/" + key ) as String;
			}
			catch( GConf.NoSuchKeyException ) 
			{
				return fallback;
			}
		}
		
		public string BaseUrl
		{
			get { return GetConfValue( "baseUrl", "" ); }
			set 
			{
				value = value.Trim();
				if (value.EndsWith("/"))
				{
					char[] trimChars = { '/' };
					value = value.TrimEnd(trimChars);
				}
					
				SetConfValue( "baseUrl", value ); 
			}
		}
		
		public string Username
		{
			get { return GetConfValue( "username", "" ); }
			set { SetConfValue( "username", value ); }
		}
		
		public string Password 
		{
			get { return GetConfValue( "password", "" ); }
			set { SetConfValue( "password", value ); }
		}
		
		public int MaxSearchResults
		{
			get 
			{
				try 
				{
					return Convert.ToInt32(GetConfValue( "maxSearchResults", "" ));
				} 
				catch (FormatException) 
				{
					return DefaultMaxSearchResults;
				}
			}
			
			set { SetConfValue( "maxSearchResults", Convert.ToString( value )); }
		}
		
		public int DefaultMaxSearchResults
		{
			get 
			{ 
				return 20; 
			}
		}
		
		/// <value>
		/// To be valid, all required fields must be filled out
		/// </value>
		public bool IsValid()
		{
			return BaseUrl.Length>0;
		}
	}
}

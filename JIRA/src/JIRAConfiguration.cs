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
using Do.Platform;
using Do.Universe;



namespace JIRA
{

	
	
	public interface IJIRAConfiguration
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
		
		string[] Projects 
		{ 
			get;
		}
		
		/// <value>
		/// To be valid, all fields must be filled out
		/// </value>
		bool IsValid();
		
		/// <summary>
		/// Persist the properties of this configuration bean to an underlying store
		/// </summary>
		void Persist();
		
		/// <summary>
		/// Pre-fill the properties of this bean from the underlying store
		/// </summary>
		void Load();
	}	
	
	/// <summary>
	/// Configuration that uses gconf to do the magic
	/// </summary>
	public class JIRAConfiguration : IJIRAConfiguration
	{
		private string _baseUrl;
		private string _username;
		private string _password;
		private string[] _projects;

		static private IPreferences prefs;
		
		/// <summary>
		/// The gnome conf key that contains the base url for our JIRA installation.
		/// ie. http://issues.apache.org/jira
		/// </summary>
		private const string _sGCONF_KEY= "/apps/gnome-do/plugins/JIRA";
	
		private GConf.Client _gconf;		
		
		public JIRAConfiguration()
		{
			_gconf= new GConf.Client();
		}
		
		static JIRAConfiguration() 
		{
			prefs = Do.Platform.Services.Preferences.Get<JIRA.JIRAConfiguration>();
		}			
		
		private void SetConfValue( string key, string val )
		{
			_gconf.Set( _sGCONF_KEY +"/"+key, val );
		}
		
		private string GetConfValue( string key, string fallback )
		{
			try
			{
				return _gconf.Get( _sGCONF_KEY+"/"+key ) as String;
			}
			catch( GConf.NoSuchKeyException ) 
			{
				return fallback;
			}
		}
		
		public string BaseUrl
		{
			get { return _baseUrl ?? ""; }
			set { _baseUrl= value; }
		}
		public string Username
		{
			get { return _username ?? "";	}
			set { _username= value; }
		}
		
		public string Password 
		{
			get { return _password ?? ""; }
			set { _password= value; }
		}
		public string[] Projects 
		{ 
			get { return _projects ?? new string[ 0 ]; }
			set { _projects= value; }
		}
		
		/// <value>
		/// To be valid, all fields must be filled out
		/// </value>
		public bool IsValid()
		{
			return _baseUrl!=null
				&& _username!=null
				&& _password!=null
				&& _projects!=null; 
		}
		
		public void Load()
		{
			_baseUrl= GetConfValue( "baseUrl", null );
			
			string projStr= GetConfValue( "projects", "" );
			_projects= projStr.Length>0 ? projStr.Split( ',' ) : null;

			// Use the gnome-do framework for retrieving from the keychain
			_username = prefs.GetSecure("username", "");
			_password = prefs.GetSecure("password", "");
		}
		
		public void Persist()
		{
			prefs.Set("baseUrl", _baseUrl );
			prefs.Set("projects", string.Join(",", _projects));
			prefs.SetSecure("username", _username );
			prefs.GetSecure("password", _password );
		}
		
		
		/// <summary>
		/// Do a migration to the gnome keyring and return true if we do a migrate, false if
		/// we've done it previously
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		public bool DoMigrate()
		{
			// Because we used to write the username/password to gconf, provide a migration
			// strategy that will haul the values into the keyring and clear them out of
			// gconf
			if( "".Equals( GetConfValue( "password", "" ) ) ) return false;

			prefs.SetSecure("username", GetConfValue("username", null));
			prefs.SetSecure("password", GetConfValue("password", null));
			
			SetConfValue( "username", "" );
			SetConfValue( "password", "" );
			
			return true;
		}
	}
}

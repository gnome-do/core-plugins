using System;
using Gtk;
using Do.Platform;
using Do.Platform.Linux;

namespace YouTube
{	
	
	public class YouTubeConfig : AbstractLoginWidget
	{
		static IPreferences prefs;		
		const string Uri = "http://www.youtube.com/signup?next_url=/index&";
        
		public YouTubeConfig() : base ("YouTube", Uri)
		{
			Username = YouTube.Youtube.Preferences.Username;
			Password = YouTube.Youtube.Preferences.Password;
		}
		
		protected override bool Validate (string username, string password)
		{
			if (username.Length > 0 && password.Length > 0)
				return Youtube.TryConnect (username, password);
			return false;
		}
		
		protected override void SaveAccountData(string username, string password)
		{
			YouTube.Youtube.Preferences.Username = username;
			YouTube.Youtube.Preferences.Password = password;
		}
		
		public static string username { 
			get { return prefs.GetSecure<string> ("username", "" ); } 
		}

		public static string password { 
			get { return prefs.GetSecure<string> ("password", "" ); } 
		}
	}
}


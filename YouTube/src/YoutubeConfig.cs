using System;
using Gtk;
using Do.Platform;
using Do.Platform.Linux;

namespace YouTube
{	
	
	public class YouTubeConfig : AbstractLoginWidget
	{
		const string Uri = "http://www.youtube.com/signup?next_url=/index&";
        
		public YouTubeConfig() : base ("YouTube", Uri)
		{
			Username = Youtube.Preferences.Username;
			Password = Youtube.Preferences.Password;
		}
		
		
		protected override void SaveAccountData(string username, string password)
		{
			Youtube.Preferences.Username = username;
			Youtube.Preferences.Password = password;
		}
		
		protected override bool Validate (string username, string password)
		{
			if (username.Length > 0 && password.Length > 0)
				return Youtube.TryConnect (username, password);
			return false;
		}
	}
}


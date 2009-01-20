// Preferences.cs created with MonoDevelop
// User: luis at 07:53 p 14/01/2009
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;

using Mono.Unix;

using Do.Platform;

namespace YouTube
{
	
	public class YouTubePreferences
	{
		const string UsernameKey = "Username";
		const string PasswordKey = "Password";
		
		IPreferences prefs;
		
		public YouTubePreferences()
		{
			prefs = Services.Preferences.Get<YouTubePreferences> ();
		}

		public string Username {
			get { return prefs.Get<string> (UsernameKey, ""); }
			set { prefs.Set<string> (UsernameKey, value); }
		}

		public string Password {
			get { return prefs.GetSecure (PasswordKey, ""); }
			set { prefs.SetSecure (PasswordKey, value); }
		}
	}
}
/* Configuration.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Threading;
using Mono.Addins;

using Gtk;
using FlickrNet;

using Do.Platform;

namespace Flickr
{	
	public partial class AccountConfig : Gtk.Bin
	{	
		private FlickrNet.Flickr flickr;
		private static IPreferences prefs;
		private string Frob; 
		
		public readonly static string ApiKey	= "aa645b69c14422e095dee81dda21385b";
		public readonly static string ApiSecret	= "a266af1a63024d3d";
		
		public AccountConfig ()
		{
			Build();
			if (!String.IsNullOrEmpty (AuthToken)) {
			flickr = new FlickrNet.Flickr (ApiKey, ApiSecret, AuthToken);
				SetBtnStateComplete ();
			} else
				flickr = new FlickrNet.Flickr (ApiKey, ApiSecret);
		}
		
		static AccountConfig ()
		{
			prefs = Services.Preferences.Get<AccountConfig> ();
		}
		
		public static string AuthToken {
			get { return prefs.Get ("token", ""); }
			set { prefs.Set ("token", value); }
		}
		
		public static string Username {
			get { return prefs.Get ("username", ""); }
			set { prefs.Set ("username", value);; }
		}
		
		public static string Tags {
			get { return prefs.Get ("tags", ""); }
			set { prefs.Set ("tags", value); }
		}
		
		public static bool IsPublic {
			get { return prefs.Get ("is_public", false); }
			set { prefs.Set ("is_public", value); }
		}
		
		public static bool FamilyAllowed {
			get { return prefs.Get ("allow_family", false); }
			set { prefs.Set ("allow_family", value); }
		}
		
		public static bool FriendsAllowed {
			get { return prefs.Get ("allow_friends", false); }
			set { prefs.Set ("allow_friends", value); }
		}
		
		protected virtual void OnAuthBtnClicked (object sender, EventArgs e)
		{
			flickr = new FlickrNet.Flickr (ApiKey, ApiSecret);
			Frob = flickr.AuthGetFrob ();
			Services.Environment.OpenUrl (flickr.AuthCalcUrl (Frob, AuthLevel.Write));
			Widget image = auth_btn.Image;
			auth_btn.Label = AddinManager.CurrentLocalizer.GetString ("Click to complete authorization");
			auth_btn.Image = image; 
			auth_btn.Clicked -= new EventHandler (OnAuthBtnClicked);
			auth_btn.Clicked += new EventHandler (OnCompleteBtnClicked);
		}
		
		protected virtual void OnCompleteBtnClicked (object sender, EventArgs e)
		{
			try {
				Auth auth = flickr.AuthGetToken(Frob);
		  		AuthToken = auth.Token;
		  		Username = auth.User.Username;
		  		flickr = new FlickrNet.Flickr (ApiKey, ApiSecret, AuthToken);
		  		SetBtnStateComplete ();
		  	} catch (FlickrNet.FlickrException ex) {
		  		Console.Error.WriteLine (ex);
		  	}
		}
		
		private void SetBtnStateComplete ()
		{
			status_lbl.Text = String.Format (AddinManager.CurrentLocalizer.GetString ("Thank you {0} "
				+ "for allowing Do access to Flickr."), Username);
		  	auth_btn.Label = "Sign in as a different user";
		  	auth_btn.Clicked -= new EventHandler (OnCompleteBtnClicked);
		  	auth_btn.Clicked += new EventHandler (OnAuthBtnClicked);
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return this;
		}
	}
}

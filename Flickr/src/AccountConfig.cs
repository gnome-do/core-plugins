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
using Mono.Unix;

using Gtk;
using FlickrNet;
using Do.Addins;

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
			prefs = Do.Addins.Util.GetPreferences ("flickr");
		}
		
		public static string AuthToken {
			get { return prefs ["token"]; }
			set { prefs ["token"] = value; }
		}
		
		public static string Username {
			get { return prefs ["username"]; }
			set { prefs ["username"] = value; }
		}
		
		public static string Tags {
			get { return prefs ["tags"]; }
			set { prefs ["tags"] = value; }
		}
		
		public static bool IsPublic {
			get { return prefs.Get<bool> ("is_public", false); }
			set { prefs.Set<bool> ("is_public", value); }
		}
		
		public static bool FamilyAllowed {
			get { return prefs.Get<bool> ("allow_family", false); }
			set { prefs.Set<bool> ("allow_family", value); }
		}
		
		public static bool FriendsAllowed {
			get { return prefs.Get<bool> ("allow_friends", false); }
			set { prefs.Set<bool> ("allow_friends", value); }
		}
		
		protected virtual void OnAuthBtnClicked (object sender, EventArgs e)
		{
			flickr = new FlickrNet.Flickr (ApiKey, ApiSecret);
			Frob = flickr.AuthGetFrob ();
			Do.Addins.Util.Environment.Open (flickr.AuthCalcUrl (Frob, AuthLevel.Write));
			Widget image = auth_btn.Image;
			auth_btn.Label = Catalog.GetString ("Click to compete authorization");
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
			status_lbl.Text = String.Format (Catalog.GetString ("Thank you {0} "
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
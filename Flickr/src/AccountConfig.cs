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

using Gtk;
using GConf;
using FlickrNet;

namespace Flickr
{	
	public partial class AccountConfig : Gtk.Bin
	{	
		private FlickrNet.Flickr flickr;
		private static GConf.Client gconf;
		private string Frob; 
		
		static string API_KEY  			= "aa645b69c14422e095dee81dda21385b";
		static string API_SECRET        = "a266af1a63024d3d";
		static string GCONF_KEY_BASE    = "/apps/gnome-do/plugins/flickr/";
		static string GCONF_KEY_TOKEN   = GCONF_KEY_BASE + "token";
		static string GCONF_KEY_TAGS    = GCONF_KEY_BASE + "tags";
		static string GCONF_KEY_PUBLIC  = GCONF_KEY_BASE + "is_public"; 
		static string GCONF_KEY_FAMILY  = GCONF_KEY_BASE + "allow_family";
		static string GCONF_KEY_FRIENDS = GCONF_KEY_BASE + "allow_friends";
		
		public AccountConfig ()
		{
			this.Build();
			gconf = new GConf.Client ();
			try {
				gconf.Get (GCONF_KEY_TOKEN);
				SetBtnStateComplete ();
			} catch (GConf.NoSuchKeyException e) { 
				Console.Error.WriteLine (e);
			}
		}
		
		static AccountConfig ()
		{
			gconf = new GConf.Client ();
		}
		
		public static string ApiKey {
			get { return API_KEY; }
		}
		
		public static string ApiSecret {
			get { return API_SECRET; }
		}
		
		public static string AuthToken {
			get {
				try {
					return gconf.Get (GCONF_KEY_TOKEN) as string;
				} catch (GConf.NoSuchKeyException) {
					Console.Error.WriteLine ("No Auth Token! Please authorize");
				}
				return "";
			}
			set {
				gconf.Set (GCONF_KEY_TOKEN, value);
			}
		}
		
		public static string Tags {
			get {
				try {
					return gconf.Get (GCONF_KEY_TAGS) as string;
				} catch (GConf.NoSuchKeyException) {
					Tags = "";
				}
				return "";
			}
			set {
				gconf.Set (GCONF_KEY_TAGS, value);
			}
		}
		
		public static bool IsPublic {
			get {
				try {
					return (bool) gconf.Get (GCONF_KEY_PUBLIC);
				} catch (GConf.NoSuchKeyException) {
					IsPublic = false;
				}
				return false;
			}
			set {
				gconf.Set (GCONF_KEY_PUBLIC, value);
			}
		}
		
		public static bool FamilyAllowed {
			get {
				try {
					return (bool) gconf.Get (GCONF_KEY_FAMILY);
				} catch (GConf.NoSuchKeyException) {
					FamilyAllowed = false;
				}
				return false;
			}
			set {
				gconf.Set (GCONF_KEY_FAMILY, value);
			}
			
		}
		
		public static bool FriendsAllowed {
			get {
				try {
					return (bool) gconf.Get (GCONF_KEY_FRIENDS);
				} catch (GConf.NoSuchKeyException) {
					FriendsAllowed = false;
				}
				return false;
			}
			set {
				gconf.Set (GCONF_KEY_FRIENDS, value);
			}
		}
		
		protected virtual void OnAuthBtnClicked (object sender, EventArgs e)
		{
			flickr = new FlickrNet.Flickr (API_KEY, API_SECRET);
			Frob = flickr.AuthGetFrob ();
			Do.Addins.Util.Environment.Open (flickr.AuthCalcUrl (Frob, AuthLevel.Delete));
			Widget image = auth_btn.Image;
			auth_btn.Label = ("Click to compete authorization");
			auth_btn.Image = image; 
			auth_btn.Clicked -= new EventHandler (OnAuthBtnClicked);
			auth_btn.Clicked += new EventHandler (OnCompleteBtnClicked);
		}
		
		protected virtual void OnCompleteBtnClicked (object sender, EventArgs e)
		{
			try {
				Auth auth = flickr.AuthGetToken(Frob);
		  		gconf.Set (GCONF_KEY_TOKEN, auth.Token);
		  		SetBtnStateComplete ();
		  	} catch (FlickrNet.FlickrException ex) {
		  		Console.Error.WriteLine (ex);
		  	}
		}
		
		private void SetBtnStateComplete ()
		{
			Widget image = auth_btn.Image;
		  	auth_btn.Label = "Sign in as a different user";
		  	auth_btn.Image = image;
		  	auth_btn.Clicked -= new EventHandler (OnCompleteBtnClicked);
		  	auth_btn.Clicked += new EventHandler (OnAuthBtnClicked);
		}
	}
}

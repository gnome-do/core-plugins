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
	public partial class Configuration : Gtk.Bin
	{	
		private FlickrNet.Flickr flickr;
		private GConf.Client gconf;
		
		private string Frob; 
		const string API_KEY = "aa645b69c14422e095dee81dda21385b";
		const string API_SECRET = "a266af1a63024d3d";
		const string GCONF_KEY_TOKEN = "/apps/gnome-do/plugins/flickr/token";
		
		public Configuration()
		{
			this.Build();
			
		}
		
		protected virtual void OnAuthBtnClicked (object sender, EventArgs e)
		{
			flickr = new FlickrNet.Flickr (API_KEY, API_SECRET);
			gconf = new GConf.Client ();
			Frob = flickr.AuthGetFrob ();
			Do.Addins.Util.Environment.Open (flickr.AuthCalcUrl (Frob, AuthLevel.Delete));
			auth_btn.Label = ("Click to compete authorization");
			auth_btn.Clicked -= new EventHandler (OnAuthBtnClicked);
			auth_btn.Clicked += new EventHandler (OnCompleteBtnClicked);
		}
		
		protected virtual void OnCompleteBtnClicked (object sender, EventArgs e)
		{
			Console.Error.WriteLine ("Complete btn clicked");
			try {
				Auth auth = flickr.AuthGetToken(Frob);
		  		gconf.Set (GCONF_KEY_TOKEN, auth.Token);
		  	} catch (FlickrNet.FlickrException ex) {
		  		Console.Error.WriteLine (ex);
		  	}
		}
		
		protected virtual void OnPrivateRadioToggled (object sender, System.EventArgs e)
		{
		}

		protected virtual void OnFriendsChkClicked (object sender, System.EventArgs e)
		{
		}

		protected virtual void OnFamilyChkClicked (object sender, System.EventArgs e)
		{
		}

		protected virtual void OnPublicRadioClicked (object sender, System.EventArgs e)
		{
		}
	}
}

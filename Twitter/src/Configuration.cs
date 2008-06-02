/*
 * Configuration.cs
 * 
 * GNOME Do is the legal property of its developers, whose names are too numerous
 * to list here.  Please refer to the COPYRIGHT file distributed with this
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
using Do.Addins;

namespace DoTwitter
{
	public partial class Configuration : Gtk.Bin
	{		
		public Configuration()
		{
			this.Build();
			string username, password;
			
			GConf.Client gconf = new GConf.Client ();
			try {
				username = gconf.Get (TwitterAction.GConfKeyBase + "username") as string;
				password = gconf.Get (TwitterAction.GConfKeyBase + "password") as string;
			} catch (GConf.NoSuchKeyException) {
				username = "";
				password = "";
			}
			
			username_entry.Text = username;
			passwd_entry.Text = password;
		}

		protected virtual void OnNewAcctClicked (object sender, System.EventArgs e)
		{
			Util.Environment.Open ("https://twitter.com/signup");
		}

		protected virtual void OnApplyBtnClicked (object sender, System.EventArgs e)
		{
			string username = username_entry.Text.Trim ();
			string password = passwd_entry.Text.Trim ();
			
			apply_btn.Label = "Validating...";
			apply_btn.Sensitive = false;
			
			new Thread ((ThreadStart) delegate {
				bool valid = TwitterAction.TryConnect (username, password);
				
				Gtk.Application.Invoke (delegate {
					if (valid) {
						valid_lbl.Markup = "<i>Account validation succeeded</i>!";
						TwitterAction.SetAccountData (username, password);
					} else {
						valid_lbl.Markup = "<i>Account validation failed!</i>";
					}
					apply_btn.Label = "Apply";
					apply_btn.Sensitive = true;
				});
			}).Start ();
		}
	}
}

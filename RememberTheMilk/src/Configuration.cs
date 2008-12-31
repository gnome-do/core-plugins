/* Configuration.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this source distribution.
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
using Mono.Unix;
using RtmNet;
using Do.Platform;

using Gtk;


namespace RememberTheMilk
{
	public partial class Configuration : Gtk.Bin
	{
		private static IPreferences prefs;
		private LinkButton rtm_ref_btn;
		private string frob;
		
		public Configuration ()
		{
			this.Build();
			rtm_ref_btn = new LinkButton ("Visit Remember The Milk website for more information.",
			                              "List of available operators");
			info_hbox.Add (rtm_ref_btn);
			Box.BoxChild wInt = info_hbox [rtm_ref_btn] as Box.BoxChild;
			wInt.Position = 1;
			rtm_ref_btn.Clicked += OnRtmRefBtnClicked;
			
			if (!System.String.IsNullOrEmpty (AuthToken)) {
				SetStateComplete ();
			}
		}
		
		static Configuration ()
		{
			prefs = Do.Platform.Services.Preferences.Get<RememberTheMilk.Configuration>();
		}
		
		public static string AuthToken {
			get { return prefs.Get("Token", ""); }
			set { prefs.Set("Token", value); }
		}
		
		public static string Username {
			get { return prefs.Get("Username", ""); }
			set { prefs.Set("Username",value); }
		}
		
		public static string Filter {
			get { return prefs.Get<string> ("Filter", "status:incomplete"); }
			set { prefs.Set<string> ("Filter", value); }
		}
		
		public static bool OverdueNotification {
			get { return prefs.Get<bool> ("OverdueNotification", true); }
			set { prefs.Set<bool> ("OverdueNotification", value); }
		}
		
		public static bool ActionNotification {
			get { return prefs.Get<bool> ("ActionNotification", true); }
			set { prefs.Set<bool> ("ActionNotification", value); }
		}

		protected virtual void OnConfirmChkbtnClicked (object sender, System.EventArgs e)
		{
			ActionNotification = confirm_chkbtn.Active;
		}

		protected virtual void OnOverdueChkbtnClicked (object sender, System.EventArgs e)
		{
			OverdueNotification = overdue_chkbtn.Active;
		}

		protected virtual void OnAuthBtnClicked (object sender, System.EventArgs e)
		{
			frob = RTM.AuthInit ();
			authinfo_lbl.Text = Catalog.GetString ("A webpage from Remember The Milk should be opened"
			     + " in your web browser now. Please follow the instructions there and come back to complete"
			     + " the authrozation by clicking the button below.");
			AuthToken = "";
			Username = "";
			//notification_frm.Visible = false;
			//filter_frm.Visible = false;
			Widget image = auth_btn.Image;
			auth_btn.Label = Catalog.GetString ("Complete authorization");
			auth_btn.Image = image;
			auth_btn.Clicked -= new EventHandler (OnAuthBtnClicked);
			auth_btn.Clicked += new EventHandler (OnCompleteBtnClicked);
		}
		
		protected virtual void OnCompleteBtnClicked (object sender, EventArgs e)
		{
			Auth auth;
			auth = RTM.AuthComplete (frob);
			if (auth != null ) {
				AuthToken = auth.Token;
				Username = auth.User.Username;
				auth_btn.Clicked -= new EventHandler (OnCompleteBtnClicked);
				auth_btn.Clicked += new EventHandler (OnAuthBtnClicked);
				SetStateComplete ();
			} else {
				authinfo_lbl.Text = Catalog.GetString ("Fail to complete authorization.");
				auth_btn.Clicked -= new EventHandler (OnCompleteBtnClicked);
				auth_btn.Clicked += new EventHandler (OnAuthBtnClicked);
				auth_btn.Label = Catalog.GetString ("Authorize again");
			}
		}
		
		private void SetStateComplete ()
		{
			authinfo_lbl.Text = String.Format (Catalog.GetString ("Thank you {0}, "
			    + "RTM plugin is now authorized to operate on your account."), Username);
			auth_btn.Label = "Sign in as a different user";
			notification_frm.Visible = true;
			filter_frm.Visible = true;
			confirm_chkbtn.Active = ActionNotification;
			overdue_chkbtn.Active = OverdueNotification;
			filter_entry.Text = Filter;	
		}

		protected virtual void OnFilterEntryChanged (object sender, System.EventArgs e)
		{
			Filter = filter_entry.Text;
		}

		protected virtual void OnRtmRefBtnClicked (object sender, EventArgs e)
		{
			Do.Platform.Services.Environment.OpenUrl("http://www.rememberthemilk.com/help/answers/search/advanced.rtm");
		}
	}
}

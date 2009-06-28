// Configuration.cs
// 
// Copyright (C) 2009 GNOME Do
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using Mono.Addins;
using RtmNet;
using Do.Platform;

using Gtk;

namespace RememberTheMilk
{
	/// <summary>
	/// (Partial) class for the preference dialog
	/// </summary>
	public partial class Configuration : Gtk.Bin
	{
		private LinkButton rtm_ref_btn;
		private string frob;
		
		/// <summary>
		/// Constructor. Creates a new link button point to the help page for filter
		/// Initialize the UI.
		/// </summary>
		public Configuration ()
		{
			this.Build();
			rtm_ref_btn = new LinkButton ("Visit Remember The Milk website for more information.",
			                              "List of available operators");
			info_hbox.Add (rtm_ref_btn);
			Box.BoxChild wInt = info_hbox [rtm_ref_btn] as Box.BoxChild;
			wInt.Position = 1;
			rtm_ref_btn.Clicked += OnRtmRefBtnClicked;
			
			if (!System.String.IsNullOrEmpty (RTMPreferences.Token)) {
				SetStateComplete ();
			}
		}
		
		static Configuration ()
		{
		}
		
		/// <summary>
		/// Called when the user checks to receive notification after each action
		/// </summary>
		/// <param name="sender">
		/// Ignored
		/// </param>
		/// <param name="e">
		/// Ignored
		/// </param>
		protected virtual void OnConfirmChkbtnClicked (object sender, System.EventArgs e)
		{
			RTMPreferences.ActionNotification = confirm_chkbtn.Active;
		}
		
		/// <summary>
		/// Called when the user checkr the "Notfiy overdue tasks option"
		/// </summary>
		/// <param name="sender">
		/// Ignored
		/// </param>
		/// <param name="e">
		/// Ignored
		/// </param>
		protected virtual void OnOverdueChkbtnClicked (object sender, System.EventArgs e)
		{
			RTMPreferences.OverdueNotification = overdue_chkbtn.Active;
			overdue_interval_spinbtn.Sensitive = overdue_chkbtn.Active;
			overdue_interval_spinbtn.Value = RTMPreferences.OverdueInterval;
			
		}
		
		/// <summary>
		/// Called when the user checks the "Return the newly created task" option.
		/// </summary>
		/// <param name="sender">
		/// Ignored
		/// </param>
		/// <param name="e">
		/// Ignored
		/// </param>
		protected virtual void OnReturnNewChkBtnClicked (object sender, System.EventArgs e)
		{
			RTMPreferences.ReturnNewTask = returnnew_chkbtn.Active;
		}
		
		/// <summary>
		/// Called when the "Authorize" button is clicked. Initializes the authentication.
		/// </summary>
		/// <param name="sender">
		/// Ignored
		/// </param>
		/// <param name="e">
		/// Ignored
		/// </param>
		protected virtual void OnAuthBtnClicked (object sender, System.EventArgs e)
		{
			frob = RTM.AuthInit ();
			authinfo_lbl.Text = AddinManager.CurrentLocalizer.GetString ("A webpage from Remember The Milk should be opened"
			     + " in your web browser now. Please follow the instructions there and come back to complete"
			     + " the authrozation by clicking the button below.");
			RTMPreferences.Token = "";
			RTMPreferences.Username = "";
			notification_frm.Sensitive = false;
			filter_frm.Sensitive = false;
			Widget image = auth_btn.Image;
			auth_btn.Label = AddinManager.CurrentLocalizer.GetString ("Complete authorization");
			auth_btn.Image = image;
			auth_btn.Clicked -= new EventHandler (OnAuthBtnClicked);
			auth_btn.Clicked += new EventHandler (OnCompleteBtnClicked);
		}
		
		/// <summary>
		/// Called when user returns from the authentication webpage and clicks the "Complete" button.
		/// </summary>
		/// <param name="sender">
		/// Ignored
		/// </param>
		/// <param name="e">
		/// Ignored
		/// </param>
		protected virtual void OnCompleteBtnClicked (object sender, EventArgs e)
		{
			Auth auth;
			auth = RTM.AuthComplete (frob);
			if (auth != null ) {
				RTMPreferences.Token = auth.Token;
				RTMPreferences.Username = auth.User.Username;
				auth_btn.Clicked -= new EventHandler (OnCompleteBtnClicked);
				auth_btn.Clicked += new EventHandler (OnAuthBtnClicked);
				SetStateComplete ();
			} else {
				authinfo_lbl.Text = AddinManager.CurrentLocalizer.GetString ("Fail to complete authorization.");
				auth_btn.Clicked -= new EventHandler (OnCompleteBtnClicked);
				auth_btn.Clicked += new EventHandler (OnAuthBtnClicked);
				auth_btn.Label = AddinManager.CurrentLocalizer.GetString ("Authorize again");
			}
		}
		
		/// <summary>
		/// Initialize the state of various UI components.
		/// </summary>
		private void SetStateComplete ()
		{
			authinfo_lbl.Text = String.Format (AddinManager.CurrentLocalizer.GetString ("Thank you {0}, RTM plugin is now authorized to operate on your account."), 
			                                   RTMPreferences.Username);
			auth_btn.Label = "Sign in as a different user";
			notification_frm.Sensitive = true;
			filter_frm.Sensitive = true;
			confirm_chkbtn.Active = RTMPreferences.ActionNotification;
			overdue_chkbtn.Active = RTMPreferences.OverdueNotification;
			overdue_interval_spinbtn.Sensitive = overdue_chkbtn.Active;
			overdue_interval_spinbtn.Value = RTMPreferences.OverdueInterval;
			returnnew_chkbtn.Active = RTMPreferences.ReturnNewTask;
			filter_entry.Text = RTMPreferences.Filter;
		}
		
		/// <summary>
		/// Called when the Filter entry is changed by user.
		/// </summary>
		/// <param name="sender">
		/// Ignored
		/// </param>
		/// <param name="e">
		/// Ignored
		/// </param>
		protected virtual void OnFilterEntryChanged (object sender, System.EventArgs e)
		{
			RTMPreferences.Filter = filter_entry.Text;
		}
		
		/// <summary>
		/// Called when the link button to RTM reference page is clicked by user.
		/// </summary>
		/// <param name="sender">
		/// Ignored
		/// </param>
		/// <param name="e">
		/// Ignored
		/// </param>
		protected virtual void OnRtmRefBtnClicked (object sender, EventArgs e)
		{
			Do.Platform.Services.Environment.OpenUrl("http://www.rememberthemilk.com/help/answers/search/advanced.rtm");
		}
		
		/// <summary>
		/// Called when user changes the interval spin button.
		/// </summary>
		/// <param name="sender">
		/// Ignored
		/// </param>
		/// <param name="e">
		/// Ignored
		/// </param>
		protected virtual void OnOverdueIntervalChanged (object sender, System.EventArgs e)
		{
			RTMPreferences.OverdueInterval = overdue_interval_spinbtn.Value;
		}
	}
}

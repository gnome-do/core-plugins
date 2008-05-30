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
using System.Text.RegularExpressions;
using Gtk;
using Do.Addins;

namespace GMailContacts
{
	public partial class Configuration : Gtk.Bin
	{		
		public Configuration()
		{
			this.Build();
			string username, password;
			
			GMail.GetUserAndPassFromKeyring (out username, out password,
			                                GMail.GAppName);
			username_entry.Text = username;
			passwd_entry.Text = password;
		}

		protected virtual void OnNewAcctClicked (object sender, System.EventArgs e)
		{
			Util.Environment.Open ("https://www.google.com/accounts/NewAccount?service=mail");
		}

		protected virtual void OnApplyBtnClicked (object sender, System.EventArgs e)
		{
			string username = username_entry.Text.Trim ();
			string password = passwd_entry.Text.Trim ();
			apply_btn.Label = "Validating...";
			apply_btn.Sensitive = false;
			if (ValidateUsername (username) && ValidatePassword (password)
			    && GMail.TryConnect (username, password)) {
				valid_lbl.Markup = "<i>Account validation succeeded</i>!";
				GMail.WriteAccountToKeyring (username, password, GMail.GAppName);
			} else {			
				valid_lbl.Markup = "<i>Account validation failed!</i>";
			}
			apply_btn.Label = "_Apply";
			apply_btn.Sensitive = true;
		}
		
		private bool ValidateUsername (string username)
		{
			const string emailPattern = @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\."
            + @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*"
            + @"[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?";
			
			Regex validEmail = new Regex (emailPattern, RegexOptions.Compiled);
			return validEmail.IsMatch (username);
		}
		
		private bool ValidatePassword (string password)
		{
			return password.Length >= 8;
		}
		
	}
}
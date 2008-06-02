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
using System.Text.RegularExpressions;
using Gtk;
using Do.Addins;

namespace GMailContacts
{	
	public partial class Configuration : AbstractLoginWidget
	{
		
		public Configuration() : 
			base ("Gmail")
		{
			this.Build();
			
			string username, password;
			
			GMail.GetUserAndPassFromKeyring (out username, out password,
			                                GMail.GAppName);
			Username.Text = username;
			Password.Text = password;
			
			GetAccountButton.Uri = "https://www.google.com/accounts/NewAccount?service=cl";
		}
		
		protected override void Validate ()
		{
			string username, password;
			username = Username.Text;
			password = Password.Text;
			
			if (ValidateUsername (username) && ValidatePassword (password))
			{
				StatusLabel.Markup = "Validating...";
				ValidateButton.Sensitive = false;
			
				new Thread ((ThreadStart) delegate {
					bool valid = GMail.TryConnect (username, password);
					
					Gtk.Application.Invoke (delegate {
						if (valid) {
							StatusLabel.Markup = "<i>Account validation succeeded</i>!";
							GMail.WriteAccountToKeyring (username, password, GMail.GAppName);
						} else {
							StatusLabel.Markup = "<i>Account validation failed!</i>";
						}
						ValidateButton.Sensitive = true;
					});
				}).Start ();
			}
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

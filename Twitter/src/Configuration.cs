/* Configuration2.cs
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
using Do.Addins;
using Gtk;

namespace DoTwitter
{
	public partial class Configuration : AbstractLoginWidget
	{
		public Configuration () : 
			base ("Twitter")
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
			
			base.Username.Text = username;
			base.Password.Text = password;
			
			base.GetAccountButton.Uri = "https://twitter.com/signup";			
		}
		
		protected override void Validate ()
		{
			string username = base.Username.Text;
			string password = base.Password.Text;
			
			base.StatusLabel.Markup = "Validating...";
			base.ValidateButton.Sensitive = false;
			
			new Thread ((ThreadStart) delegate {
				bool valid = TwitterAction.TryConnect (username, password);
				
				Gtk.Application.Invoke (delegate {
					if (valid) {
						base.StatusLabel.Markup = "<i>Account validation succeeded</i>!";
						TwitterAction.SetAccountData (username, password);
					} else {
						base.StatusLabel.Markup = "<i>Account validation failed!</i>";
					}
					base.ValidateButton.Sensitive = true;
				});
			}).Start ();
		}
	}
}

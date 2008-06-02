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
using Do.UI;

namespace GMailContacts
{	
	public partial class Configuration : AbstractLoginWidget
	{
		
		public Configuration() : 
			base ("Gmail")
		{
			GetAccountButton.Uri = "https://www.google.com/accounts/NewAccount?service=cl";
		}
		
		protected override bool Validate ()
		{
			if (ValidateUsername (UsernameEntry.Text) && PasswordEntry.Text.Length >= 8)
				return GMail.TryConnect (UsernameEntry.Text, PasswordEntry.Text);
			return false;
		}
		
		private bool ValidateUsername (string username)
		{
			const string emailPattern = @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\."
            + @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*"
            + @"[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?";
			
			Regex validEmail = new Regex (emailPattern, RegexOptions.Compiled);
			return validEmail.IsMatch (username);
		}
	}
}

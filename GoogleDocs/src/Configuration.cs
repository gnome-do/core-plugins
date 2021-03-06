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

using System;
using System.Text.RegularExpressions;

using Gtk;
using Mono.Addins;

using Do.Platform;
using Do.Platform.Linux;

namespace GDocs
{
	public class Configuration : AbstractLoginWidget
	{
		const string EmailPattern = @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\."
			+ @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*"
			+ @"[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?";
		const string Uri = "https://www.google.com/accounts/NewAccount?service=Writely";

		public Configuration () : base ("Google Docs", Uri)
		{
			UsernameLabel = AddinManager.CurrentLocalizer.GetString ("E-Mail:");
			Username = GDocs.Preferences.Username;
			Password = GDocs.Preferences.Password;
		}

		protected override bool Validate (string username, string password)
		{
			return ValidateUsername (username) &&
				0 < password.Length &&
				GDocs.Connect (username, password);
		}

		private bool ValidateUsername (string username)
		{
			return new Regex (EmailPattern, RegexOptions.Compiled)
				.IsMatch (username);
		}

		protected override void SaveAccountData (string username, string password)
		{
			//Log.Error ("Account data not saved");
			GDocs.Preferences.Username = username;
			GDocs.Preferences.Password = password;
		}
	}
}

// Configuration.cs
// 
// GNOME Do is the legal property of its developers, whose names are too
// numerous to list here.  Please refer to the COPYRIGHT file distributed with
// this source distribution.
// 
// This program is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free Software
// Foundation, either version 3 of the License, or (at your option) any later
// version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
// details.
// 
// You should have received a copy of the GNU General Public License along with
// this program.  If not, see <http://www.gnu.org/licenses/>.
//
using System;
using System.Linq;
using System.Collections.Generic;

using Gtk;

using Twitterizer.Framework;

using Do.Platform.Linux;

namespace Microblogging
{
	public class Configuration : AbstractLoginWidget
	{
		static Dictionary<Service, string> register_links;
		public static event EventHandler ServiceChanged;

		ComboBox services_combo;
		
		static Configuration ()
		{
			register_links = new Dictionary<Service, string> ();
			SetupServiceLinks ();
		}
		
		public Configuration () : 
			base (Microblog.Preferences.MicroblogService, register_links[Microblog.Preferences.ActiveService])
		{
			//insert service combobox at top of page	
			services_combo = new ComboBox (register_links.Keys.Select (item => Microblog.Preferences.GetServiceName (item)).ToArray ());
			services_combo.Active = (int) Microblog.Preferences.ActiveService;
			services_combo.Changed += OnServiceComboChanged;
			InsertWidgetAtTop (services_combo);

			Username = Microblog.Preferences.Username;
			Password = Microblog.Preferences.Password;
			
		}
		
		protected override bool Validate (string username, string password)
		{
			return Microblog.Connect (username, password);
		}

		protected override void SaveAccountData (string username, string password)
		{
			Microblog.Preferences.Username = username;
			Microblog.Preferences.Password = password;
		}
		
		protected virtual void OnServiceComboChanged (object sender, EventArgs e)
		{
			// update current service
			Microblog.Preferences.MicroblogService = services_combo.ActiveText;
			
			Username = Microblog.Preferences.Username;
			Password = Microblog.Preferences.Password;
			ChangeService (Microblog.Preferences.MicroblogService, register_links [Microblog.Preferences.ActiveService]);
			
			if (ServiceChanged != null)
            	ServiceChanged(this, e);
		}

		static void SetupServiceLinks ()
		{
			register_links.Add (Service.Twitter, "https://twitter.com/signup");
			register_links.Add (Service.Identica, "http://identi.ca/main/register");
		}
	}
}

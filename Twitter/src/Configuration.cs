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
using System.Collections.Generic;

using Twitterizer.Framework;

using Do.UI;

namespace Microblogging
{
	public class Configuration : AbstractLoginWidget
	{
		static Dictionary<Service, string> register_links;
		
		static Configuration ()
		{
			register_links = new Dictionary<Service, string> ();
			register_links.Add (Service.Twitter, "https://twitter.com/signup");
			register_links.Add (Service.Identica, "http://identi.ca/main/register");
		}
			
		public Configuration () : 
			base (Microblog.Preferences.MicroblogService)
		{
			GenConfig.ServiceChanged += ServiceChanged;
			GetAccountButton.Uri = register_links[Microblog.ActiveService];
		}
		
		protected override bool Validate (string username, string password)
		{
			return Microblog.Connect (username, password);
		}

		protected void ServiceChanged (object o, EventArgs e)
		{
			// TODO: the AbstractLoginWidget has a shortcoming here with geting account data, this should
			// be a job for secure preferences anyway. For now you just need to update the account data
			// manually.
			GetAccountLabel.Markup = string.Format ("<i>Don't have {0}?</i>", Microblog.ActiveService);
			GetAccountButton.Label = string.Format ("Sign up for {0}", Microblog.ActiveService);
			GetAccountButton.Uri = register_links[Microblog.ActiveService];
		}
	}
}

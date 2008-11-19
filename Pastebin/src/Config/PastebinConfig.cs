//  PastebinConfig.cs
//
//  GNOME Do is the legal property of its developers, whose names are too
//  numerous to list here.  Please refer to the COPYRIGHT file distributed with
//  this source distribution.
//
//  This program is free software: you can redistribute it and/or modify it
//  under the terms of the GNU General Public License as published by the Free
//  Software Foundation, either version 3 of the License, or (at your option)
//  any later version.
//
//  This program is distributed in the hope that it will be useful, but WITHOUT
//  ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
//  FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
//  more details.
//
//  You should have received a copy of the GNU General Public License along with
//  this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using Do.Addins;

namespace Pastebin
{	
	public partial class PastebinConfig : Gtk.Bin
	{	
		static IPreferences prefs;
		
		public PastebinConfig ()
		{
			this.Build();
			
			switch (SelectedProviderType) {
			case "Pastebin.Paste2":
				Paste2RadioButton.Active = true;
				break;
			case "Pastebin.PastebinCA":
				PastebinCARadioButton.Active = true;
				break;
			case "Pastebin.LodgeIt":
				LodgeItRadioButton.Active = true;
				break;
			default:
				Paste2RadioButton.Active = true;
				break;
			}
		}	
		
		public static string SelectedProviderType {
			get { return prefs.Get<string> ("SelectedProviderType", typeof(Paste2).ToString()); }
			set { prefs.Set<string> ("SelectedProviderType", value); }
		}
		
		static PastebinConfig ()
		{
			prefs = Do.Addins.Util.GetPreferences ("Pastebin");
		}

		protected virtual void OnPaste2Toggled (object sender, System.EventArgs e)
		{
			SelectedProviderType = typeof(Paste2).ToString();
		}

		protected virtual void OnPastebinCAToggled (object sender, System.EventArgs e)
		{
			SelectedProviderType = typeof(PastebinCA).ToString();
		}

		protected virtual void OnLodgeItToggled (object sender, System.EventArgs e)
		{
			SelectedProviderType = typeof(LodgeIt).ToString();
		}
	}
}

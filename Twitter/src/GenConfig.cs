/* GenConfig.cs
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
using System.Collections.Generic;

using Gtk;

namespace Twitter
{
	public partial class GenConfig : Gtk.Bin
	{		
		public GenConfig ()
		{
			this.Build();
			show_updates_chk.Active = Twitter.Preferences.ShowNotifications;
			
			foreach (string key in Twitter.AvailableServices.Keys) {
				service_combo.AppendText (key);
			}
		}
		
		protected virtual void OnShowUpdatesChkClicked (object sender, System.EventArgs e)
		{
			Twitter.Preferences.ShowNotifications = show_updates_chk.Active;
		}

		protected virtual void OnServiceComboChanged (object sender, System.EventArgs e)
		{
			Twitter.Preferences.MicroblogService = service_combo.ActiveText;
			Twitter.ChangeService (service_combo.ActiveText);
		}
	}
}
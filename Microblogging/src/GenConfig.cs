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

namespace Microblogging
{
	public partial class GenConfig : Gtk.Bin
	{
		public static event EventHandler ServiceChanged;

		public GenConfig ()
		{
			this.Build();
			show_updates_chk.Active = Microblog.Preferences.ShowNotifications;
			
			foreach (string service in Microblog.Preferences.AvailableServices) {
				service_combo.AppendText (service);
			}

			service_combo.Active = (int) Microblog.Preferences.ActiveService;
		}
		
		protected virtual void OnShowUpdatesChkClicked (object sender, EventArgs e)
		{
			Microblog.Preferences.ShowNotifications = show_updates_chk.Active;
		}

		protected virtual void OnServiceComboChanged (object sender, EventArgs e)
		{
			// TODO: we need to make the account config regenerate when this gets changed
			Microblog.Preferences.MicroblogService = service_combo.ActiveText;
			OnServiceChanged (e);
		}

		protected virtual void OnServiceChanged (EventArgs e)
		{
        	if (ServiceChanged != null)
            	ServiceChanged(this, e);
		}
	}
}
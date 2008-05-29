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
using Gtk;
using Do.Addins;

namespace GCalendar
{
	public partial class Configuration : Gtk.Bin
	{		
		public Configuration()
		{
			this.Build();
		}

		protected virtual void OnNewAcctClicked (object sender, System.EventArgs e)
		{
			Util.Environment.Open ("https://www.google.com/accounts/NewAccount?service=cl");
		}

		protected virtual void OnUsernameEntryEditingDone (object sender, System.EventArgs e)
		{
			Console.Error.WriteLine (username_entry.Text);
		}
	}
}

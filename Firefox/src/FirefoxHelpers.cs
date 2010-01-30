//  FirefoxHelpers.cs
//  GNOME Do is the legal property of its developers, whose names are too
//  numerous to list here.  Please refer to the COPYRIGHT file distributed with
//  this source distribution.
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using Gtk;

namespace Firefox
{
	public static class FirefoxHelpers
	{
		public static string IconName {get; private set;}
		
		static FirefoxHelpers ()
		{
			// List a whole bunch of possible names for the firefox icon.
			string[] iconNames = new string [] {
				"firefox",
				"iceweasel",
				"firefox-4.0",
				"firefox-3.9",
				"firefox-3.8",
				"firefox-3.7",
				"firefox-3.6",
				"firefox-3.5",
				"firefox-3.0",
			};
			
			// Start with "firefox".  If we can't find an icon in the theme,
			// at least "firefox" makes our intent clear.
			IconName = "firefox";
			foreach (string iconName in iconNames) {
				if (IconTheme.Default.HasIcon (iconName)) {
					IconName = iconName;
					break;
				}
			}
		}
	}
}

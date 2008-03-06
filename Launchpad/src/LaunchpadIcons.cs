/* LaunchpadIcons.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this source distribution.
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
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Do.Addins;
using Do.Universe;

namespace Do.Launchpad
{
	/// <summary>
	/// Singleton class for saving and storing Launchpad icons.
	/// When an icon is requested, it is extracted from the resources file
	/// and saved to a temp directory, and the path to that icon is returned.
	/// Upon destruction, the class will clear out all its icons.
	/// </summary>
	class LaunchpadIcons
	{
		public static LaunchpadIcons instance = null;
		private Dictionary<string, string> iconcache;
		Assembly asm;

		private LaunchpadIcons()
		{
			iconcache = new Dictionary<string, string>();
			asm = Assembly.GetExecutingAssembly();
		}

		public static LaunchpadIcons Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new LaunchpadIcons();
				}
				return instance;
			}
		}

		~LaunchpadIcons()
		{
			foreach (string s in iconcache.Keys)
			{
				try
				{
					if (File.Exists(iconcache[s]))
					{
						File.Delete(iconcache[s]); //TODO: be smarter about this.
					}
				}
				catch (Exception)
				{
				}
			}
		}

		public string GetIconPath(string iconName)
		{
			if (false == iconcache.ContainsKey(iconName))
			{
				//Extract the icon from the resource file.
				Stream icon = asm.GetManifestResourceStream(iconName);
				//TODO: Use a facility within gnome-do's core for creating
				//temporary icons
				if (false == Directory.Exists("/tmp/gnome-do"))
					Directory.CreateDirectory("/tmp/gnome-do");
				if (false == Directory.Exists("/tmp/gnome-do/icons"))
					Directory.CreateDirectory("/tmp/gnome-do/icons");
				string tmp_filename = Path.Combine("/tmp/gnome-do/icons", iconName);
				BinaryReader input = new BinaryReader(icon);
				BinaryWriter output = new BinaryWriter(File.OpenWrite(tmp_filename));
				
				int try_read;
				while (true) {
					try {
						try_read = input.ReadInt32();
						output.Write(try_read);
					} catch (Exception) {
						break;
					}
				}

				input.Close();
				output.Close();

				iconcache[iconName] = tmp_filename;
			}
			return iconcache[iconName];
		}
	}
}

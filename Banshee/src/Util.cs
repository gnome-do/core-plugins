/* Util.cs 
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
using System.Diagnostics;

using Mono.Unix;

using Do.Platform;
using Do.Universe;

namespace Banshee
{
	
	public static class Util
	{
		// TODO: use an IApplicationItem instead of this hardcoded string.
		const string BansheeBin = "banshee-1";
		const string MinBansheeVersion = "4.2"; // 1 is assumed.

		public static string UnsupportedVersionMessage {
			get { return string.Format (Catalog.GetString ("Banshee Version is unsupported. Banshee {0} is newer is required to index collection"),
				MinBansheeVersion);
			}
		}
		
		public static bool VersionSupportsIndexing ()
		{
			string stdout, version;
			
			Process banshee = new Process ();
			banshee.StartInfo.FileName = BansheeBin;
			banshee.StartInfo.Arguments = "--version";
			banshee.StartInfo.UseShellExecute = false;

			banshee.Start ();
			banshee.WaitForExit ();
			stdout = banshee.StandardOutput.ReadToEnd ();

			version = stdout.Substring (stdout.IndexOf (MinBansheeVersion), MinBansheeVersion.Length);
			return int.Parse (version) >= int.Parse (MinBansheeVersion);
		}
	}
}

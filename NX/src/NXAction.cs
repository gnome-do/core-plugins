/* SSHAction.cs
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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Do;
using Do.Universe;
using System.Diagnostics;

using Mono.Addins;

namespace NX 
{
    public class NXAction : Act
    {
	
        public override string Name {
            get { return AddinManager.CurrentLocalizer.GetString ("Connect with NX"); }
        }
	
        public override string Description {
            get { return AddinManager.CurrentLocalizer.GetString ("Connect with NX"); }
        }
	
        public override string Icon {
            get { return "network-server"; }
        }
	
        public override IEnumerable<Type> SupportedItemTypes {
            get { yield return typeof (NXHostItem); }
        }
	

        public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems) 
        {
            EnsureInPath ("/usr/NX/bin");
            
            string exec = "nxclient";
            NXHostItem hostitem = items.First () as NXHostItem;

            Process nxclient = new Process ();
            nxclient.StartInfo.FileName = exec;
            nxclient.StartInfo.Arguments = "--session " + hostitem.Path;
            nxclient.Start ();
            
            yield break;
        }

		void EnsureInPath(string path) 
		{
            char[] splitters = {':'};
            String[] paths = Environment.GetEnvironmentVariable ("PATH").Split (splitters);
            if (Array.IndexOf (paths, path) < 0) {
                Environment.SetEnvironmentVariable ("PATH", Environment.GetEnvironmentVariable ("PATH") + ":" + path);
            }
        }
    }
}

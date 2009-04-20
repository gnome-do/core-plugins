/* VNCHost.cs

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

using Do.Universe;

namespace VinagreVNC 
{  
    public class HostItem : Item  
    {
        public HostItem (string bookmark, string hostname, string port)
        {
        	port = port;
            Bookmark = bookmark;
            Hostname = hostname;
        }

        public override string Name { 
            get { return Bookmark; }
        }

        public override string Description { 
            get { return Hostname; }
        }

        public override string Icon { 
            get { return "gnome-globe"; } 
        }
        
        public string Hostname { get; private set; }

        public string Bookmark { get; private set; }
        
        public string Port { get; private set; }
    }
}

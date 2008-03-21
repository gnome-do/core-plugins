// apturl.cs 
//
// Copyright (C) 2008 Christer Edwards <christer.edwards@ubuntu.com>

// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//

using System;
using Do.Universe;

namespace apturl {

    public class APTAction : AbstractAction {

        public override string Name {
            get { return "Install with apturl";  }
        }

        public override string Description {
            get { return "Use apturl to install a package"; }
        }

        public override string Icon {
            get { return "update-manager"; }
        }

        public override Type[] SupportedItemTypes {
            get {
                return new Type[] {
                    typeof(ITextItem)
                };
            }
        }

        public override IItem[] Perform (IItem[] items, IItem[] modItems)
        {
            string package = (items [0] as ITextItem).Text;

            System.Diagnostics.Process.Start ("apturl apt:" + package);
            return null;
        }
    }
}

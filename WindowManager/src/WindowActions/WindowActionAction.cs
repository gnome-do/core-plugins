// WindowSwitchAction.cs
//
//GNOME Do is the legal property of its developers. Please refer to the
//COPYRIGHT file distributed with this
//source distribution.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading;

using Do.Universe;
using Do.Interface.Wink;

using Wnck;
using Mono.Unix;

namespace WindowManager
{
	//This code can't go without acknowledging the woman who made it all
	//possible.  I know this file in itself is not much but it seemed
	//like a good place to put this.  You have always supported me Kristin.
	//You have put up with the late nights of me hacking away at this and many
	//many other parts of Do.  You showed amazing patience and you
	//have even showed interest from time to time.  I love you baby girl.
	//Without you I never would have gotten this far.
	
	public abstract class WindowActionAction : Act
	{
		public override bool ModifierItemsOptional {
			get { return true; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { return new Type[] {
					typeof (IApplicationItem),
					typeof (IWindowItem),
				};
			}
		}

		public override bool SupportsItem (Item item)
		{
			if (item is IApplicationItem) {
				string application = (item as IApplicationItem).Exec;
				application = application.Split (new char[] {' '})[0];
			
				return WindowUtils.WindowListForCmd (application).Any ();
			} else if (item is WindowItem) {
				return true;
			}
			return false;
		}
	}
}

/* RunInTerminalAction.cs
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Mono.Unix;

using Do.Universe;
using Do.Platform;

namespace GNOME.Terminal
{
	public class RunInTerminalAction : Act
	{

		public RunInTerminalAction()
		{
		}

		public override string Name {
	      get { return Catalog.GetString ("Run in Terminal"); }
	    }

	    public override string Description {
	      get { return Catalog.GetString ("Runs a command in GNOME Terminal."); }
	    }

	    public override string Icon {
	      get { return "gnome-terminal"; }
	    }

	    public override IEnumerable<Type> SupportedItemTypes {
	      get {
	      	yield return typeof (ITextItem);
	      	yield return typeof (IFileItem);
	      }
	    }

		public override bool SupportsItem (Item item)
		{
			if (item is ITextItem) {
				return Services.Environment.IsExecutable ((item as ITextItem).Text);
			} else if (item is IFileItem) {
				return Services.Environment.IsExecutable ((item as IFileItem).Path);
			}
			return false;
		}

	    public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems)
	    {
			foreach (Item item in items) {
				string cmd = "";
				if (item is ITextItem) {
					cmd = (item as ITextItem).Text;
				} else if (item is IFileItem) {
					cmd = (item as IFileItem).Path;
				}

				try {
					Process.Start ("gnome-terminal", "-x " + cmd);
				} catch (Exception e) {
					Log.Error ("Could not open gnome-terminal with command {0}: {1}", cmd, e.Message );
					Log.Debug (e.StackTrace);
				}
			}
			yield break;
	    }
	}
}

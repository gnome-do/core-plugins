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

namespace GNOME.Terminal
{
	public class RunInTerminalAction : Act
	{
		static string last_command_found;
		
		public RunInTerminalAction()
		{
		}

		public override string Name
		{
	      get { return Catalog.GetString ("Run in Terminal"); }
	    }

	    public override string Description
		{
	      get { return Catalog.GetString ("Runs a command in GNOME Terminal."); }
	    }

	    public override string Icon
		{
	      get { return "gnome-terminal"; }
	    }

	    public override IEnumerable<Type> SupportedItemTypes
		{
	      get {
	      	return new Type[] {
	      		typeof (ITextItem),
	      		typeof (IFileItem),
	      	};
	      }
	    }

		public override bool SupportsItem (Item item)
		{
			if (item is ITextItem) {
				return CommandLineFoundOnPath ((item as ITextItem).Text);
			} else if (item is IFileItem) {
				return IFileItem.IsExecutable (item as IFileItem);
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
					Process.Start ("gnome-terminal", "-x " + cmd );
				} catch (Exception e) {
					Console.Error.WriteLine ("Could not open gnome-terminal with command {0}: {1}",
						cmd, e.Message );
				}
			}
			return null;
	    }

		public static bool CommandLineFoundOnPath (string command_line)
		{
			string path, command, command_file;
			int space_position;
			
			if (command_line == null) return false;
			
			command = command_line.Trim ();			
			space_position = command.IndexOf (" ");
			if (space_position > 0) {
				command = command.Substring (0, space_position);
			}

			// If this command is the same as the last, yes.
			if (command == last_command_found) return true;
			
			// If the command is found, fine.
			if (System.IO.File.Exists (command)) {
				last_command_found = command;
				return true;
			}
			
			// Otherwise, try to find the command file in path.
			path = System.Environment.GetEnvironmentVariable ("PATH");
			if (path != null) {
				foreach (string part in path.Split (':')) {
					command_file = System.IO.Path.Combine (part, command);
					if (System.IO.File.Exists (command_file)) {
						last_command_found = command;
						return true;
					}
				}
			}
			return false;
		}
	}
}

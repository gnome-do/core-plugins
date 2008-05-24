// RunInTerminalAction.boo
//
// GNOME Do is the legal property of its developers. Please refer to the
// COPYRIGHT file distributed with this
// source distribution.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace GNOME

import System
import System.IO
import System.Diagnostics
import Do.Universe

class RunInTerminalAction (AbstractAction):

	last_command_found as string

	Name as string:
		get:
			return "Run in Terminal"
	
	Description as string:
		get:
			return "Runs a command in GNOME Terminal."
	
	Icon as string:
		get:
			return "gnome-terminal"
	
	SupportedItemTypes as (Type):
		get:
			return (typeof (ITextItem),
					typeof (FileItem),)
	
	def SupportsItem (item as IItem) as bool:
		if item isa ITextItem:
			return CommandLineFoundOnPath ((item as ITextItem).Text)
		elif item isa FileItem:
			return FileItem.IsExecutable (item as FileItem)
		return false

	def Perform (items as (IItem), modItems as (IItem)) as (IItem):
		for item in items:
			cmd = ""
			if item isa ITextItem:
				cmd = (item as ITextItem).Text
			elif item isa IFileItem:
				cmd = (item as IFileItem).Path
			Process.Start ("gnome-terminal -x ${cmd}")
		return null

	def CommandLineFoundOnPath ([required] cmd as string) as bool:
		cmd = cmd.Split (char (' '))[0]
		if cmd == last_command_found: return true
		
		// If the command is found, fine.
		if File.Exists (cmd):
			last_command_found = cmd
			return true
		
		// Otherwise, try to find the command file in path.
		path = Environment.GetEnvironmentVariable ("PATH") or ""
		for p in path.Split (char (':')):
			if File.Exists (Path.Combine (p, cmd)):
				last_command_found = cmd
				return true
		return false



// OpenTerminalHereAction.boo
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
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace GNOME

import System
import Do.Universe

class OpenTerminalHereAction (AbstractAction):
		
	Name as string:
		get:
			return "Open Terminal Here"
		
	Description as string:
		get:
			return "Opens a Terminal in a given location."
		
	Icon as string:
		get:
			return "gnome-terminal"
		
	SupportedItemTypes as (Type):
		get:
			return (typeof (IFileItem),)
		
	def Perform (items as (IItem), modItems as (IItem)) as (IItem):
		fi = items [0] as IFileItem
		dir = fi.Path
		unless FileItem.IsDirectory (fi):
			dir = System.IO.Path.GetDirectoryName (dir)
		term = System.Diagnostics.Process ()
		term.StartInfo.WorkingDirectory = dir
		term.StartInfo.FileName = "gnome-terminal"
		term.Start ()
		return null

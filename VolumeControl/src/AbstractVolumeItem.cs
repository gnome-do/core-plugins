//  AbstractVolumeItem.cs
//  
//  GNOME Do is the legal property of its developers, whose names are too numerous
//  to list here.  Please refer to the COPYRIGHT file distributed with this
//  source distribution.
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.Diagnostics;

using Do.Platform;
using Do.Universe;

namespace VolumeControl
{
	
	
	public abstract class AbstractVolumeItem : Item, IRunnableItem
	{

		protected abstract string VolumeArgument { get; }
		
		public void Run ()
		{
			try {
				Process amixer = new Process ();
				amixer.StartInfo.FileName = "amixer";
				amixer.StartInfo.Arguments = string.Format ("set Master {0} > /dev/null", VolumeArgument);
				amixer.Start ();
			} catch (Exception e) {
				Log<AbstractVolumeItem>.Error ("Failed to launch amixer: {0}", e.Message);
				Log<AbstractVolumeItem>.Debug (e.StackTrace);
			}
		}
	}
}

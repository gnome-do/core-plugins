//  RhythmboxPlayCommand.cs
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

using System;
using System.Threading;
using System.Diagnostics;

using Do.Universe;

namespace Do.Addins.Rhythmbox
{

	public class RhythmboxPlayCommand : ICommand
	{
		
		public RhythmboxPlayCommand ()
		{
		}
		
		public string Name {
			get { return "Play"; }
		}
		
		public string Description {
			get { return "Play an item in Rhythmbox."; }
		}
		
		public string Icon {
			get { return "rhythmbox"; }
		}
		
		public Type[] SupportedItemTypes {
			get {
				return new Type[] {
					typeof (MusicItem),
				};
			}
		}
		
		public Type[] SupportedModifierItemTypes {
			get { return null; }
		}

		public bool SupportsItem (IItem item) {
			return true;
		}
		
		public bool SupportsModifierItemForItems (IItem[] items, IItem modItem)
		{
			return false;
		}
		
		public void Perform (IItem[] items, IItem[] modifierItems)
		{
			new Thread ((ThreadStart) delegate {
				Rhythmbox.StartIfNeccessary ();
				
				Rhythmbox.Client ("--clear-queue --no-present", true);
				foreach (IItem item in items) {
					string enqueue;
					
					enqueue = "--no-present ";
					foreach (SongMusicItem song in Rhythmbox.LoadSongsFor (item as MusicItem))
						enqueue += string.Format ("--enqueue \"{0}\" ", song.File);
					Rhythmbox.Client (enqueue, true);
				}
				Rhythmbox.Client ("--next --play --no-present");
			}).Start ();
		}
	}
}

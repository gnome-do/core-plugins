// BansheeAction.cs
//
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
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

using System;
using System.Threading;
using System.Collections.Generic;
using Do.Universe;
using Do.Addins;

namespace Do.Addins.Banshee {

        public class BansheePlayAction : AbstractAction {

                public override string Name {
                        get { return "Play"; }
                }
                public override string Description {
                        get { return "Play an item in Banshee."; }
                }
                public override string Icon {
                        get { return "music-player-banshee"; }
                }

                public override IEnumerable<Type> SupportedItemTypes {
                        get {
                                return new Type[] { typeof (MusicItem), };
                        }
                }

                public override bool SupportsModifierItemForItems (IEnumerable<IItem> items, IItem modItem)
                {
                        return false;
                }

                public override IEnumerable<IItem> Perform (IEnumerable<IItem> items, IEnumerable<IItem> modItems)
                {
                        new Thread ((ThreadStart) delegate {
                                BansheeDBus b = new BansheeDBus();
                                List<string> filenames = new List<string>();
                                foreach (IItem item in items) {
                                        foreach (SongMusicItem song in Banshee.LoadSongsFor (item as MusicItem))
                                                filenames.Add(song.File);
                                }
                                b.Enqueue(filenames.ToArray());
                        }).Start ();
                return null;
                }
        }
}

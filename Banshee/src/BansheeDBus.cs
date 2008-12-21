// BansheeDBus.cs
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
//

using System;
using System.Collections;
using NDesk.DBus;
using org.freedesktop.DBus;


namespace Do.Addins.Banshee {

        [Interface("org.gnome.Banshee.Core")]
        public interface IBansheePlayer {
                void Play();
                void Pause();
                void Next();
                void Previous();
                void TogglePlaying();
                void EnqueueFiles(string[] uris);
        }

        public class BansheeDBus {
                private const string OBJECT_PATH = "/org/gnome/Banshee/Player";
                private const string BUS_NAME = "org.gnome.Banshee";
                private static IBansheePlayer player_instance = null;

                static private IBansheePlayer FindInstance()
                {
                    if(!Bus.Session.NameHasOwner(BUS_NAME)) {
                            Bus.Session.StartServiceByName(BUS_NAME);
                            System.Threading.Thread.Sleep(5000);
                            if(!Bus.Session.NameHasOwner(BUS_NAME))
                                    throw new Exception(String.Format("Name {0} has no owner.", BUS_NAME));
                    }
                    return Bus.Session.GetObject<IBansheePlayer>(BUS_NAME, new ObjectPath(OBJECT_PATH));
                }

                public BansheeDBus()
                {
                    try {
                        player_instance = FindInstance();
                    }
                    catch (Exception) {
                        Console.Error.WriteLine("Could not locate Banshee on D-Bus. Perhaps it's not running?");
                    }
                    BusG.Init();
                }

                public void TogglePlaying()
                {
                    player_instance.TogglePlaying();
                }

                public void Next()
                {
                    player_instance.Next();
                }

                public void Previous()
                {
                    player_instance.Previous();
                }

                public void Enqueue(string[] uris)
                {
                    player_instance.EnqueueFiles(uris);
                }
        }
}

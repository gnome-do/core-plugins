/* BansheeDBus.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
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

using Do.Addins;
using Do.Universe;

using NDesk.DBus;
using org.freedesktop.DBus;

namespace Banshee1
{
	[Interface ("org.bansheeproject.Banshee.PlayerEngine")]
	public interface IBansheePlayer {
		void TogglePlaying ();
	}
	
	[Interface ("org.bansheeproject.Banshee.PlayQueue")]
	public interface IBansheePlayQueue {
		void EnqueueUri (string uri);
		void EnqueueUri (string uri, bool prepend);
	}
	
	[Interface ("org.bansheeproject.Banshee.PlaybackController")]
	public interface IBansheeController {
		void Next (bool restart);
		void Previous (bool restart);
	}
	
	public class BansheeDBus
	{
		const string BUS_NAME = "org.bansheeproject.Banshee";
		static Dictionary<string, string> object_paths;
		
		static IBansheePlayer player;
		static IBansheePlayQueue queue;
		static IBansheeController controller;
		
		static T GetIBansheeObject<T> (string object_path)
		{
			if (!Bus.Session.NameHasOwner (BUS_NAME)) {
				Bus.Session.StartServiceByName (BUS_NAME);
				System.Threading.Thread.Sleep (5000);
				if (!Bus.Session.NameHasOwner (BUS_NAME))
					throw new Exception (string.Format("Name {0} has no owner.", BUS_NAME));
			}
			
			return Bus.Session.GetObject<T> (BUS_NAME, new ObjectPath (object_path));
		}
		
		static IBansheePlayer Player {
			get {
				return player ?? 
					player = GetIBansheeObject<IBansheePlayer> (object_paths["player"]);
			}
		}
		
		static IBansheePlayQueue PlayQueue {
			get {
				return queue ?? 
					queue = GetIBansheeObject<IBansheePlayQueue> (object_paths["queue"]);
			}
		}	
		
		static IBansheeController Controller {
			get {
				return controller ??
					controller = GetIBansheeObject<IBansheeController> (object_paths["controller"]);
			}
		}
				
		static BansheeDBus ()
		{
			BuildObjectPathsDict ();
		}
		
		public void TogglePlaying ()
		{
			try {
				Player.TogglePlaying ();
			} catch (Exception e) {
				Log.Error ("Encountered a problem in Enqueue. {0}.", e.Message);
			}
		}

		public void Enqueue (string[] uris)
		{
			Enqueue (uris, false);
		}
		
		public void Enqueue (string[] uris, bool prepend)
		{
			try {
				if (prepend)
					Array.Reverse (uris);
				
				foreach (string uri in uris)
					PlayQueue.EnqueueUri (uri,prepend);
				
			} catch (Exception e) {
				Log.Error ("Encountered a problem in Enqueue. {0}.", e.Message);
			}
		}
		
		public void Next ()
		{
			try {
				Controller.Next (false);
			} catch (Exception e) {
				Log.Error ("Encountered a problem in Next. {0}.", e.Message);
			}
		}
		
		public void Previous ()
		{
			try {
				Controller.Previous (false);
			} catch (Exception e) {
				Log.Error ("Encountered a problem in Next. {0}.", e.Message);
			}
		}
		
		static void BuildObjectPathsDict ()
		{
			object_paths = new Dictionary<string,string> ();
			object_paths.Add ("player", "/org/bansheeproject/Banshee/PlayerEngine");
			object_paths.Add ("queue", "/org/bansheeproject/Banshee/SourceManager/PlayQueue");
			object_paths.Add ("controller", "/org/bansheeproject/Banshee/PlaybackController");
		}
	}
}

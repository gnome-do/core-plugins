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
using System.Linq;
using System.Collections.Generic;

using NDesk.DBus;
using org.freedesktop.DBus;

using Do.Platform;

namespace Banshee
{
	[Interface ("org.bansheeproject.Banshee.PlayerEngine")]
	interface IBansheePlayer {
		void Pause ();
		void TogglePlaying ();
		string CurrentState { get; }
	}
	
	[Interface ("org.bansheeproject.Banshee.PlayQueue")]
	interface IBansheePlayQueue {
		void EnqueueUri (string uri, bool prepend);
	}
	
	[Interface ("org.bansheeproject.Banshee.PlaybackController")]
	interface IBansheeController {
		void Next (bool restart);
		void Previous (bool restart);
		int ShuffleMode { get; set; }
	}
	
	public class BansheeDBus
	{
		const string BusName = "org.bansheeproject.Banshee";

		# region static Banshee d-bus members
		
		static Dictionary<Type, string> object_paths;
		
		static IBansheePlayer player;
		static IBansheePlayQueue queue;
		static IBansheeController controller;

		static BansheeDBus ()
		{
			BuildObjectPathsDict ();
		}
		
		static T GetIBansheeObject<T> (string object_path)
		{
			if (!Bus.Session.NameHasOwner (BusName)) {
				Bus.Session.StartServiceByName (BusName);
				System.Threading.Thread.Sleep (5000);
				if (!Bus.Session.NameHasOwner (BusName))
					throw new Exception (string.Format("Name {0} has no owner.", BusName));
			}
			
			return Bus.Session.GetObject<T> (BusName, new ObjectPath (object_path));
		}
		
		static IBansheePlayer Player {
			get {
				return player ?? 
					player = GetIBansheeObject<IBansheePlayer> (object_paths [typeof (IBansheePlayer)]);
			}
		}
		
		static IBansheePlayQueue PlayQueue {
			get {
				return queue ?? 
					queue = GetIBansheeObject<IBansheePlayQueue> (object_paths  [typeof (IBansheePlayQueue)]);
			}
		}	
		
		static IBansheeController Controller {
			get {
				return controller ??
					controller = GetIBansheeObject<IBansheeController> (object_paths [typeof (IBansheeController)]);
			}
		}

		#endregion

		public PlaybackShuffleMode ShuffleMode { 
			get { return (PlaybackShuffleMode) Controller.ShuffleMode; }
			set { Controller.ShuffleMode = (int) value; }
		}

		public bool IsPlaying ()
		{
			try {
				return Player.CurrentState == "playing";
			} catch (Exception e) {
				LogError ("IsPlaying", e);
			}
			return false;
		}

		public void Pause ()
		{
			try {
				Player.Pause ();
			} catch (Exception e) {
				LogError ("Pause", e);
			}
		}
		
		public void TogglePlaying ()
		{
			try {
				Player.TogglePlaying ();
			} catch (Exception e) {
				LogError ("TogglePlaying", e);
			}
		}

		public void Play (IEnumerable<IMediaFile> media)
		{
			Enqueue (media, true);
			Next ();
		}			

		public void Enqueue (IEnumerable<IMediaFile> media)
		{
			Enqueue (media, false);
		}
		
		void Enqueue (IEnumerable<IMediaFile> media, bool prepend)
		{
			try {
				// if we're prepending to the queue we need to queue in the uris in reverse order
				if (prepend) media = media.Reverse ();

				media.ForEach (item => PlayQueue.EnqueueUri (item.Path, prepend));
				
			} catch (Exception e) {
				LogError ("Enqueue", e);
			}
		}
		
		public void Next ()
		{
			try {
				Controller.Next (false);
			} catch (Exception e) {
				LogError ("Next", e);
			}
		}
		
		public void Previous ()
		{
			try {
				Controller.Previous (false);
			} catch (Exception e) {
				LogError ("Previous", e);
			}
		}
		
		static void BuildObjectPathsDict ()
		{
			object_paths = new Dictionary<Type, string> ();
			object_paths.Add (typeof (IBansheePlayer), "/org/bansheeproject/Banshee/PlayerEngine");
			object_paths.Add (typeof (IBansheePlayQueue), "/org/bansheeproject/Banshee/SourceManager/PlayQueue");
			object_paths.Add (typeof (IBansheeController), "/org/bansheeproject/Banshee/PlaybackController");
		}

		void LogError (string methodName, Exception e)
		{
			Log<BansheeDBus>.Error ("Encountered a problem in {0}: {1}", methodName, e.Message);
		}
	}
}

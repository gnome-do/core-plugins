// BansheeDBus.cs
//
// GNOME Do is the legal property of its developers. Please refer to the
// COPYRIGHT file distributed with this source distribution.
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
//


using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

#if USE_DBUS_SHARP
using DBus;
#else
using NDesk.DBus;
#endif

using org.freedesktop.DBus;

using Do.Platform;

namespace Banshee
{

	[Interface ("org.bansheeproject.Banshee.PlayerEngine")]
	interface IBansheePlayer {
		void Play ();
		void Pause ();
		string CurrentState { get; }
	}
	
	[Interface ("org.bansheeproject.Banshee.PlayQueue")]
	interface IBansheePlayQueue {
		void EnqueueUri (string uri, bool prepend);
	}
	
	[Interface ("org.bansheeproject.Banshee.PlaybackController")]
	interface IBansheeController
	{
		void First ();
		void Next (bool restart);
		void Previous (bool restart);
		int ShuffleMode { get; set; }
	}
	
	public class BansheeDBus
	{
		const string BusName = "org.bansheeproject.Banshee";
		const string SessionBusName = "org.freedesktop.DBus";
		const string ErrorMessage = "Banshee encountered an error in {0}; {1}";

		# region static Banshee d-bus members
		
		static Dictionary<Type, string> object_paths;
		
		static IBus session_bus;
		static IBansheePlayer player;
		static IBansheePlayQueue play_queue;
		static IBansheeController controller;

		static BansheeDBus ()
		{
			BuildObjectPathsDict ();
			session_bus = Bus.Session.GetObject<IBus> (SessionBusName, new ObjectPath (object_paths[typeof (IBus)]));
			session_bus.NameOwnerChanged += HandleNameOwnerChanged;
		}

		static void HandleNameOwnerChanged(string name, string old_owner, string new_owner)
		{
			// when the owner changes on this path, we release our dbus objects.
			if (name == BusName) {
				player = null;
				controller = null;
				play_queue = null;
			}	
		}

		static bool FullApplicationAvailable {
			get { return Bus.Session.NameHasOwner (BusName); }
		}

		static void MaybeStartFullApplication ()
		{
			if (FullApplicationAvailable) return;
			
			Bus.Session.StartServiceByName (BusName);
			Thread.Sleep (5000);

			if (!FullApplicationAvailable)
				throw new Exception (string.Format("Name {0} has no owner.", BusName));
		}
		
		static T GetIBansheeObject<T> (string objectPath)
		{
			MaybeStartFullApplication ();
			return Bus.Session.GetObject<T> (BusName, new ObjectPath (objectPath));
		}
		
		static IBansheePlayer Player {
			get {
				player = ((player == null)
					? GetIBansheeObject<IBansheePlayer> (object_paths [typeof (IBansheePlayer)])
					: player);
				return player;
			}
		}

		static IBansheeController Controller {
			get {
				controller = ((controller == null) 
					? GetIBansheeObject<IBansheeController> (object_paths [typeof (IBansheeController)])
					: controller);
				return controller;
			}
		}

		static IBansheePlayQueue PlayQueue {
			get {
				play_queue = ((play_queue == null)
					? GetIBansheeObject<IBansheePlayQueue> (object_paths [typeof (IBansheePlayQueue)])
					: play_queue);
				return play_queue;
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
				return (player != null || FullApplicationAvailable) && Player.CurrentState == "playing";
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
		
		public void Play ()
		{
			try {
				Player.Play ();
			} catch (Exception e) {
				LogError ("Play", e);
			}
		}

		public void Play (IEnumerable<IMediaFile> media)
		{
			Enqueue (media, true);
			First ();
		}			

		public void Enqueue (IEnumerable<IMediaFile> media)
		{
			Enqueue (media, false);
		}
		
		void Enqueue (IEnumerable<IMediaFile> media, bool prepend)
		{
			try {
				MaybeStartFullApplication (); //if banshee isn't already started the enqueue seems to fail
				
				// if we're prepending to the queue we need to queue in the uris in reverse order
				if (prepend) 
					media = media.Reverse ();

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
		
		void First ()
		{
			try {
				Controller.First ();
			} catch (Exception e) {
				LogError ("First", e);
			}
		}
		
		static void BuildObjectPathsDict ()
		{
			object_paths = new Dictionary<Type, string> ();
			object_paths.Add (typeof (IBus), "/org/freedesktop/DBus");
			object_paths.Add (typeof (IBansheePlayer), "/org/bansheeproject/Banshee/PlayerEngine");
			object_paths.Add (typeof (IBansheeController), "/org/bansheeproject/Banshee/PlaybackController");
			object_paths.Add (typeof (IBansheePlayQueue), "/org/bansheeproject/Banshee/SourceManager/PlayQueue");
		}

		void LogError (string methodName, Exception e)
		{
			Log<BansheeDBus>.Error ("Encountered a problem in {0}: {1}", methodName, e.Message);
		}
	}
}

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
using System.Collections.Generic;

using NDesk.DBus;
using org.freedesktop.DBus;

using Do.Platform;

namespace Banshee
{

	[Interface ("org.bansheeproject.Banshee.PlayerEngine")]
	interface IBansheePlayer
	{
		void Play ();
		void Pause ();
		bool CanPause { get; }
	}
	
	[Interface ("org.bansheeproject.Banshee.PlaybackController")]
	interface IBansheeController
	{
		void Next (bool restart);
		void Previous (bool restart);
		int ShuffleMode { get; set; }
	}
	
	public class BansheeDbus
	{
		const string BusName = "org.bansheeproject.Banshee";
		const string ErrorMessage = "Banshee encountered an error in {0}; {1}";

		# region static Banshee d-bus members
		
		static Dictionary<Type, string> object_paths;
		
		static IBansheePlayer player;
		static IBansheeController controller;

		static BansheeDbus ()
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
				if (player == null)
					player = GetIBansheeObject<IBansheePlayer> (
						object_paths [typeof (IBansheePlayer)]);
				return player;
			}
		}

		static IBansheeController Controller {
			get {
				if (controller == null)
					controller = GetIBansheeObject<IBansheeController> (
						object_paths [typeof (IBansheeController)]);
				return controller;
			}
		}

		#endregion

		public PlaybackShuffleMode ShuffleMode { 
			get { return (PlaybackShuffleMode) Controller.ShuffleMode; }
			set { Controller.ShuffleMode = (int) value; }
		}

		public bool Playing {
			get { return Player.CanPause; }
		}

		public void Play ()
		{
			try {
				Player.Play ();
			} catch (Exception e) {
				Log.Error (ErrorMessage, "Play", e.Message);
				Log.Debug (e.StackTrace);
			}
		}

		public void Pause ()
		{
			try {
				Player.Pause ();
			} catch (Exception e) {
				Log.Error (ErrorMessage, "Pause", e);
				Log.Debug (e.StackTrace);
			}
		}

		public void Next ()
		{
			try {
				Controller.Next (false);
			} catch (Exception e) {
				Log.Error (ErrorMessage, "Next", e.Message);
				Log.Debug (e.StackTrace);
			}
		}
		
		public void Previous ()
		{
			try {
				Controller.Previous (false);
			} catch (Exception e) {
				Log.Error (ErrorMessage, "Previous", e.Message);
				Log.Debug (e.StackTrace);
			}
		}
		
		static void BuildObjectPathsDict ()
		{
			object_paths = new Dictionary<Type, string> ();
			object_paths.Add (typeof (IBansheePlayer), "/org/bansheeproject/Banshee/PlayerEngine");
			object_paths.Add (typeof (IBansheeController), "/org/bansheeproject/Banshee/PlaybackController");
		}
	}
}

// MyClass.cs created with MonoDevelop
// User: zgold at 21:14 06/11/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//


using Do.Addins.DoMusic;
using System;
using System.Collections.Generic;
using System.Diagnostics;

using NDesk.DBus;

namespace DoMusicBanshee
{
	[Interface("org.bansheeproject.Banshee.PlaybackController")]
	public interface IBansheeControl
	{
		void Previous ();
		void Next ();
	}
	
	[Interface("org.bansheeproject.Banshee.PlayerEngine")]
	public interface IBansheeEngine
	{
		void Play ();
		void Pause ();
		void TogglePlaying ();
	}
	
	public class Banshee : IMusicSource
	{
		private IBansheeControl player = null;
		private IBansheeEngine engine = null;
		
		private bool available = false;
		private string busname = "org.bansheeproject.Banshee";
		
		public override string SourceName { get { return "Banshee"; } }
		public override string Icon { get { return "media-player-banshee"; } }			
				
		/// <summary>
		/// Public Bureaucratic (non-music) Functions
		/// IsAvailable() : Check to see that the music source is still alive.
		/// This is called before every action to make sure things are still working.
		/// </summary>
		
		public override bool IsAvailable()
		{
			if (!Bus.Session.NameHasOwner (busname)) return false;
			
			if (engine == null) {
				engine = Bus.Session.GetObject<IBansheeEngine> 
					(busname, new ObjectPath ("/org/bansheeproject/Banshee/PlayerEngine"));
			}
			
			if (player == null) {
				player = Bus.Session.GetObject<IBansheeControl> 
					(busname, new ObjectPath ("/org/bansheeproject/Banshee/PlaybackController"));
			}
			
			return (player != null && engine != null);
		}
		
		/// <summary>
		/// Any necessary setup for your music source goes here
		/// </summary>
		public override void Initialize()
		{
		}
				
		/// <summary>
		/// Public Music Relatd Functions
		/// </summary>			
		
		public override void LoadMusicList(out List<Artist> artists, out List<Album> albums, out List<Song> songs){			
			albums = new List<Album>();
			artists  = new List<Artist>();
			songs = new List<Song>();												
		}			
		public override void EnqueueArtist(Artist artist){
			
		}
		
		public override void EnqueueAlbum(Album album){
			
		}		
		
		public override void EnqueueSong (Song song)
		{
		}
		
		public override void Next ()
		{
			if (IsAvailable ()) 
				player.Next ();
		}
		
	    public override void Prev ()
		{
			if (IsAvailable ()) 
				player.Previous ();
		}
		
		public override void PauseResume ()
		{
			if (IsAvailable ()) 
				engine.TogglePlaying ();
		}
		
		public override void Play ()
		{
			if (IsAvailable ()) 
				engine.Play ();
		}
		
		public override void PlayArtist (Artist artist)
		{
			
		}
		
		public override void PlayAlbum (Album album)
		{
			
		}
		
		public override void PlaySong (Song song)
		{
			
		}
	}
}

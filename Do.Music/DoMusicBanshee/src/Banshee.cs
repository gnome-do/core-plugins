// MyClass.cs created with MonoDevelop
// User: zgold at 21:14 06/11/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//


using Do.Addins.DoMusic;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DoMusicBanshee
{
	
	
	public class Banshee : IMusicSource
	{
		
		public override string SourceName { get { return "Banshee"; } }
		public override string Icon { get { return "media-player-banshee"; } }			
				
		/// <summary>
		/// Public Bureaucratic (non-music) Functions
		/// IsAvailable() : Check to see that the music source is still alive.
		/// This is called before every action to make sure things are still working.
		/// </summary>
		
		public override bool IsAvailable(){
			return true;
		}
		
		/// <summary>
		/// Any necessary setup for your music source goes here
		/// </summary>
		public override void Initialize(){
			
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
		
		public override void EnqueueSong(Song song){
						     
		}
		
		public override void Next (){
			execCommand ("PlaybackController", "Next boolean:true", true);
		}
		
	    public override void Prev (){
			execCommand ("PlaybackController", "Previous boolean:true", true);
		}
		
		public override void PauseResume (){
			execCommand ("PlayerEngine", "Pause", true);
		}
		
		public override void Play (){
			execCommand ("PlayerEngine", "Play", true);
		}
		
		public override void PlayArtist (Artist artist){
			
		}
		
		public override void PlayAlbum (Album album){
			
		}
		
		public override void PlaySong (Song song){
			
		}

		/// <summary>
		/// Private Functions
		/// </summary>
		//dbus-send --type=method_call --dest=org.bansheeproject.Banshee /org/bansheeproject/Banshee/PlayerEngine org.bansheeproject.Banshee.PlayerEngine.Pause
		private void execCommand (string component, string name, bool isMethod)
		{
			string action = "";
			if(isMethod){
				action += "--type=method_call ";
			}
			action += "--dest=org.bansheeproject.Banshee ";
			action += "/org/bansheeproject/Banshee/";
			action += component;
			action += " org.bansheeproject.Banshee." + component + "." + name;
			exec("dbus-send",action);
		}
		
		private string exec (string command, string args)
		{
			Process p = new Process();
			p.StartInfo.FileName = command; 
			p.StartInfo.Arguments = args;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.UseShellExecute = false;
			p.Start();				
			string buffer = "";
			
			while (!p.HasExited) {
				buffer += p.StandardOutput.ReadToEnd ();
			}
			string err = p.StandardError.ReadToEnd (); 
			if( err.Length > 1)
				throw new ApplicationException ("Banshee problem!");
			return buffer;
		}
	}
}

// MPD.cs created with MonoDevelop
// User: zgold at 20:31 06/04/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using Do.Addins;
using Do.Universe;
using Do.Addins.DoMusic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MPD
{	
	public class MPD : IMusicSource
	{
		public override string SourceName { get { return "MPD"; } }
		public override string Icon { get { return "mpd.png@" + GetType ().Assembly.FullName; } }
					
		private string CoverArtDirectory = Environment.GetFolderPath (Environment.SpecialFolder.Personal) + "/.covers";
		private bool available = false;
		static MPD() {
			Console.WriteLine("MPD Dll Loaded");
		}
		/// <summary>
		/// Public Bureaucratic (non-music) Functions 
		/// </summary>
					
		public override bool IsAvailable(){
			if(!available){
				Process p = new Process();
				p.StartInfo.FileName = "mpc";
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.RedirectStandardError = true;
				p.StartInfo.UseShellExecute = false;
				p.Start();				
				while(!p.HasExited){}
				string err = p.StandardError.ReadToEnd();
				if (err.Length > 0)
					return false;
				available = true;
				return true;
			}
			return available;
		}
		
		public override void Initialize(){			
			EnableEnqueueArtist   = false;
			EnableEnqueueAlbum    = false;
			EnableEnqueueSong     = false;
		}

		/// <summary>
		/// Public Music Functions
		/// </summary>
		
		public override void LoadMusicList(out List<Artist> artists, out List<Album> albums, out List<Song> songs){
			albums = new List<Album>();
			artists  = new List<Artist>();
			songs = new List<Song>();
			try { 
				Process proc = new Process();
				proc.StartInfo.FileName = "/usr/bin/mpc"; 
				proc.StartInfo.Arguments =@"playlist --format "":%title%:%artist%:%album%:%file%""";
				proc.StartInfo.UseShellExecute=false;
				proc.StartInfo.RedirectStandardOutput = true;
				proc.Start();
				String line = proc.StandardOutput.ReadLine();
				while(line != null){
					Song song;
					string song_file, song_name, album_name, artist_name;
					string[] info = line.Split(':'); 							
					int number = Convert.ToInt32(info[0].Substring(1,info[0].IndexOf(')') -1 ));
					song_name = info[1];													
					artist_name = info[2];						
					album_name = info[3];					
					song_file = info[4];		
					
					string cover_name_artist = artist_name;
					string cover_name_album = album_name;

					//Try the album art first, then the artist art
					cover_name_artist = cover_name_artist.Replace(" ","%20");
					cover_name_album = cover_name_album.Replace(" ","%20");

					string album_cover = string.Format ("{0}-{1}.jpg", cover_name_artist, cover_name_album);
					album_cover = Path.Combine (CoverArtDirectory, album_cover);							

					string artist_cover = string.Format ("{0}.jpg", cover_name_artist);
					artist_cover = Path.Combine (CoverArtDirectory, artist_cover);
					
					bool album_cover_exists = false;
					bool artist_cover_exists = false;
					
					if(File.Exists (album_cover))
						album_cover_exists = true;

					if(File.Exists(artist_cover))
						artist_cover_exists = true;
					
					string song_cover = album_cover;
					
					if(!album_cover_exists && artist_cover_exists){
						song_cover = artist_cover;
						album_cover = artist_cover;
					}
					
					if(album_cover_exists && !artist_cover_exists)
							artist_cover = album_cover;
							
					if(!album_cover_exists && !artist_cover_exists)
						song_cover = null;
										
					Artist artist = new Artist(artist_name, artist_cover, this);
					if(!artists.Contains(artist))
						artists.Add(artist);
					
					Album album = new Album(album_name,  album_cover,  artist_name, this);
					if(!albums.Contains(album))
						albums.Add(album);
					
					song = new Song(song_name,  artist_name,  album_name,  song_cover,  song_file,  ""+number, this);
					songs.Add (song);
					line = proc.StandardOutput.ReadLine();
				}				
			} catch (Exception e) {
				Console.Error.WriteLine ("Could not read MPD database file: " + e.Message);
			}
		}		
		
		public override void EnqueueArtist(Artist artist){
			//do nothing
		}
		
		public override void EnqueueAlbum(Album album){
			//do nothing
		}
		
		public override void EnqueueSong(Song song){
			//do nothing
		}
		
		public override void Next(){
			exec("next");
		}
		
	    public override void Prev(){
			exec("prev");
		}
		
		public override void PauseResume(){
			exec("toggle");
		}
		
		public override void Play(){
			exec("play");
		}
		
		public override void PlayArtist(Artist a){
			List<Song> songs = DoMusic.LoadSongsFor(a,this);
			PlaySong(songs.ToArray()[0]);
		}
		
		public override void PlayAlbum(Album a){
			List<Song> songs = DoMusic.LoadSongsFor(a,this);
			PlaySong(songs.ToArray()[0]);
		}
		
		public override void PlaySong(Song song){		
			Console.WriteLine("OTHER: " + song.Other);
			exec(string.Format("play {0}", Convert.ToInt32(song.Other)));  
		}

		/// <summary>
		/// Private convenience functions
		/// </summary>
		
		private void exec(string command){
				Console.WriteLine("mpc " + command);
			try {
				Process.Start ("mpc", command);
			} catch {}
		}								
	}
}

// Amarok.cs created with MonoDevelop
// User: zgold at 23:21 06/07/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//


using Do.Addins.DoMusic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DoMusicAmarok
{	
	public class Amarok: IMusicSource
	{
		public override string SourceName { get { return "Amarok"; } }
		public override string Icon { get { return "amarok"; } }		
		private string home, AmazonCoverArtDirectory, LocalCoverArtDirectory;		
			
		public Amarok(){			
			home =  Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			AmazonCoverArtDirectory = "~/.kde/share/apps/amarok/albumcovers/large/".Replace("~", home);
			LocalCoverArtDirectory = "~/.kde/share/apps/amarok/albumcovers/tagcover/".Replace("~", home);
		}
		/// <summary>
		/// Public Bureaucratic (non-music) Functions
		/// </summary>
		
		public override bool IsAvailable(){
			try {
				exec("dcop","amarok");
				return true;
			} catch {
				return false;
			}
		}
		public override void Initialize(){}
				
		/// <summary>
		/// Public Music Relatd Functions
		/// </summary>			
		
		public override void LoadMusicList(out List<Artist> artists, out List<Album> albums, out List<Song> songs){			
			albums = new List<Album>();
			artists  = new List<Artist>();
			songs = new List<Song>();								
				
			string query = "select al.name, ar.name, t.title, t.url from tags t, artist ar, album al where t.album = al.id AND t.artist = ar.id";
			string results = "";
			try{
				results = runQuery(query);
			}catch{
				Console.WriteLine("Amarok Plugin: Amarok does not appear to be started!  Start amarok and the next time the plugin updates (<2 min) it will be indexed.");
			}
			string[] lines = results.Split('\n');
			for(int x = 0; x<lines.Length-1; x+=4){
				string album_name = lines[x];
				string artist_name = lines[x+1];
				string song_name = lines[x+2];
				string song_url = lines[x+3];
				string cover = calcCover(artist_name,album_name);
				Album a = new Album (album_name, cover, artist_name, this);
				Artist ar = new Artist (artist_name, cover, this);
				Song s = new Song (song_name, artist_name, album_name, cover, song_url, song_url, this);
				if(!albums.Contains(a))
					albums.Add(a);
				if(!artists.Contains(ar))
					artists.Add(ar);
				songs.Add(s);
			}
		}			
		public override void EnqueueArtist(Artist artist){
			foreach (Song song in DoMusic.LoadSongsFor(artist,this))
				EnqueueSong(song);
		}
		
		public override void EnqueueAlbum(Album album){
			foreach(Song song in DoMusic.LoadSongsFor(album,this))
				EnqueueSong(song);
		}		
		
		public override void EnqueueSong(Song song){
			string url = cleanURL(song.Other);
			exec("dcop","amarok playlist addMedia \""+ url +"\"");			     
		}
		
		public override void Next(){
			exec("dcop","amarok player next");
		}
		
	    public override void Prev(){
			exec("dcop","amarok player prev");
		}
		
		public override void PauseResume(){
			exec("dcop","amarok player playPause");
		}
		
		public override void Play(){
			exec("dcop","amarok player play");
		}
		
		public override void PlayArtist(Artist artist){
			Song first = null;
			foreach(Song song in DoMusic.LoadSongsFor(artist,this)){
				if(first == null) first = song;
				EnqueueSong(song);
			}
			PlaySong(first);		
		}
		
		public override void PlayAlbum(Album album){
			Song first = null;
			foreach(Song song in DoMusic.LoadSongsFor(album,this)){
				if(first == null) first = song;
				EnqueueSong(song);
			}
			PlaySong(first);		
		}
		
		public override void PlaySong(Song song){
			string url = cleanURL(song.Other);
			exec("dcop","amarok playlist playMedia \""+ url +"\"");			
		}

		/// <summary>
		/// Private convenience functions
		/// </summary>
		private string cleanURL(string url){
			if(url.StartsWith("."))
				url = url.Substring(1);
			return url;
		}
		private string exec(string command, string args){
			Process p = new Process();
			p.StartInfo.FileName = command; 
			p.StartInfo.Arguments = args;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.UseShellExecute = false;
			p.Start();				
			string buffer = "";
			while(!p.HasExited){
				buffer += p.StandardOutput.ReadToEnd ();
			}
			string err = p.StandardError.ReadToEnd (); 
			if(err.Length > 1)
				throw new ApplicationException ("Amarok not loaded");
			return buffer;
		}

		private string runQuery(String sql){
			string cmd = "dcop";
			string args = "amarok collection query \"" + sql + "\"";
			return exec(cmd, args);
		}
		
		private string calcCover(string artist_name, string album_name){
			string cover = (CalculateMD5Hash ((artist_name + album_name).ToLower ())).ToLower ();
			if(File.Exists(AmazonCoverArtDirectory+cover))
				return AmazonCoverArtDirectory + cover;
			else
				if(File.Exists(LocalCoverArtDirectory + cover))
					return LocalCoverArtDirectory + cover;
				else
					return null;				
		}
		
		private static string CalculateMD5Hash (string input)
		{
			// step 1, calculate MD5 hash from input
		    MD5 md5 = System.Security.Cryptography.MD5.Create ();
		    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes (input);
		    byte[] hash = md5.ComputeHash (inputBytes);
		 
		    // step 2, convert byte array to hex string
		    StringBuilder sb = new StringBuilder();
		    for (int i = 0; i < hash.Length; i++)
		    {
		        sb.Append (hash[i].ToString ("X2"));
		    }
		    return sb.ToString();
		}
	}
}

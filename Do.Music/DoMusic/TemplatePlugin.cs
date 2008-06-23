// TemplatePlugin.cs created with MonoDevelop
// User: zgold at 20:09 06/04/2008
//
//  GNOME Do is the legal property of its developers, whose names are too numerous
//  to list here.  Please refer to the COPYRIGHT file distributed with this
//  source distribution.
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
//
// This file is a guide to creating your own Do Music plugins.  
// In MonoDevelop you need three references:
// DoMusic.dll
// Do.Addins
// System
// Your plugin needs to be a subclass of IMusicSource... just fill in the funcitons below
// and add private helpers as necessary :)


using Do.Addins.DoMusic;
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace YOURNAMESPACE
{
	
	
	public class YOURCLASSNAME : IMusicSource
	{

		public override string SourceName { get { return "NAMEHERE"; } }
		public override string Icon { get { return "ICONHERE"; } }		
		
			
				
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
		
		public override void Next(){
			
		}
		
	    public override void Prev(){
			
		}
		
		public override void PauseResume(){
			
		}
		
		public override void Play(){
			
		}
		
		public override void PlayArtist(Artist artist){
			
		}
		
		public override void PlayAlbum(Album album){
			
		}
		
		public override void PlaySong(Song song){
			
		}
		
		/// <summary>
		/// Private functions down here
		/// 
		/// exec is provided because I think its likely most plugin writers will use it.  
		/// </summary>
		
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
				throw new ApplicationException ("An Error was thrown!");
			return buffer;
		}
	}
}

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

/*
 * To enable debug outputm add "VERBOSE_OUTPUT" to "define symbols".
 *
 */  

using System;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Text;


using Do.Universe;
using Do.Platform;

namespace SqueezeCenter
{

	public class Server
	{
	
		static Server instance;
		internal static Dictionary<string, Settings.Setting> settings;
		
		public static string ConfigFile {
			get {
				return System.IO.Path.Combine (System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
				                               "SqueezeCenter.config");
 			}
		} 

		public static Server Instance
		{
			get {
				if (instance == null)					
					InitInstance ();
				return instance;				
			}
		}
		
		public static void DisposeInstance ()
		{
			Server currInstance = Interlocked.Exchange (ref instance, null);			
			if (currInstance != null)			
				currInstance.Quit ();
		}
		
		static void InitInstance ()
		{
			// Initialize settings
			settings = new Dictionary<string,Settings.Setting> ();
			
			settings.Add ("Host", new Settings.Setting ("Host", "Host-name of SqueezeCenter server", "localhost"));
			settings.Add ("CLIPort", new Settings.Setting ("CLIPort", "Port of the SqueezeCenter server cli interface", 9090));
			settings.Add ("WebPort", new Settings.Setting ("WebPort", "Port of the SqueezeCenter server web interface", 9000));
			
			settings.Add ("LoadInBackground",  
				new Settings.Setting ("LoadInBackground",
			                                    "Load artist, albums and radio in the background when loading DO. " +
			                                    "If set to false, these items are loaded when DO is loading causing " + 
			                                    "a delay until all items are loaded.", true));
			
			settings.Add ("Radios",  new Settings.Setting ("Radios", "Comma-seperated list of radios to load", "alien"));
			
			Settings.ReadSettings (ConfigFile, settings.Values, true);
			
			List<string> radioLst = new List<string> ();
			string radios = settings["Radios"].Value;
			foreach (string s in radios.Split (new char[] {','})) 
			{
				if (s.Trim ().Length > 0) 
				{
					radioLst.Add (s.Trim ());
				}
			}
				
			instance = new Server (
			                       settings["Host"].Value, 
			                       settings["CLIPort"].ValueAsInt, 
			                       settings["WebPort"].ValueAsInt, 
			                       settings["LoadInBackground"].ValueAsBool, 
			                       radioLst);
			
			instance.Initialze ();
	}
		
		bool quitThread = false;
		Thread workThread;		
		string host;
		int cliport, httpport;
		List<string> radiosToLoad = new List<string> ();
		bool loadInBackground;
		
		Queue commandQueue = Queue.Synchronized (new Queue ());
		
		bool playersLoaded, artistAndAlbumsLoaded, radiosLoaded = false;
		
		Dictionary<string, Player> players = new Dictionary<string,Player>();		
		List<RadioSuperItem> radios = new List<RadioSuperItem> ();
		List<ArtistMusicItem> artists = new List<ArtistMusicItem> ();
		List<AlbumMusicItem> albums = new List<AlbumMusicItem> ();
		
		Dictionary<string, ArtistMusicItem> artistnameToArtistMap = new Dictionary<string, ArtistMusicItem> ();
	
		private Server(string host, int cliport, int httpport, bool loadInBackground, IEnumerable <string> radiosToLoad)
		{			
			this.host = host;
			this.cliport = cliport;
			this.httpport = httpport;
			this.loadInBackground = loadInBackground;
			foreach (string s in radiosToLoad) 
			{
				this.radiosToLoad.Add (s.Trim ().ToLower ());
			}
		}
		
		private void Initialze ()
		{
#if VERBOSE_OUTPUT
			Console.WriteLine ("SqueezeCenter: Host: {0} cliPort: {1} httpPort: {2}  loadInBackground: {3}",
			                   this.host, this.cliport, this.httpport, loadInBackground);
#endif
						
			this.workThread = new Thread (new ThreadStart (Execute));			
			this.workThread.Start ();
			
			// wait for max 15 seconds to allow the players and items to be loaded			
			DateTime continueAt = DateTime.Now.AddSeconds (15);
			// wait while players aren't loaded and, if background loading is disabled, artist, albums and radios aren't loaded			
			bool itemsLoaded = false;			
			while ( DateTime.Now < continueAt && !itemsLoaded)			       
			{
				Thread.Sleep(50);
				itemsLoaded = this.playersLoaded && (this.loadInBackground || (this.artistAndAlbumsLoaded && IsRadioLoaded ())); 
			}
						
			if (!loadInBackground && !this.artistAndAlbumsLoaded) 
			{
				Console.WriteLine ("SqueezeCenter: Time-out (15 sec.) reading artists and albums. Will read in the background."); 
			}
			
			if (!loadInBackground && !IsRadioLoaded ()) 
			{
				Console.WriteLine ("SqueezeCenter: Time-out (15 sec.) reading radios. Will read in the background."); 
			}
		}
		
		private void Quit ()
		{
			this.quitThread = true;
			// wait 5 seconds for thread to end 
			if (!this.workThread.Join (5000))
			{
				Console.WriteLine ("SqueezeCenter: Failed to stop background thread in 5 second. Aborting..."); 
				this.workThread.Abort ();
			}
		}
	
		private void Execute ()
		{		
			
			AppDomain.CurrentDomain.ProcessExit += delegate(object sender,EventArgs e)
			{
				Quit ();
			};
			
			try 
			{
				TcpClient tcpClient;
				NetworkStream stream;
				StreamWriter writer;
				NetworkStreamTextReader reader;
				string response;
							
				while (!quitThread)
				{
					try 
					{
						tcpClient = new TcpClient (this.host, this.cliport);
						tcpClient.NoDelay = false;
						stream = tcpClient.GetStream ();
						writer = new StreamWriter (stream);
						reader = new NetworkStreamTextReader (stream);
					}
					catch (Exception)
					{
						Console.WriteLine ("SqueezeCenter: Error connecting to server {0}:{1}. Retrying in 30 seconds." + 
						                   "You may want to adjust the settings in the SqueezeCenter configuration dialog.", 
						                   this.host, this.cliport);
						DateTime continueAt = DateTime.Now.AddSeconds (30);
						while (!this.quitThread && DateTime.Now < continueAt)
							Thread.Sleep (1000);
						continue;
					}
					
					try
					{
						// subscribe to needed events
						commandQueue.Enqueue ("subscribe rescan");
						commandQueue.Enqueue ("players 0 100");			
												
						while (!quitThread)
						{						
							while ( commandQueue.Count > 0) 
							{
#if VERBOSE_OUTPUT
								Console.WriteLine ("SQC: Sending: " + commandQueue.Peek ());								
#endif
								writer.WriteLine (commandQueue.Dequeue ());
								writer.Flush ();
							}				
							
							if ((response = reader.ReadLine ()) != null) 
							{
								// data received
								ParseResponse (response);
								response = null;
							}
							else 
							{
								Thread.Sleep(100);
							}
						}
						
						// quit
						try
						{
#if VERBOSE_OUTPUT
							Console.WriteLine ("SQC: Sending exit to server.");
#endif
							writer.WriteLine ("exit");
							writer.Flush ();
							tcpClient.Close ();
						}
						catch {}
					}
					catch (ThreadAbortException) { throw; }
					catch (Exception ex)
					{						
						ClearData ();
						if (ex is IOException || ex is SocketException)
						{						
							Console.WriteLine ("SqueezeCenter: Connection lost. Trying to reconnect in 5 seconds...");
							Thread.Sleep (5000);
						}
						else
						{
							Console.WriteLine ("SqueezeCenter unhandled {0}: {1}", ex.GetType ().ToString (),  ex.Message);
							return;
						}
					}
				}
			}
			catch (ThreadAbortException)
			{  
			}
		}
		
		private void ClearData ()
		{
			lock (this.players)
				this.players.Clear ();
			
			lock (this.artists)
				this.artists.Clear ();
			
			lock (this.albums)
				this.albums.Clear ();
			
			lock (this.radios)
				this.radios.Clear ();			
		}
		
		private void ParseResponse (string response)
		{			
			string head = response.Substring(0, Math.Max(0, response.IndexOf(' ')));					
			
			if (string.Equals (head, "players")) 
			{
				
				// parse players
				lock (players) {
					
					foreach (QueryResponseItem itm in ParseQueryResponse 
			         ("playerindex", response, new string[] {"playerid", "name", "model", "connected", "canpoweroff"})) 
					{							
														
						Player player;
						if (players.TryGetValue(itm.Values[0], out player)) 
						{
							player.Connected = itm.Values[3] == "1";
							player.CanPowerOff = itm.Values[4] == "1";
						}
						else 
						{
							players.Add (itm.Values[0], new Player (itm.Values[0], itm.Values[1], itm.Values[2], 
							                                        itm.Values[3] == "1", false, itm.Values[4] == "1"));
						}
					}
				
					// get status for all players
					foreach (Player p in players.Values) 
					{
						commandQueue.Enqueue(p.Id + " status - 0 subscribe:0");
					}
				}
				
				this.playersLoaded = true;
				
				// now load radios, artists and albums
				commandQueue.Enqueue ("radios - 100000");
				commandQueue.Enqueue ("artists 0 1000000");
				commandQueue.Enqueue ("albums 0 1000000 tags:lyja");
			}
			
			else if (string.Equals (head, "artists")) 
			{
				
				artistnameToArtistMap.Clear ();
				
				lock (this.artists) 
				{
					artists.Clear ();
					ArtistMusicItem artist;
					foreach (QueryResponseItem itm in ParseQueryResponse ("id", response, new string[] {"id", "artist"})) 
					{
						artist = new ArtistMusicItem (int.Parse (itm.Values[0]), itm.Values[1]); 
						artists.Add (artist);
						artistnameToArtistMap.Add (artist.Artist, artist);
					}
				}
				
#if VERBOSE_OUTPUT
					Console.WriteLine ("SQC: Artists loaded");
#endif
			}
			
			else if (string.Equals (head, "albums")) 
			{								
				
				lock (this.albums) 
				{
					albums.Clear ();
					
					ArtistMusicItem artist;
					int firstSongId;
					foreach (QueryResponseItem itm in ParseQueryResponse ("id", response, 
					                                                      new string[] 
					                                                      {"id", "album", "year", "artwork_track_id", "artist"})) 
					{
						if (!artistnameToArtistMap.TryGetValue (itm.Values[4], out artist)) 
						{
							artist = null;
						}
						if (!int.TryParse (itm.Values[3], out firstSongId))
						{
							firstSongId = -1;
						}
						albums.Add (new AlbumMusicItem (int.Parse (itm.Values[0]), itm.Values[1], artist, itm.Values[2], firstSongId));
					}
				}
				this.artistAndAlbumsLoaded = true;
				
#if VERBOSE_OUTPUT
				Console.WriteLine ("SQC: Albums loaded");
#endif
			}
			
			else if (string.Equals (head, "radios")) 
			{				
				lock (this.radios) 
				{
					radios.Clear ();
												
					foreach (QueryResponseItem itm in ParseQueryResponse ("cmd", response, 
					                                                      new string[] 
					                                                      {"cmd", "name", "type"})) 
					{						
						// only xmlbrowser types
						if (itm.Values[2] == "xmlbrowser" && this.radiosToLoad.Contains (itm.Values[1].ToLower ())) 
						{									
							RadioSuperItem radio = new RadioSuperItem (itm.Values[0], itm.Values[1]);
							radios.Add (radio);
							// request radio items
							commandQueue.Enqueue (radio.Command + " items 0 100000");
						}
					}
				}
				this.radiosLoaded = true;
			}
			
			else if (string.Equals (head, "rescan") && string.Equals (response, "rescan done")) 
			{
				commandQueue.Enqueue ("artists 0 1000000");
				commandQueue.Enqueue ("albums 0 1000000 tags:lyja");
			}
			
			else 
			{

				// check if it's a player
				Player player;
				lock (this.players) 
				{
					if (!players.TryGetValue (Util.UriDecode (head), out player)) 
					{
						player = null;
					}
				}
										
				if (player != null && response.Length > head.Length + 1) 
				{
					lock (this.players) 
					{						
						response = response.Substring (head.Length +1);
						int i = response.IndexOf (' ');
						if (i<0)
							i = response.Length;
						string command = response.Substring (0, i);
						
#if VERBOSE_OUTPUT
						Console.WriteLine ("SQC: Player command response: " + command);
#endif
						
						if (string.Equals (command, "status")) 
						{
							foreach (QueryResponseItem itm in 
							         ParseQueryResponse ("player_name", response, 
							                             new string[] {								
								"player_connected",	"power", "sync_master", "sync_slaves" })) 
							{
								player.Connected = itm.Values[0] == "1";
								player.PoweredOn = itm.Values[1] == "1";

								// get players that are synced with this player
								List<Player> syncedPlayers = new List<Player> ();
								
								if (itm.Values[2] != null && itm.Values[3] != null) 
								{											
									string playersInSyncGroup = string.Format ("{0},{1}", itm.Values[2], itm.Values[3]);
									foreach (string s in playersInSyncGroup.Split (new char[] {','}, StringSplitOptions.RemoveEmptyEntries)) 
									{
										Player p;
										if (this.players.TryGetValue (s.Trim (), out p) && p != player) 
										{
										    syncedPlayers.Add (p);
										}
									}
								}
								// due to bug http://bugs.slimdevices.com/show_bug.cgi?id=7990
								// we need to get the status of all players that are now unsynced
								// BEGIN workaround
								foreach (Player p in player.SyncedPlayers) 
								{
									if (!syncedPlayers.Contains (p)) 
									{
										commandQueue.Enqueue(p.Id + " status - 0");												
									}
								}
								// END workaround
								player.SetSynchedPlayers (syncedPlayers);
							}
						}
						
						else 
						{
							// check if it's a radio
							RadioSuperItem radio = null;
							
							lock (this.radios) 
							{
								foreach (RadioSuperItem r in this.radios) {
									if (response.StartsWith (r.Command + " items ")) {
										radio = r;
										break;
									}
								}								
							}
												
							if (radio != null) {
								// get position of content start
								int pos = response.IndexOf ("item_id%3A");
								if (pos < 0)
									pos = response.IndexOf ("title%3A");

								if (pos > 0)
								{
									string ids;
									int id;
									List <RadioSubItem> children;
									RadioSubItem child;
										
									// find parent
									RadioItem parent = radio;

									// is there a item_id filter?
									if (response.Substring (pos).StartsWith ("item_id%3A")) {										
										ids = response.Substring (pos+10, response.IndexOf (" ", pos+10) - pos - 10);
										
										foreach (string subId in ids.Split (new char[] {'.'})) {
											foreach (RadioSubItem rmi in parent.Children) {
												if (rmi.Id.ToString() == subId) {
													parent = rmi;
													break;
												}
											}
										}										
									}
									//Console.WriteLine (response);
									children = new List<RadioSubItem> ();
									
									foreach (QueryResponseItem itm in 
									         ParseQueryResponse ("id", response, new string[] {
										"id", "hasitems", "name"})) {
										
										ids = itm.Values[0];
										if (parent is RadioSubItem && ids == (parent as RadioSubItem).IdPath) {
											continue;
										}
										    
										if (ids.Contains ("."))
											ids = ids.Substring (ids.LastIndexOf (".")+1);
										
										if (int.TryParse (ids, out id) && itm.Values[2] != null) {
											child = new RadioSubItem (parent, id, itm.Values[2], itm.Values[1] != null && itm.Values[1] != "0");
											children.Add (child);
											
											// request children
											if (child.HasItems) {
												commandQueue.Enqueue (string.Format ("{0} items 0 10000 item_id:{1}",
												                                     child.GetSuper ().Command,
												                                     child.IdPath ));
											}
										}
									}									
									parent.Children = children.ToArray ();
								}								
							}
						}
					}					
				}
			}
		}
		
		public ArtistMusicItem[] GetArtists ()
		{
			lock (this.artists) {
				return this.artists.ToArray ();
			}
		}
		
		public AlbumMusicItem[] GetAlbums ()
		{
			lock (this.albums) {
				return this.albums.ToArray ();
			}
		}
		
		public RadioSuperItem[] GetRadios ()
		{
			lock (this.radios) {
				return this.radios.ToArray ();
			}
		}
		
		public Player[] GetPlayers () 
		{			
			lock (players) {
				Player[] result = new Player[players.Count];
				players.Values.CopyTo (result, 0);
				return result;
			}
		}
		
		public List<Player> GetConnectedPlayers ()
		{			
			lock (players) {
				List<Player> result = new List<Player> (players.Count);
				foreach (Player p in players.Values)
					if (p.Connected)
						result.Add (p);
				return result;				
			}
		}
		
		public List<Do.Universe.Item> GetConnectedPlayersAsItem ()
		{			
			lock (players) {
				List<Do.Universe.Item> result = new List<Do.Universe.Item> (players.Count);
				foreach (Player p in players.Values)
					if (p.Connected)
						result.Add (p);
				return result;				
			}
		}

		public void ExecuteCommand(string command)
		{
			this.commandQueue.Enqueue (command);
		}
		
		public void AddItemsToPlayer(Player player, IEnumerable<MusicItem> items)
		{
			PlaylistControl (player, items, "add");
		}
		
		public void LoadItemsToPlayer(Player player, IEnumerable<MusicItem> items)
		{
			PlaylistControl (player, items, "load");			
		}
		
		public string GetCoverUrl (int songId)
		{
			return string.Format ("http://{0}:{1}/music/{2}/cover.jpg", this.host, this.httpport, songId);
		}

		private bool IsRadioLoaded ()
		{
			if (!this.radiosLoaded)
				return false;
			
			lock (this.radios) {
				foreach (RadioSuperItem r in this.radios) {
					if (!r.IsLoadedRecursive) {						
						return false;
					}
				}
			} 
			return true;
		}
		
		private void PlaylistControl(Player player, IEnumerable<MusicItem> items, string command)
		{	
			foreach (MusicItem item in items) {
				string cmd = string.Format ("{0} playlistcontrol cmd:{1} {2}:{3}", 
				                           player.Id, command, item.SqueezeCenterIdKey, item.Id);
				commandQueue.Enqueue (cmd);
			}
		}

		private List<QueryResponseItem> ParseQueryResponse (string seperatorTag, string resp, string[] fields)
		{
			List<QueryResponseItem> result = new List<QueryResponseItem>();

			int pos = resp.IndexOf(seperatorTag + "%3A");

			try {
				QueryResponseItem respItm = null;
				while(pos >= 0 && pos < resp.Length)
				{							
					int pos2 = resp.IndexOf("%3A", pos);
					if(pos2 < 0)
						break;
					
					string tagName = resp.Substring(pos, pos2-pos);				
					pos = pos2 + 3;
					pos2 = resp.IndexOf(" ", pos);
					if(pos2 < 0) pos2 = resp.Length;
					
					string fieldValue = resp.Substring(pos, pos2-pos);					
					if (tagName.Equals (seperatorTag))
					{
						respItm = new QueryResponseItem(fields.Length);
						result.Add(respItm);
					}
					
					int fieldIdx = Array.IndexOf(fields, tagName);
					if(fieldIdx >= 0)						
					{						
						//respItm.Values[fieldIdx] = System.Web.HttpUtility.UrlDecode (fieldValue);
						respItm.Values[fieldIdx] = Util.UriDecode (fieldValue);
					}
					
					pos = pos2+1; 
				}
			} catch(Exception ex) 
			{
				Console.WriteLine("SqueezeCenter: Error parsing response:\n" + ex.ToString ()); 
			}
			return result;
		}							
		
		private class QueryResponseItem
		{
			public QueryResponseItem(int valuesCount)
			{				
				this.Values = new string[valuesCount];
			}
						
			public string[] Values;
		}
		
	}
}

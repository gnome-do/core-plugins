// Youtube.cs created with MonoDevelop
// User: luis at 07:10 p 06/09/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.IO;
using System.Net;
using System.Threading;
using Mono.Unix;
using System.Collections.Generic;

using Do.Universe;

using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.YouTube;
using Google.GData.Extensions.MediaRss;

namespace YouTube
{
	
	
	public static class Youtube
	{

		static readonly string ConnectionErrorMessage = Catalog.GetString ("An error occurred connecting to YouTube, "
			+ "are your credentials valid?");
			
		static readonly string MissingCredentialsMessage = Catalog.GetString ("Missing login credentials. Please set "
			+ "login information in YouTube plugin configuration.");
		
		public const string appName = "luismmontielg-gnomeDoYoutubePlugin0.1";
		public static string clientID = "ytapi-lmg-test-ojd8d285-0";
		public static string developerKey = "AI39si5c61jYzQLvzEDjAnU1HOQIf-DzyzvIBXAkGJ82NlXoMg10RDW1sRz5Uyodv9_ETPzmJXdfFqVRNt51yGkkNo2YW0BdxQ";
		public static List<Item> favorites;
		public static List<Item> subscriptions;
		public static List<Item> own;
		private static object fav_lock;
		private static object sub_lock;
		private static object own_lock;
		private static YouTubeService service;
		private static string username;
		private static string password;
		private static int subUpdate;
		private static int favUpdate;
		private static int ownUpdate;
		
		public static YouTubePreferences Preferences { get; private set; }
		
		static Youtube()
		{
			Youtube.favorites = new List<Item>();
			Youtube.own = new List<Item>();
			Youtube.subscriptions = new List<Item>();
			Preferences = new YouTubePreferences ();
			subUpdate = 0;
			favUpdate = 0;
			ownUpdate = 0;
			username = Preferences.Username;

			password = Preferences.Password;

			fav_lock = new object ();
			sub_lock = new object ();
			own_lock = new object ();

			//Connect (Preferences.Username, Preferences.Password);
			Connect (username, password);
		}
		
		public static void updateFavorites()
		{
			favUpdate++;
			Console.WriteLine("Favorites update tries= "+favUpdate);
			if (favUpdate < 2 || favUpdate%20 == 0){
				Console.WriteLine("iniciando updateFavorites(), count "+favUpdate);
				Youtube.favorites.Clear();
				if (!Monitor.TryEnter (fav_lock)) return;
				int i =0;
				int maxResults = 50;
				int startIndex = 1;
				
				string feedUrl = "http://gdata.youtube.com/feeds/api/users/"+ username +"/favorites?start-index="+ startIndex +"&max-results="+maxResults;
				YouTubeQuery query = new YouTubeQuery(feedUrl);
				Console.WriteLine("feedUrl for favorites: "+feedUrl);
				
				try{
					YouTubeFeed videoFeed = service.Query(query);
					
					while(videoFeed.Entries.Count > 0){				
						foreach (YouTubeEntry entry in videoFeed.Entries) 
						{
							Console.WriteLine(++i);
							Console.WriteLine("Title: " + entry.Title.Text);
							string url = "http://www.youtube.com/watch?v="+entry.Id.AbsoluteUri.Substring(entry.Id.AbsoluteUri.Length - 11);
							YoutubeVideoItem video = new YoutubeVideoItem(entry.Title.Text, url, entry.Media.Description.Value);
							favorites.Add(video);
						}
						startIndex += maxResults;
						feedUrl = "http://gdata.youtube.com/feeds/api/users/"+ username +"/favorites?start-index="+ startIndex +"&max-results="+maxResults;
						query = new YouTubeQuery(feedUrl);
						videoFeed = service.Query(query);
					}
					startIndex = 1;
				}catch(Exception e) {
					Console.Error.WriteLine ("Youtube Error: {0}",e.Message);				
				}finally{
					Monitor.Exit(fav_lock);
				}
			}
		}
		
		public static void updateOwn()
		{
			ownUpdate++;
			Console.WriteLine("Own videos update tries= "+ownUpdate);
			if (ownUpdate < 2 || ownUpdate%20 == 0){
				Console.WriteLine("iniciando updateOwn(), count "+ownUpdate);
				Youtube.own.Clear();
				if (!Monitor.TryEnter (own_lock)) return;
				int i =0;
				int maxResults = 50;
				int startIndex = 1;
				
				string feedUrl = "http://gdata.youtube.com/feeds/api/users/"+ username +"/uploads?start-index="+ startIndex +"&max-results="+maxResults;
				YouTubeQuery query = new YouTubeQuery(feedUrl);
				Console.WriteLine("feedUrl for own Videos: "+feedUrl);
				
				try{
					YouTubeFeed videoFeed = service.Query(query);
					
					while(videoFeed.Entries.Count > 0){				
						foreach (YouTubeEntry entry in videoFeed.Entries) 
						{
							Console.WriteLine(++i);
							Console.WriteLine("Title of own video: " + entry.Title.Text);
							string url = "http://www.youtube.com/watch?v="+entry.Id.AbsoluteUri.Substring(entry.Id.AbsoluteUri.Length - 11);
							YoutubeVideoItem video = new YoutubeVideoItem(entry.Title.Text, url, entry.Media.Description.Value);
							own.Add(video);
						}
						startIndex += maxResults;
						feedUrl = "http://gdata.youtube.com/feeds/api/users/"+ username +"/uploads?start-index="+ startIndex +"&max-results="+maxResults;
						query = new YouTubeQuery(feedUrl);
						videoFeed = service.Query(query);
					}
				}catch(Exception e) {
					Console.Error.WriteLine ("Youtube Error: {0}",e.Message);				
				}finally{
					Monitor.Exit(own_lock);
				}
			}
		}		
		
		public static void updateSubscriptions()
		{
			subUpdate++;
			Console.WriteLine("Subscriptions update tries= "+subUpdate);
			if (subUpdate < 2 || subUpdate%20==0){
				Console.WriteLine("init updateSubscriptions(), count"+subUpdate);				
				Youtube.subscriptions.Clear();
				if (!Monitor.TryEnter (sub_lock)) return;
				int i =0;
				
				string feedUrl = "http://gdata.youtube.com/feeds/api/users/"+ username +"/subscriptions";
				YouTubeQuery query = new YouTubeQuery(feedUrl);
				Console.WriteLine("feedUrl for subscriptions: "+feedUrl);
				
				try
				{
					SubscriptionFeed subFeed = service.GetSubscriptions(query);
					
					if(subFeed.Entries.Count > 0){
						foreach (SubscriptionEntry entry in subFeed.Entries)
						{
							Console.WriteLine(++i);
							Console.WriteLine(entry.Title.Text);
							string url = "http://www.youtube.com/user/"+entry.Title.Text.Substring(entry.Title.Text.IndexOf(":")+1).Trim();
							YouTubeSubscriptionItem subscription = new YouTubeSubscriptionItem(entry.Title.Text.Substring(entry.Title.Text.IndexOf(":")+1).Trim(), url, entry.Title.Text);
							Youtube.subscriptions.Add(subscription);
						}
					}
				}
				catch(Exception e) 
				{
					Console.Error.WriteLine ("Youtube subscription Error: {0}",e.Message);				
				}
				finally
				{
					Monitor.Exit(sub_lock);
				}
			}
		}		
		
		public static bool TryConnect (string username, string password)
		{
			Connect(username, password);
			return true;
		}		
		
		private static void Connect (string username, string password) 
		{
			try {
				service = new YouTubeService (appName, clientID, developerKey);
				service.setUserCredentials (username, password);
			} catch (Exception e) {
				Console.Error.WriteLine ("YouTube connection error: {0}",e.Message);
			}
		}
		
	}
}

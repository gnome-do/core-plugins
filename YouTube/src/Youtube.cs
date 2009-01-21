using System;
using System.IO;
using System.Net;
using Mono.Unix;
using System.Collections.Generic;
using Do.Universe;
using Do.Platform;
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
			Connect (username, password);
		}
		
		public static void updateFavorites()
		{
			favUpdate++;
			Log.Debug ("YouTube > Update favorites videos tries = {0}", favUpdate);
			if (favUpdate < 2 || favUpdate%20 == 0){
				Youtube.favorites.Clear();
				int i =0;
				int maxResults = 50;
				int startIndex = 1;
				
				string feedUrl = "http://gdata.youtube.com/feeds/api/users/"+ username +"/favorites?start-index="+ startIndex +"&max-results="+maxResults;
				YouTubeQuery query = new YouTubeQuery(feedUrl);
				Log.Debug ("YouTube > feedUrl for favorites videos: {0}", feedUrl);
				try{
					YouTubeFeed videoFeed = service.Query(query);
					while(videoFeed.Entries.Count > 0){				
						foreach (YouTubeEntry entry in videoFeed.Entries) 
						{
						    Log.Debug ("YouTube > Video #{0}, Title: ", ++i, entry.Title.Text);
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
					Log.Error ("YouTube > Error getting favorites videos - {0}", e.Message);
    				Log.Debug (e.StackTrace);
				}
			}
		}
		
		public static void updateOwn()
		{
			ownUpdate++;
			Log.Debug ("YouTube > Update own videos tries = {0}", ownUpdate);
			if (ownUpdate < 2 || ownUpdate%20 == 0){
				Youtube.own.Clear();
				int i =0;
				int maxResults = 50;
				int startIndex = 1;
				
				string feedUrl = "http://gdata.youtube.com/feeds/api/users/"+ username +"/uploads?start-index="+ startIndex +"&max-results="+maxResults;
				YouTubeQuery query = new YouTubeQuery(feedUrl);
				Log.Debug ("YouTube > feedUrl for own videos: {0}", feedUrl);
				try{
					YouTubeFeed videoFeed = service.Query(query);
					while(videoFeed.Entries.Count > 0){				
						foreach (YouTubeEntry entry in videoFeed.Entries) 
						{
						    Log.Debug ("YouTube > Video #{0}, Title(own video): ", ++i, entry.Title.Text);
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
					Log.Error ("YouTube > Error getting own videos - {0}", e.Message);
    				Log.Debug (e.StackTrace);
				}
			}
		}		
		
		public static void updateSubscriptions()
		{
			subUpdate++;
			Log.Debug ("YouTube > Update subscriptions tries = {0}", ownUpdate);
			if (subUpdate < 2 || subUpdate%20==0){
				Youtube.subscriptions.Clear();
				int i =0;
				
				string feedUrl = "http://gdata.youtube.com/feeds/api/users/"+ username +"/subscriptions";
				YouTubeQuery query = new YouTubeQuery(feedUrl);
				Log.Debug ("YouTube > feedUrl for subscriptions: {0}", feedUrl);				
				try
				{
					SubscriptionFeed subFeed = service.GetSubscriptions(query);
					if(subFeed.Entries.Count > 0){
						foreach (SubscriptionEntry entry in subFeed.Entries)
						{
                            Log.Debug ("YouTube > Subscriptions - {0}", ++i);
                            Log.Debug ("YouTube > {0}", entry.Title.Text);
							string url = "http://www.youtube.com/user/" + entry.UserName;
							YouTubeSubscriptionItem subscription = new YouTubeSubscriptionItem(entry.UserName, url, entry.Title.Text);
							Youtube.subscriptions.Add(subscription);
						}
					}
				}
				catch(Exception e) 
				{
                    Log.Error ("YouTube > Error getting subscriptions - {0}", e.Message);
    				Log.Debug (e.StackTrace);
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
                Log.Error ("YouTube > Error connecting to service - {0}", e.Message);
   				Log.Debug (e.StackTrace);
			}
		}
		
	}
}

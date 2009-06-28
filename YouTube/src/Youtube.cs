using System;
using System.IO;
using System.Net;
using Mono.Addins;
using System.Collections.Generic;
using Do.Universe;
using Do.Platform;
using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.YouTube;
using Google.GData.Extensions.MediaRss;

namespace Do.Universe
{
	public class Youtube
	{

		static readonly string ConnectionErrorMessage = AddinManager.CurrentLocalizer.GetString ("An error occurred connecting to YouTube, "
			+ "are your credentials valid?");
			
		static readonly string MissingCredentialsMessage = AddinManager.CurrentLocalizer.GetString ("Missing login credentials. Please set "
			+ "login information in YouTube plugin configuration.");
		
		public const string appName = "luismmontielg-gnomeDoYoutubePlugin0.1";
		public const string searchUrl = "http://www.youtube.com/results?search_query=";
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
			Log<Youtube>.Debug("Update favorites videos tries = {0} - favorite.Count : {1}", favUpdate, Youtube.favorites.Count);
			if (Youtube.favorites.Count == 0 || favUpdate%20 == 0){
				Youtube.favorites.Clear();
				int maxResults = 50;
				int startIndex = 1;
				
				string feedUrl = "http://gdata.youtube.com/feeds/api/users/"+ username +"/favorites?start-index="+ startIndex +"&max-results="+maxResults;
				YouTubeQuery query = new YouTubeQuery(feedUrl);
				Log<Youtube>.Debug("feedUrl for favorites videos: {0}", feedUrl);
				try{
					YouTubeFeed videoFeed = service.Query(query);
					while(videoFeed.Entries.Count > 0){				
						foreach (YouTubeEntry entry in videoFeed.Entries) 
						{
						    //Log<Youtube>.Debug("Video #{0}, Title: {1}", ++i, entry.Title.Text);
							string url = ("http://www.youtube.com/watch?v="+entry.VideoId);
							//Log<Youtube>.Debug("Video url: {0}", url);
							YoutubeVideoItem video = new YoutubeVideoItem(entry.Title.Text, url, entry.Media.Description.Value);
							favorites.Add(video);
						}
						startIndex += maxResults;
						feedUrl = "http://gdata.youtube.com/feeds/api/users/"+ username +"/favorites?start-index="+ startIndex +"&max-results="+maxResults;
						query = new YouTubeQuery(feedUrl);
						videoFeed = service.Query(query);
					}
					startIndex = 1;
					Log<Youtube>.Debug("Finished updating favorite videos");
				}catch(Exception e) {
					Log<Youtube>.Error ("Error getting favorites videos - {0}", e.Message);
    				Log<Youtube>.Debug (e.StackTrace);
				}
			}
		}
		
		public static void updateOwn()
		{
			ownUpdate++;
			Log<Youtube>.Debug("Update own videos tries = {0} - own.Count : {1}", ownUpdate, Youtube.own.Count);
			if (Youtube.own.Count == 0 || ownUpdate%20 == 0){
				Youtube.own.Clear();
				int maxResults = 50;
				int startIndex = 1;
				
				string feedUrl = "http://gdata.youtube.com/feeds/api/users/"+ username +"/uploads?start-index="+ startIndex +"&max-results="+maxResults;
				YouTubeQuery query = new YouTubeQuery(feedUrl);
				Log<Youtube>.Debug("feedUrl for own videos: {0}", feedUrl);
				try{
					YouTubeFeed videoFeed = service.Query(query);
					while(videoFeed.Entries.Count > 0){				
						foreach (YouTubeEntry entry in videoFeed.Entries) 
						{
						    //Log<Youtube>.Debug("Video #{0}, Title(own video): {1}", ++i, entry.Title.Text);
							string url = "http://www.youtube.com/watch?v="+entry.VideoId;
							YoutubeVideoItem video = new YoutubeVideoItem(entry.Title.Text, url, entry.Media.Description.Value);
							own.Add(video);
						}
						startIndex += maxResults;
						feedUrl = "http://gdata.youtube.com/feeds/api/users/"+ username +"/uploads?start-index="+ startIndex +"&max-results="+maxResults;
						query = new YouTubeQuery(feedUrl);
						videoFeed = service.Query(query);
					}
					Log<Youtube>.Debug("Finished updating own videos");
				}catch(Exception e) {
					Log<Youtube>.Error ("Error getting own videos - {0}", e.Message);
    				Log<Youtube>.Debug (e.StackTrace);
				}
			}
		}		
		
		public static void updateSubscriptions()
		{
			subUpdate++;
			Log<Youtube>.Debug("Update subscriptions tries = {0} - subscriptions.Count - {1}", subUpdate, Youtube.subscriptions.Count);
			if (Youtube.subscriptions.Count == 0 || subUpdate%20==0){
				Youtube.subscriptions.Clear();
				
				string feedUrl = "http://gdata.youtube.com/feeds/api/users/"+ username +"/subscriptions";
				YouTubeQuery query = new YouTubeQuery(feedUrl);
				Log<Youtube>.Debug("feedUrl for subscriptions: {0}", feedUrl);				
				try
				{
					SubscriptionFeed subFeed = service.GetSubscriptions(query);
					if(subFeed.Entries.Count > 0){
						foreach (SubscriptionEntry entry in subFeed.Entries)
						{
                            //Log<Youtube>.Debug("Subscriptions - {0}", ++i);
                            //Log<Youtube>.Debug("{0}", entry.Title.Text);
							string url = "http://www.youtube.com/user/" + entry.UserName;
							YouTubeSubscriptionItem subscription = new YouTubeSubscriptionItem(entry.UserName, url, entry.Title.Text);
							Youtube.subscriptions.Add(subscription);
						}
					}
					Log<Youtube>.Debug("Finished updating subscriptions");
				}
				catch(Exception e) 
				{
                    Log<Youtube>.Error ("Error getting subscriptions - {0}", e.Message);
    				Log<Youtube>.Debug (e.StackTrace);
				}
			}
		}		
		
		public static bool TryConnect (string username, string password)
		{
			try {
				service = new YouTubeService (appName, clientID, developerKey);
				service.setUserCredentials (username, password);
				Connect (username, password);
			} catch (Exception) {
				Log<Youtube>.Error (ConnectionErrorMessage);
				return false;
			}
			
			return true;
		}		
		
		private static void Connect (string username, string password) 
		{
			if (string.IsNullOrEmpty (username) || string.IsNullOrEmpty (password)) {
				Log<Youtube>.Error (MissingCredentialsMessage);
				return;
			}
			
			try {
				service = new YouTubeService (appName, clientID, developerKey);
				service.setUserCredentials (username, password);
			} catch (Exception e) {
				Log<Youtube>.Error (ConnectionErrorMessage);
			}
		}
	}
}

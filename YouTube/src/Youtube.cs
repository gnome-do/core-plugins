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

namespace Youtube
{
	public class Youtube
	{

		static readonly string ConnectionErrorMessage = AddinManager.CurrentLocalizer.GetString ("An error occurred connecting to YouTube, "
			+ "are your credentials valid?");

		static readonly string MissingCredentialsMessage = AddinManager.CurrentLocalizer.GetString ("Missing login credentials. Please set "
			+ "login information in YouTube plugin configuration.");

		private const string appName = "gnome-do-plugin";
		public const string searchUrl = "http://www.youtube.com/results?search_query=";
		private const string clientID = "gnome-do-client";
		private const string developerKey = "AI39si5utjLEVOmAty2JLxz8KlixVQkwbSsEZqUXVUV-hUK1zDctrUbujGL2kWJBs47a7CaO-LOf_FXUiyuvQ9j7pbq8YO9wsA";

		public static List<Item> favorites;
		public static List<Item> subscriptions;
		public static List<Item> own;

		private static YouTubeService service;
		private static string username;
		private static string password;

		private static int subUpdate;
		private static int favUpdate;
		private static int ownUpdate;

                private const string favoritesQueryTemplate = "http://gdata.youtube.com/feeds/api/users/default/favorites?start-index={0}&max-results={1}";
                private const string ownQueryTemplate = "http://gdata.youtube.com/feeds/api/users/default/uploads?start-index={0}&max-results={1}";
                private const string youtubeWatchUrlTemplate = "http://www.youtube.com/watch?v={0}";

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

                private static void parseFeed(YouTubeFeed feed, List<Item> videos)
                {
                    string description = "";
                    string url = null;
                    foreach(YouTubeEntry entry in feed.Entries) 
                    {
                        description = "";
                        url = String.Format(youtubeWatchUrlTemplate, entry.VideoId);
                        if (entry.Media.Description != null)
                        {
                            description = entry.Media.Description.Value;
                        }
                        YoutubeVideoItem video = new YoutubeVideoItem(entry.Title.Text, url, description);
                        videos.Add(video);
                    }
                }

                private static void update(string queryTemplate, List<Item> videos, ref int counter, string category)
                {
                    if (videos.Count != 0 || (counter % 20 != 0 && counter != 0))
                    {
                        counter = counter + 1;
                        return;
                    }

                    counter = counter + 1;

                    videos.Clear();
                    int maxResults = 50;
                    int startIndex = 1;

                    string feedUrl = String.Format(queryTemplate, startIndex, maxResults);

                    YouTubeQuery query = new YouTubeQuery(feedUrl);
                    YouTubeFeed videoFeed = null;

                    try
                    {
                        videoFeed = service.Query(query);
                        while(videoFeed.Entries.Count > 0)
                        {
                            parseFeed(videoFeed, videos);

                            startIndex += maxResults;
                            feedUrl = String.Format(queryTemplate, startIndex, maxResults);
                            query = new YouTubeQuery(feedUrl);
                            videoFeed = service.Query(query);
                        }
                        startIndex = 1;
                        Log<Youtube>.Debug("Finished updating {0} videos", category);
                    }
                    catch(Exception e)
                    {
                        Log<Youtube>.Error ("Error getting {0} videos - {1}", category, e.Message);
                        Log<Youtube>.Debug (e.StackTrace);
                    }

                }

		public static void updateFavorites()
		{
                        update (favoritesQueryTemplate, Youtube.favorites, ref favUpdate, "favorites");
		}

		public static void updateOwn()
		{
                        update (ownQueryTemplate, Youtube.own, ref ownUpdate, "own youtube");
                }

		public static void updateSubscriptions()
		{
			subUpdate++;
			Log<Youtube>.Debug("Update subscriptions tries = {0} - subscriptions.Count - {1}", subUpdate, Youtube.subscriptions.Count);
			if (Youtube.subscriptions.Count == 0 || subUpdate%20==0){
				Youtube.subscriptions.Clear();
				
				string feedUrl = "http://gdata.youtube.com/feeds/api/users/default/subscriptions";
				YouTubeQuery query = new YouTubeQuery(feedUrl);
				Log<Youtube>.Debug("feedUrl for subscriptions: {0}", feedUrl);				
				SubscriptionFeed subFeed = null;
				string url = "http://www.youtube.com/user/{0}";
				try
				{
					subFeed = service.GetSubscriptions(query);
					if(subFeed.Entries.Count > 0){
						foreach (SubscriptionEntry entry in subFeed.Entries)
						{
							YouTubeSubscriptionItem subscription = 
                                                            new YouTubeSubscriptionItem(entry.UserName, String.Format(url, entry.UserName), entry.Title.Text);
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
                                ServicePointManager.CertificatePolicy = new CertHandler ();
			} catch (Exception e) {
				Log<Youtube>.Error (ConnectionErrorMessage);
				Log<Youtube>.Error (e.Message);
			}
		}
	}
}

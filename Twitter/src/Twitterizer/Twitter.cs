using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Twitterizer.Framework
{
    public class Twitter
    {
        private string userName;
        private string password;

        public Twitter(string UserName, string Password)
        {
            userName = UserName;
            password = Password;
        }

        public TwitterStatus Update(string Status)
        {
            TwitterRequest Request = new TwitterRequest();
            TwitterRequestData Data = new TwitterRequestData();
            Data.UserName = userName;
            Data.Password = password;
            
            Data.ActionUri = new Uri(
                string.Format("http://twitter.com/statuses/update.xml?source=Do&status={0}",
                  HttpUtility.UrlEncode(Status)));
			
            Data = Request.PerformWebRequest(Data);

            return Data.Statuses[0];
        }

        public void Destroy(int ID)
        {
            TwitterRequest Request = new TwitterRequest();
            TwitterRequestData Data = new TwitterRequestData();
            Data.UserName = userName;
            Data.Password = password;

            Data.ActionUri = new Uri(
                string.Format("http://twitter.com/statuses/destroy/{0}.xml",ID));

            Data = Request.PerformWebRequest(Data);
        }

        public TwitterStatus Show(int ID)
        {
            TwitterRequest Request = new TwitterRequest();
            TwitterRequestData Data = new TwitterRequestData();
            Data.UserName = userName;
            Data.Password = password;

            Data.ActionUri = new Uri(
                string.Format("http://twitter.com/statuses/show/{0}.xml",ID));
            Data = Request.PerformWebRequest(Data);

            return Data.Statuses[0];
        }

        public TwitterStatusCollection FriendsTimeline()
        {
            TwitterRequest Request = new TwitterRequest();
            TwitterRequestData Data = new TwitterRequestData();
            Data.UserName = userName;
            Data.Password = password;

            Data.ActionUri = new Uri("http://twitter.com/statuses/friends_timeline.xml");

            Data = Request.PerformWebRequest(Data);

            return Data.Statuses;
        }

        public TwitterUserCollection Friends()
        {
            TwitterRequest Request = new TwitterRequest();
            TwitterRequestData Data = new TwitterRequestData();
            Data.UserName = userName;
            Data.Password = password;
            Data.ActionUri = new Uri("http://twitter.com/statuses/friends.xml");

            Data = Request.PerformWebRequest(Data);

            return Data.Users;
        }

        public TwitterStatusCollection UserTimeline()
        {
            TwitterRequest Request = new TwitterRequest();
            TwitterRequestData Data = new TwitterRequestData();

            Data.ActionUri = new Uri("http://twitter.com/statuses/user_timeline.xml");

            Data = Request.PerformWebRequest(Data);

            return Data.Statuses;
        }

        public TwitterStatusCollection PublicTimeline()
        {
            TwitterRequest Request = new TwitterRequest();
            TwitterRequestData Data = new TwitterRequestData();

            Data.ActionUri = new Uri("http://twitter.com/statuses/public_timeline.xml");

            Data = Request.PerformWebRequest(Data);

            return Data.Statuses;
        }

        public TwitterStatusCollection Replies()
        {
            TwitterRequest Request = new TwitterRequest();
            TwitterRequestData Data = new TwitterRequestData();
            Data.UserName = userName;
            Data.Password = password;

            Data.ActionUri = new Uri("http://twitter.com/statuses/replies.xml");

            Data = Request.PerformWebRequest(Data);

            return Data.Statuses;
        }

        public TwitterUserCollection Followers()
        {
            TwitterRequest Request = new TwitterRequest();
            TwitterRequestData Data = new TwitterRequestData();
            Data.UserName = userName;
            Data.Password = password;

            Data.ActionUri = new Uri("http://twitter.com/statuses/followers.xml");

            Data = Request.PerformWebRequest(Data);

            return Data.Users;
        }
    }
}

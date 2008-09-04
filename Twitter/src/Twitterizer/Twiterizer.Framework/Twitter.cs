/*
 * Twitter.cs
 *
 * Copyright © 2008 digitallyborn <digitallyborn@gmail.com>
 * Copyright © 2008 jmargolese  <jmargolese@gmail.com>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
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
                string.Format("http://twitter.com/statuses/destroy/{0}.xml",
                  ID));

            Data = Request.PerformWebRequest(Data);
        }

       
        public TwitterUser Show(string id_or_ScreenName)
        {
            TwitterRequest Request = new TwitterRequest();
            TwitterRequestData Data = new TwitterRequestData();
            Data.UserName = userName;
            Data.Password = password;

            Data.ActionUri = new Uri(
                string.Format("http://twitter.com/statuses/show/{0}.xml",
                  id_or_ScreenName));

            Data = Request.PerformWebRequest(Data, "GET");

            return Data.Users[0];
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

        public TwitterStatusCollection DirectMessages(ulong since_id)
        {
            TwitterRequest Request = new TwitterRequest();
            TwitterRequestData Data = new TwitterRequestData();
            Data.UserName = userName;
            Data.Password = password;

            Data.ActionUri = new Uri(
                string.Format("http://twitter.com/direct_messages.xml",
                since_id));

            Data = Request.PerformWebRequest(Data,"GET");

            return Data.Statuses;
        }

        public TwitterStatusCollection DirectMessagesSent()
        {
            return DirectMessagesSent(0);
        }

        public TwitterStatusCollection DirectMessagesSent(ulong since_id)
        {
            TwitterRequest Request = new TwitterRequest();
            TwitterRequestData Data = new TwitterRequestData();
            Data.UserName = userName;
            Data.Password = password;

            Data.ActionUri = new Uri(
                string.Format("http://twitter.com/direct_messages/sent.xml",
                since_id));

            Data = Request.PerformWebRequest(Data);

            return Data.Statuses;
        }
		/*
        public TwitterStatusCollection Archive()
        {
            return Archive(0);
        }
        
        
        public TwitterStatusCollection Archive(ulong since_id)
        {
            TwitterRequest Request = new TwitterRequest();
            TwitterRequestData Data = new TwitterRequestData();
            Data.UserName = userName;
            Data.Password = password;

            Data.ActionUri = new Uri(
                string.Format(,
                since_id));

            Data = Request.PerformWebRequest(Data);

            return Data.Statuses;
        }
		*/
		
        public TwitterUserCollection Friends()
        {
            return (Friends(1));
        }

        public TwitterUserCollection Friends(int page)
        {
            // page 0 == page 1 is the start
            TwitterRequest Request = new TwitterRequest();
            TwitterRequestData Data = new TwitterRequestData();
            Data.UserName = userName;
            Data.Password = password;
           
            Data.ActionUri = new Uri(
                    string.Format("http://twitter.com/statuses/friends.xml?page={0}", page));

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
            return (Followers(0));
        }

        public TwitterUserCollection Followers(int page)
        {
            TwitterRequest Request = new TwitterRequest();
            TwitterRequestData Data = new TwitterRequestData();
            Data.UserName = userName;
            Data.Password = password;

            
             Data.ActionUri = new Uri(
                    string.Format("http://twitter.com/statuses/followers.xml?page={0}", page));

            Data = Request.PerformWebRequest(Data);

            return Data.Users;
        }
    }
}

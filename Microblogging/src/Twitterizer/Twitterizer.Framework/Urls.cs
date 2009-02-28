/* HttpLocation.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
 *  
 * This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
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

namespace Twitterizer.Framework
{
	
	internal abstract class Urls
	{
		string rootUrl;
		
		public Urls (string rootUrl)
		{
			this.rootUrl = rootUrl;
		}
		
		public string UpdateUrl {
			get { return rootUrl + "statuses/update.xml?status={0}&source=Do&in_reply_to_status_id={1}"; }
		}
		
		public string RepliesUrl { 
			get { return rootUrl + "statuses/replies.xml"; }
		}
		
		public string DirectMessagesUrl { 
			get { return rootUrl + "direct_messages.xml"; }
		}
		
		public string SentDirectMessagesUrl { 
			get { return rootUrl + "direct_messages/sent.xml"; }
		}
		
		public string FriendsUrl { 
			get { return rootUrl + "statuses/friends.xml"; }
		}
		
		public string FollowersUrl { 
			get { return rootUrl + "statuses/followers.xml"; }
		}
		
		public string ShowStatusUrl	{ 
			get { return rootUrl + "users/show/{0}.xml"; }
		}
		
		public string DestroyStatusUrl { 
			get { return rootUrl + "statuses/destroy/{0}.xml"; }
		}
		
		public string UserTimelineUrl { 
			get { return rootUrl + "statuses/user_timeline.xml"; }
		}
		
		public string PublicTimelineUrl	{
			get { return rootUrl + "statuses/public_timeline.xml"; }
		}
		
		public string FriendsTimelineUrl {
			get { return rootUrl + "statuses/friends_timeline.xml"; }
		}
	}
	
	internal class TwitterUrls : Urls
	{
		const string RootUrl = "http://twitter.com/";
		
		public TwitterUrls () : base (RootUrl)
		{
		}
	}
	
	internal class IdenticaUrls : Urls
	{
		const string RootUrl = "http://identi.ca/api/";
		
		public IdenticaUrls () : base (RootUrl)
		{
		}
	}
}

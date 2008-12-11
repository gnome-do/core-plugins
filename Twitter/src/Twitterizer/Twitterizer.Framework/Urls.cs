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
	
	internal interface IUrls
	{
		string UpdateUrl			 { get; }
		string RepliesUrl			 { get; }
		string DirectMessagesUrl 	 { get; }
		string SentDirectMessagesUrl { get; }
		
		string FriendsUrl	{ get; }
		string FollowersUrl { get; }
		
		string ShowStatusUrl	{ get; }
		string DestroyStatusUrl { get; }
		
		string UserTimelineUrl		{ get; }
		string PublicTimelineUrl	{ get; }
		string FriendsTimelineUrl	{ get; }
	}
	
	internal class TwitterUrls : IUrls
	{
		public string UpdateUrl {
			get { return "http://twitter.com/statuses/update.xml?status={0}&source=Do&in_reply_to_status_id{1}"; }
		}
		
		public string RepliesUrl { 
			get { return "http://twitter.com/statuses/replies.xml"; }
		}
		
		public string DirectMessagesUrl { 
			get { return "http://twitter.com/direct_messages.xml"; }
		}
		
		public string SentDirectMessagesUrl { 
			get { return "http://twitter.com/direct_messages/sent.xml"; }
		}
		
		public string FriendsUrl { 
			get { return "http://twitter.com/statuses/friends.xml"; }
		}
		
		public string FollowersUrl { 
			get { return "http://twitter.com/statuses/followers.xml"; }
		}
		
		public string ShowStatusUrl	{ 
			get { return "http://twitter.com/users/show/{0}.xml"; }
		}
		
		public string DestroyStatusUrl { 
			get { return "http://twitter.com/statuses/destroy/{0}.xml"; }
		}
		
		public string UserTimelineUrl { 
			get { return "http://twitter.com/statuses/user_timeline.xml"; }
		}
		
		public string PublicTimelineUrl	{
			get { return "http://twitter.com/statuses/public_timeline.xml"; }
		}
		
		public string FriendsTimelineUrl {
			get { return "http://twitter.com/statuses/friends_timeline.xml"; }
		}
	}
	
	internal class IdenticaUrls : IUrls
	{
		public string UpdateUrl {
			get { return ""; }
		}
		
		public string RepliesUrl { 
			get { return ""; }
		}
		
		public string DirectMessagesUrl { 
			get { return ""; }
		}
		
		public string SentDirectMessagesUrl { 
			get { return ""; }
		}
		
		public string FriendsUrl { 
			get { return ""; }
		}
		
		public string FollowersUrl { 
			get { return ""; }
		}
		
		public string ShowStatusUrl	{ 
			get { return ""; }
		}
		
		public string DestroyStatusUrl { 
			get { return ""; }
		}
		
		public string UserTimelineUrl { 
			get { return ""; }
		}
		
		public string PublicTimelineUrl	{
			get { return ""; }
		}
		
		public string FriendsTimelineUrl {
			get { return ""; }
		}
	}
}

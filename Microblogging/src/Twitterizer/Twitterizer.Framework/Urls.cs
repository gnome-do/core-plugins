/*
 * This file is part of the Twitterizer library <http://code.google.com/p/twitterizer/>
 *
 * Copyright (c) 2009, Alex Launi <alex.launi@gmail.com>
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are 
 * permitted provided that the following conditions are met:
 *
 * - Redistributions of source code must retain the above copyright notice, this list 
 *   of conditions and the following disclaimer.
 * - Redistributions in binary form must reproduce the above copyright notice, this list 
 *   of conditions and the following disclaimer in the documentation and/or other 
 *   materials provided with the distribution.
 * - Neither the name of the Twitterizer nor the names of its contributors may be 
 *   used to endorse or promote products derived from this software without specific 
 *   prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND 
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
 * IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, 
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT 
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR 
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
 */

using System;

namespace Twitterizer.Framework
{
	
	internal abstract class Urls
	{
		public abstract string RootUrl { get; }
		
		public string UpdateUrl {
			get { return RootUrl + "statuses/update.xml?status={0}&source={1}&in_reply_to_status_id={2}"; }
		}
		
		public string RepliesUrl { 
			get { return RootUrl + "statuses/replies.xml"; }
		}
		
		public string DirectMessagesUrl { 
			get { return RootUrl + "direct_messages.xml"; }
		}
		
		public string SentDirectMessagesUrl { 
			get { return RootUrl + "direct_messages/sent.xml"; }
		}
		
		public string FriendsUrl { 
			get { return RootUrl + "statuses/friends.xml"; }
		}
		
		public string FollowersUrl { 
			get { return RootUrl + "statuses/followers.xml"; }
		}
		
		public string ShowStatusUrl	{ 
			get { return RootUrl + "users/show/{0}.xml"; }
		}
		
		public string DestroyStatusUrl { 
			get { return RootUrl + "statuses/destroy/{0}.xml"; }
		}
		
		public string UserTimelineUrl { 
			get { return RootUrl + "statuses/user_timeline.xml"; }
		}
		
		public string PublicTimelineUrl	{
			get { return RootUrl + "statuses/public_timeline.xml"; }
		}
		
		public string FriendsTimelineUrl {
			get { return RootUrl + "statuses/friends_timeline.xml"; }
		}
		
		public string VerifyCredentialsUrl {
			get { return RootUrl + "account/verify_credentials.xml"; }
		}
	}
	
	internal class TwitterUrls : Urls
	{
		public override string RootUrl {
			get { return "http://twitter.com/"; }
		}
	}
	
	internal class IdenticaUrls : Urls
	{
		public override string RootUrl {
			get { return "http://identi.ca/api/"; }
		}
	}
}

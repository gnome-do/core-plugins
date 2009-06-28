/*
	Copyright (c) 2008, Ping.fm Inc.
	All rights reserved.
	http://ping.fm/

  Redistribution and use in source and binary forms, with or without
  modification, are permitted provided that the following conditions
  are met:

	- Redistributions of source code must retain the above copyright
	   notice, this list of conditions and the following disclaimer. 

	- Neither the name Ping.fm, nor the names of its contributors
	   may be used to endorse or promote products derived from this
	   software without specific prior written permission. 

  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
  FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
  THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
  INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
  HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
  STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
  ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
  OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PingFM.API
{
	/// <summary>Ping.fm API wrapper for C#.
	/// Created by Adam Duffy (adam@ping.fm) 06/18/2008
	/// </summary>
	public class PingFMApi
	{
		public PingFMApi() { }
		public PingFMApi(string user_application_key)
		{
			this.user_application_key = user_application_key;
		}
		
		private static Version mVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
		public static Version Version
		{
			get { return mVersion; }
		}
		
		/// <summary>Ping.FM Developer API Key.	 See http://ping.fm/developers/</summary>
		public static string api_key = "bc4f5d28442893476d066e2e1be9129b";
		
		/// <summary>User Application Key.
		/// The end user will need to enter this. see http://ping.fm/key/
		/// </summary>
		public string user_application_key;
		
		#region XML Serialization Classes
		
		public class ServiceMethods
		{
			[XmlAttribute("id")]	public string ID;
			[XmlAttribute("name")]	public string Name;
			[XmlElement("methods")] public string Methods;
			[XmlElement("url")]		public string Url;
			[XmlElement("trigger")] public string Trigger;
			public override string ToString()
			{
				if (!string.IsNullOrEmpty(Name))
					return Name + " [" + ID + "]";
				else
					return base.ToString();
			}
		}
		public class Service
		{
			[XmlAttribute("id")]	public string ID;
			[XmlAttribute("name")]	public string Name;
			[XmlElement("methods")] public string Methods;
			public override string ToString()
			{
				if (!string.IsNullOrEmpty(Name))
					return Name + " [" + ID + "]";
				else
					return base.ToString();
			}
		}
		public class Trigger
		{
			[XmlAttribute("id")]	 public string ID;
			[XmlAttribute("method")] public string Method;
			[XmlArray("services"), XmlArrayItem("service", typeof(Service))]
			public List<Service> Services = new List<Service>();
			public override string ToString()
			{
				if (!string.IsNullOrEmpty(ID))
					return ID + " (" + Method + ")";
				else
					return base.ToString();
			}
		}
		public class Message
		{
			public struct PingDate
			{
				[XmlAttribute("rfc")]  public string Rfc;
				[XmlAttribute("unix")] public string Unix;
				public override string ToString()
				{
					if (!string.IsNullOrEmpty(Rfc))
						return Rfc;
					else
						return base.ToString();
				}
			}
			public struct PingContent
			{
				[XmlElement("title")] public string Title;
				[XmlElement("body")]  public string Body;
				public override string ToString()
				{
					if (!string.IsNullOrEmpty(Title))
						return Title;
					else if (!string.IsNullOrEmpty(Body))
						return Body;
					else
						return base.ToString();
				}
			}
			[XmlAttribute("id")]	 public string ID;
			[XmlAttribute("method")] public string Method;
			[XmlElement("date")]	 public PingDate Date;
			[XmlElement("content")]	 public PingContent Content;
			[XmlArray("services"), XmlArrayItem("service", typeof(Service))]
			public List<Service> Services = new List<Service>();
			public void Decode()
			{
				Content.Body = Base64Decode(Content.Body);
				Content.Title = Base64Decode(Content.Title);
			}
			public override string ToString()
			{
				if (!string.IsNullOrEmpty(Method))
					return Method;
				else
					return base.ToString();
			}
		}

		[XmlRoot("rsp")]
		public class PingResponse
		{
			[XmlAttribute("status")]	public string Status;
			[XmlElement("transaction")] public string Transaction;
			[XmlElement("method")]		public string Method;
			[XmlElement("message")]		public string Message;
		}

		[XmlRoot("rsp")]
		public class ServicesResponse : PingResponse
		{
			[XmlArray("services"), XmlArrayItem("service", typeof(ServiceMethods))]
			public List<ServiceMethods> Services;
		}
		
		[XmlRoot("rsp")]
		public class TriggerResponse : PingResponse
		{
			[XmlArray("triggers"), XmlArrayItem("trigger", typeof(Trigger))]
			public List<Trigger> Triggers;
		}
		
		[XmlRoot("rsp")]
		public class LatestResponse : PingResponse
		{
			[XmlArray("messages"), XmlArrayItem("message", typeof(Message))]
			public List<Message> Messages = new List<Message>();
			public void DecodeMessages()
			{
				if (Messages == null) return;
				foreach (Message m in Messages) 
					m.Decode();
			}
		}
		
		[XmlRoot("rsp")]
		public class MessageResponse : PingResponse
		{
			//.net has a problem deserializing this one.
			//could un-inherit the class, but i'll leave the bug here.
			[XmlElement("message")]
			public new Message Message;
		}
		
		// private bool SerializeObject(XmlTextWriter writer, object o)
//		  {
//			  try
//			  {
//				  XmlSerializer xs = new XmlSerializer(o.GetType());
//				  XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
//				  ns.Add("", ""); //remove the xmlns tags.
//				  xs.Serialize(writer, o, ns);
//				  return true;
//			  }
//			  catch (Exception ex)
//			  {
//				  System.Diagnostics.Debug.WriteLine(ex.Message);
//				  return false;
//			  }
//		  }
		private object DeserializeObject(XmlReader reader, Type type)
		{
			try
			{
				XmlSerializer xs = new XmlSerializer(type);
				return xs.Deserialize(reader);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
				DeserializeException = ex;
				return null;
			}
		}
		
		#endregion
		
		private HttpWebRequest CreateHttpWebRequest(string url)
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
			request.Headers.Add("Accept-Language", "en-us");
			request.Accept = "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
			request.UserAgent = "PingFM.dll " + Version.ToString(2);
			return request;
		}
		
		private string GetWebResponse(string url, string PostData)
		{
//			Console.WriteLine ("[Ping.FM] Request URL:  {0}", url);
//			Console.WriteLine ("[Ping.FM] Request Data: {0}", PostData);
			string html = "";
			try
			{
				HttpWebRequest request = CreateHttpWebRequest(url);
				request.Method = "POST";
				request.ContentType = "application/x-www-form-urlencoded";
				request.Timeout = 30000;
				
//				byte[] b = Encoding.Default.GetBytes(PostData);
//				  request.GetRequestStream().Write(b, 0, b.Length);
//				  request.GetRequestStream().Close();				
//				  request.Timeout = 10000;
//				  HttpWebResponse response = (HttpWebResponse)request.GetResponse();
//				  StreamReader sr = new StreamReader(response.GetResponseStream());
//				  sr.BaseStream.ReadTimeout = 10000;
//				  html = sr.ReadToEnd();
//				  sr.Close();
//				  response.Close();

				// The code commented out above is the original one which produces 
				// an exception when used with Mono: "Cannot re-call start of asynchronous
				// method while a previous call is still in progress."
				//
				// It is therefore modified by Peng Deng <dengpeng@gmail.com> on 2008-09-08
				// as following:
				
				request.ContentLength = PostData.Length;
				
				// Write the request
				StreamWriter sw = new StreamWriter (request.GetRequestStream(), Encoding.ASCII);
				sw.Write(PostData);
				sw.Close();
				
				// Do the request to get the response
				StreamReader sr = new StreamReader (request.GetResponse().GetResponseStream());
				html = sr.ReadToEnd();
				sr.Close();

//				Console.WriteLine ("[Ping.FM] Response data:");
//				Console.WriteLine (html);
			}
			catch (Exception ex)
			{
				WebException = ex;
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
			return html;
		}
		
		private PingResponse mLastResponse;
		/// <summary>Gets the last PingResponse</summary>
		public PingResponse LastResponse { get { return mLastResponse; } }
		
		/// <summary>If a web server error occurs, the exception will be stored here for reference.</summary>
		public Exception WebException;
		
		/// <summary>If an error occurs during deserialization, the exception will be stored here for reference.</summary>
		public Exception DeserializeException;
		
		/// <summary>Clears the LastResponse, WebException, and DeserializeException</summary>
		public void Reset()
		{
			mLastResponse = null;
			WebException = null;
			DeserializeException = null;
		}
		
		/// <summary>Validates the given user's application key.</summary>
		public PingResponse Validate()
		{
			string url = "http://api.ping.fm/v1/user.validate";
			string postdata = "api_key={0}&user_app_key={1}";
			postdata = string.Format(postdata, api_key, user_application_key);
			string response = GetWebResponse(url, postdata);
			XmlReader xr = XmlReader.Create(new System.IO.StringReader(response));
			PingResponse r = (PingResponse)DeserializeObject(xr, typeof(PingResponse));
			xr.Close();
			mLastResponse = r;
			return r;
		}
		
		/// <summary>Returns a list of services the particular user has set up through Ping.fm.</summary>
		public ServicesResponse GetServices()
		{
			string url = "http://api.ping.fm/v1/user.services";
			string postdata = "api_key={0}&user_app_key={1}";
			postdata = string.Format(postdata, api_key, user_application_key);
			string response = GetWebResponse(url, postdata);
			XmlReader xr = XmlReader.Create(new System.IO.StringReader(response));
			ServicesResponse r = (ServicesResponse)DeserializeObject(xr, typeof(ServicesResponse));
			xr.Close();
			mLastResponse = r;
			return r;
		}
		
		/// <summary>Validates the given user's application key.</summary>
		public TriggerResponse GetTriggers()
		{
			string url = "http://api.ping.fm/v1/user.triggers";
			string postdata = "api_key={0}&user_app_key={1}";
			postdata = string.Format(postdata, api_key, user_application_key);
			string response = GetWebResponse(url, postdata);
			XmlReader xr = XmlReader.Create(new System.IO.StringReader(response));
			TriggerResponse r = (TriggerResponse)DeserializeObject(xr, typeof(TriggerResponse));
			xr.Close();
			mLastResponse = r;
			return r;
		}
		
		/// <summary>Returns the last 25 messages a user has posted through Ping.fm.</summary>
		public LatestResponse GetLatest()
		{
			return GetLatest(-1, null);
		}
		/// <summary>Returns the last X messages a user has posted through Ping.fm.</summary>
		/// <param name="limit">Number of messages to query</param>
		/// <param name="order">Order of results (ASC/DESC)</param>
		public LatestResponse GetLatest(int limit, string order)
		{
			string url = "http://api.ping.fm/v1/user.latest";
			string postdata = "api_key={0}&user_app_key={1}";
			postdata = string.Format(postdata, api_key, user_application_key);
			if (limit > -1) postdata += "&limit=" + limit.ToString();
			if (!string.IsNullOrEmpty(order)) postdata += "&order=" + order;
			string response = GetWebResponse(url, postdata);
			XmlReader xr = XmlReader.Create(new System.IO.StringReader(response));
			LatestResponse r = (LatestResponse)DeserializeObject(xr, typeof(LatestResponse));
			xr.Close();
			if (r != null) r.DecodeMessages();
			mLastResponse = r;
			return r;
		}
		
		/// <summary>Returns data for the specified MessageID.</summary>
		/// <param name="MessageID">MessageID to query</param>
		[Obsolete("Use user.latest to get a recent message history", true)]
		public MessageResponse GetMessage(string MessageID)
		{
			string url = "http://api.ping.fm/v1/user.message";
			string postdata = "api_key={0}&user_app_key={1}&message_id={2}";
			postdata = string.Format(postdata, api_key, user_application_key, MessageID);
			string response = GetWebResponse(url, postdata);
			XmlReader xr = XmlReader.Create(new System.IO.StringReader(response));
			MessageResponse r = (MessageResponse)DeserializeObject(xr, typeof(MessageResponse));
			xr.Close();
			if (r != null && r.Message != null) r.Message.Decode();
			mLastResponse = r;
			return r;
		}
		
		/// <summary>Posts a message to the user's Ping.fm services.</summary>
		public PingResponse Post(string Body)
		{
			return Post(null, null, Body, null, null, false);
		}
		public PingResponse Post(string Method, string Body)
		{
			return Post(Method, null, Body, null, null, false);
		}
		public PingResponse Post(string Method, string Title, string Body)
		{
			return Post(Method, Title, Body, null, null, false);
		}
		public PingResponse Post(string Method, string Title, string Body, string Service)
		{
			return Post(Method, Title, Body, Service, null, false);
		}
		public PingResponse Post(string Method, string Title, string Body, 
		                         string Service, string Media, bool debug)
		{
			string url = "http://api.ping.fm/v1/user.post";
			string postdata = "api_key={0}&user_app_key={1}&post_method={2}";
			if (string.IsNullOrEmpty(Method)) Method = "default";
			postdata = string.Format(postdata, api_key, user_application_key, Method);
			if (!string.IsNullOrEmpty(Title)) postdata += "&title=" + UrlEncode(Title);
			if (!string.IsNullOrEmpty(Body)) postdata += "&body=" + UrlEncode(Body);
			if (!string.IsNullOrEmpty(Service)) postdata+="&service=" + Service;
			if (!string.IsNullOrEmpty(Media)) postdata+="&media=" + Media;
			if (debug) postdata += "&debug=1";
			string response = GetWebResponse(url, postdata);
			XmlReader xr = XmlReader.Create(new System.IO.StringReader(response));
			PingResponse r = (PingResponse)DeserializeObject(xr, typeof(PingResponse));
			xr.Close();
			mLastResponse = r;
			return r;
		}
		
		/// <summary>Posts a message to the user's Ping.fm services using one of their custom triggers.</summary>
		public PingResponse TPost(string Trigger, string Body)
		{
			return TPost(Trigger, null, Body, false);
		}
		public PingResponse TPost(string Trigger, string Title, string Body)
		{
			return TPost(Trigger, Title, Body, false);
		}
		public PingResponse TPost(string Trigger, string Title, string Body, bool debug)
		{
			string url = "http://api.ping.fm/v1/user.tpost";
			string postdata = "api_key={0}&user_app_key={1}";
			postdata = string.Format(postdata, api_key, user_application_key);
			if (!string.IsNullOrEmpty(Trigger)) postdata += "&trigger=" + Trigger;
			if (!string.IsNullOrEmpty(Title)) postdata += "&title=" + UrlEncode(Title);
			if (!string.IsNullOrEmpty(Body)) postdata += "&body=" + UrlEncode(Body);
			if (debug) postdata += "&debug=1";
			string response = GetWebResponse(url, postdata);
			XmlReader xr = XmlReader.Create(new System.IO.StringReader(response));
			PingResponse r = (PingResponse)DeserializeObject(xr, typeof(PingResponse));
			xr.Close();
			mLastResponse = r;
			return r;
		}
		
		public static string Base64Encode(string ToEncode)
		{
			if (string.IsNullOrEmpty(ToEncode)) return ToEncode;
			try
			{
				//this method is a little more work, but supports unicode.
				char[] chars = ToEncode.ToCharArray();
				byte[] bytes = Encoding.UTF8.GetBytes(chars);
				return Convert.ToBase64String(bytes);
			}
			catch
			{
				return ToEncode;
			}
		}
		
		public static string Base64Decode(string ToDecode)
		{
			try
			{
				return Encoding.Default.GetString(Convert.FromBase64String(ToDecode));
			}
			catch { return ToDecode; }
		}
		
		private static string UrlEncode(string text)
		{
			return System.Web.HttpUtility.UrlEncode(text, Encoding.Default);
		}
	}
}

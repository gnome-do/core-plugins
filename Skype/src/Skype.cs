
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using NDesk.DBus;
using org.freedesktop.DBus;

using Mono.Addins;

using Do.Universe;
using Do.Platform;

namespace Skype
{
	
	public enum OnlineStatus {
		Unknown,
		Online,
		Offline,
		SkypeMe,
		Away,
		NotAvailable,
		DoNotDisturb,
		Invisible,
		LoggedOut,
		SkypeOut,
	}
	
	
	public class Skype
	{
		const string SkypeObjectPath = "/com/Skype";
		const string SkypeServiceBusName = "com.Skype.API";

		[Interface ("com.Skype.API")]
		public interface ISkype
		{
			string Invoke (string request);
		}
		
		static Skype ()
		{
			Statuses = new Dictionary<OnlineStatus, StatusItem> () {
				{ OnlineStatus.Unknown, new StatusItem (AddinManager.CurrentLocalizer.GetString ("Unknown"), "UNKNOWN", "StatusPending_128x128.png") },
				{ OnlineStatus.Online, new StatusItem (AddinManager.CurrentLocalizer.GetString ("Online"), "ONLINE", "StatusOnline_128x128.png") },
				{ OnlineStatus.Offline, new StatusItem (AddinManager.CurrentLocalizer.GetString ("Offline"), "OFFLINE", "StatusOffline_128x128.png") },
				{ OnlineStatus.SkypeMe, new StatusItem (AddinManager.CurrentLocalizer.GetString ("Skype Me"), "SKYPEME", "StatusSkypeMe_128x128.png") },
				{ OnlineStatus.Away, new StatusItem (AddinManager.CurrentLocalizer.GetString ("Away"), "AWAY", "StatusAway_128x128.png") },
				{ OnlineStatus.NotAvailable, new StatusItem (AddinManager.CurrentLocalizer.GetString ("Not Available"), "NA", "StatusNotAvailable_128x128.png") },
				{ OnlineStatus.DoNotDisturb, new StatusItem (AddinManager.CurrentLocalizer.GetString ("Do Not Disturb"), "DND", "StatusDoNotDisturb_128x128.png") },
				{ OnlineStatus.Invisible, new StatusItem (AddinManager.CurrentLocalizer.GetString ("Invisible"), "INVISIBLE", "StatusInvisible_128x128.png") },
				{ OnlineStatus.LoggedOut, new StatusItem (AddinManager.CurrentLocalizer.GetString ("Logged Out"), "LOGGEDOUT", "StatusOffline_128x128.png") },
				{ OnlineStatus.SkypeOut, new StatusItem (AddinManager.CurrentLocalizer.GetString ("Skype Out"), "SKYPEOUT", "SkypeOut_128x128.png") },

			};
			
			SkypeApplicationItem = Services.UniverseFactory.MaybeApplicationItemFromCommand ("skype");
			
			TryGetSkypeObject ();
		}
		
		public Skype ()
		{
		}
		
		public static Dictionary<OnlineStatus, StatusItem> Statuses { get; private set; }
			
		static ISkype SkypeObject;
		static IApplicationItem SkypeApplicationItem;
		
		private static void TryGetSkypeObject ()
		{
			SkypeObject = GetSkypeObject ();
		}
		
		public static ISkype GetSkypeObject ()
		{
			if (SkypeObject != null)
				return SkypeObject;
				
			ISkype skype;
			
			try {
				skype = Bus.Session.GetObject<ISkype>
					(SkypeServiceBusName, new ObjectPath (SkypeObjectPath));
				if (skype.Invoke ("NAME GNOME_Do_Skype") != "OK")
					throw new Exception ("Skype did not return OK");
				if (skype.Invoke ("PROTOCOL 7") != "PROTOCOL 7")
				    throw new Exception ("Skype did not accept protocol 7");
				return skype;
			} catch (Exception e) {
				Log<Skype>.Error ("Skype DBUS initialization error: {0}", e.Message);
				Log<Skype>.Debug (e.StackTrace);
				return null;
			}
		}
		
		public static bool IsSkype (Item item) 
		{
			return item.Equals (SkypeApplicationItem);
		}
		
		public static bool InstanceIsRunning
		{
			get {
				Process pidof;
				ProcessStartInfo pidofInfo = new ProcessStartInfo ("pidof", "skype.real");
				pidofInfo.UseShellExecute = false;
				pidofInfo.RedirectStandardError = true;
				pidofInfo.RedirectStandardOutput = true;
								
				try {
					// Use pidof command to look for the skype process. Exit
					// status is 0 if at least one matching process is found.
					// If there's any error, just assume some Purple client
					// is running.
					pidof = Process.Start (pidofInfo);
					pidof.WaitForExit ();
					return pidof.ExitCode == 0;
				} catch {
					return true;
				}
			}
		}

		public static void StartIfNecessary ()
		{
			if (!InstanceIsRunning) {
				Process.Start ("skype");
				// 6 seconds should be enough
				System.Threading.Thread.Sleep (6 * 1000);
			}
		}
		
		private static string Get (string request, params object[] args)
		{
			request = string.Format (request, args);
			string reply = SkypeObject.Invoke (string.Format ("GET {0}", request));
			if (!reply.StartsWith ("ERROR"))
				return reply.Substring (request.Length).Trim ();
			Log<Skype>.Warn ("Skype failed for {0}", request);
			Log<Skype>.Debug ("Skype replied with: {0}", reply);
			return "";
		}
		
		public static void SetStatus (StatusItem newStatus)
		{
			if (SkypeObject == null) {
				TryGetSkypeObject ();
				if (SkypeObject == null)
					return;
			}
			
			SkypeObject.Invoke (string.Format ("SET USERSTATUS {0}", newStatus.Code));
		}
		
		public static IEnumerable<string> ContactHandles {
			get {
				if (SkypeObject == null) {
					TryGetSkypeObject ();
					if (SkypeObject == null)
						yield break;
				}
				
				IEnumerable<string> groups = SkypeObject.Invoke ("SEARCH GROUPS ALL") .Substring (7).Split (new [] {','});

				string onlineGroup = groups.FirstOrDefault (g => 
				    SkypeObject.Invoke (string.Format ("GET GROUP {0} TYPE", g.Trim ())).Contains ("SKYPE_FRIENDS"));
				
				if (string.IsNullOrEmpty (onlineGroup)) {
					Log<Skype>.Error ("Could not find online group.");
					groups.ForEach (g => {
						Log<Skype>.Debug (SkypeObject.Invoke (string.Format ("GET GROUP {0} TYPE", g.Trim())));
					} );
					yield break;
				}
				
				string handlesReply = Skype.Get ("GROUP {0} USERS", onlineGroup);

				if (handlesReply.StartsWith ("ERROR")) {
					Log<Skype>.Error ("Could not fetch friend handles.");
					Log<Skype>.Debug ("Skype returned: {0}", handlesReply);
				}
				
				IEnumerable<string> handles = handlesReply.Split (new [] {','});

				foreach (string handle in handles)
					yield return handle.Trim ();
                                  
				yield break;
			}
		}
		
		public static StatusItem ContactStatus (string handle)
		{
			return Statuses.Values.FirstOrDefault (s => s.Code == Skype.Get ("USER {0} ONLINESTATUS", handle)) ?? 
				Statuses [OnlineStatus.Unknown];
		}
		
		public static string ContactFullName (string handle) 
		{
			return Skype.Get ("USER {0} FULLNAME", handle);
		}
		
		public static string ContactDisplayName (string handle)
		{
			return Skype.Get ("USER {0} DISPLAYNAME", handle);
		}
		
		public static string ContactMood (string handle)
		{
			return Skype.Get ("USER {0} MOOD_TEXT", handle);
		}
		
		public static string ContactHomePhone (string handle)
		{
			return Skype.Get ("USER {0} PHONE_HOME", handle);
		}
			
		public static string ContactOfficePhone (string handle)
		{
			return Skype.Get ("USER {0} PHONE_OFFICE", handle);
		}
		
		public static string ContactMobilePhone (string handle)
		{
			return Skype.Get ("USER {0} PHONE_MOBILE", handle);
		}
		
		public static void ChatWith (string handle)
		{
			ChatWith (handle, "");
		}
		
		public static void ChatWith (string handle, string message)
		{
			string chatID = Regex.Match (SkypeObject.Invoke (string.Format ("CHAT CREATE {0}", handle)), "CHAT (.+) STATUS DIALOG").Groups [1].Value;

			if (!string.IsNullOrEmpty (chatID))
				SkypeObject.Invoke (string.Format ("OPEN CHAT {0}", chatID));
			
			if (!string.IsNullOrEmpty (message)) {
				Log<Skype>.Debug ("Opening chat with: {0}", handle);
				SkypeObject.Invoke (string.Format ("CHATMESSAGE {0} {1}", chatID, message));
			}
		}
		
		public static void Call (ContactItem contact)
		{
			Skype.Call (contact ["handle.skype"]);
		}
		
		public static void Call (string request)
		{
			if (!Skype.InstanceIsRunning) {
				Log<Skype>.Debug ("Starting Skype.");
				Skype.StartIfNecessary ();
			}
			
			if (SkypeObject == null) {
				TryGetSkypeObject ();
				if (SkypeObject == null)
					return;
			}

			Log<Skype>.Debug ("Calling {0}", request);
			SkypeObject.Invoke (string.Format ("CALL {0}", request));
		}
		
		public static string StripPhoneChars (string input) 
		{
			new [] {"(", ")", ".", "-", " "}.ForEach (s => input = input.Replace (s, ""));
			
			return input;
		}
	}
}
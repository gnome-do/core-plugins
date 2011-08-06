// Pidgin.cs
//
// GNOME Do is the legal property of its developers, whose names are too
// numerous to list here.  Please refer to the COPYRIGHT file distributed with
// this source distribution.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

#if USE_DBUS_SHARP
using DBus;
#else
using NDesk.DBus;
#endif

using org.freedesktop.DBus;

using Do.Platform;
using Do.Universe;

namespace PidginPlugin
{

	public class Pidgin
	{

		const string PurpleObjectPath = "/im/pidgin/purple/PurpleObject";
		const string PurpleServiceBusName = "im.pidgin.purple.PurpleService";

		[Interface ("im.pidgin.purple.PurpleInterface")]
		public interface IPurpleObject
		{
			int[] PurpleAccountsGetAll ();
			int PurpleBuddyGetIcon (int buddy);
			int[] PurpleAccountsGetAllActive ();
			bool PurpleBuddyIsOnline (int buddy);
			int PurpleBuddyGetAccount (int buddy);
			string PurpleAccountGetAlias (int account);
			bool PurpleAccountIsConnected (int account);
			string PurpleBuddyGetLocalAlias (int buddy);
			string PurpleBuddyGetServerAlias (int buddy);
			string PurpleBuddyIconGetFullPath (int icon);
			string PurpleAccountGetUsername (int account);
			int PurpleFindBuddy (int account, string name);
			string PurpleAccountGetProtocolName (int account);
			int[] PurpleFindBuddies (int account, string name);
			int PurpleAccountsFindConnected (string account, string proto);
			void PurpleAccountSetEnabled (int account, string ui, int val);

			int PurpleSavedstatusGetCurrent ();
			int [] PurpleSavedstatusesGetAll ();
			int PurpleSavedstatusFind (string title);
			void PurpleSavedstatusActivate (int status);
			int PurpleSavedstatusGetType (int saved_status);
			int PurpleSavedstatusNew (string title, int type);
			string PurpleSavedstatusGetTitle (int saved_status);
			bool PurpleSavedstatusIsTransient (int saved_status);
			string PurpleSavedstatusGetMessage (int saved_status);
			void PurpleSavedstatusSetMessage (int type, string message);
			
			int PurpleConversationGetImData (int conv);
			void PurpleConvImSend (int im, string message);
			void PurpleConversationPresent (int conversation);
			int PurpleConversationNew (int type, int account, string name);
			
			#region Pidgin < 2.5.4 compatibility methods
			
			int PurpleSavedstatusNew (string title, uint type);
			int PurpleConversationNew (uint type, int account, string name);
			void PurpleAccountSetEnabled (int account, string ui, uint val);
			
			#endregion
		}

		public static string ChatIcon {
			get { return "internet-group-chat.svg@" + typeof (Pidgin).Assembly.FullName; }
		}
		
		public static bool IsPidgin (Item item) 
		{
			return item.Equals (Do.Platform.Services.UniverseFactory.MaybeApplicationItemFromCommand ("pidgin"));
		}

		public static string GetProtocolIcon (string proto)
		{
			string icon = null;

			proto = proto.ToLower ();
			string[] parts = proto.Split ('-');
			
			if (parts.Length >= 2) {
				if (parts[0] == "prpl")
					proto = parts[1];
			}
			icon = Path.Combine ("/usr/share/pixmaps/pidgin/protocols/48", proto + ".png");
			return File.Exists (icon) ? icon : Pidgin.ChatIcon;
		}
		
		public static string GetBuddyIconPath (int buddyID)
		{
			IPurpleObject prpl = GetPurpleObject ();

			int icon = prpl.PurpleBuddyGetIcon (buddyID);
			if (icon == 0) 
				return null;
			
			string iconPath = prpl.PurpleBuddyIconGetFullPath (icon);
			return (File.Exists (iconPath)) ? iconPath : null;
		}

		public static IPurpleObject GetPurpleObject ()
		{
			try {
				return Bus.Session.GetObject<IPurpleObject>
					(PurpleServiceBusName, new ObjectPath (PurpleObjectPath));
			} catch {
				return null;
			}
		}
		
		private static IEnumerable<int> ConnectedAccounts {
			get {
				IPurpleObject prpl = GetPurpleObject ();

				return prpl.PurpleAccountsGetAllActive ().Where (acct => prpl.PurpleAccountIsConnected (acct));
			}
		}
		
		public static int GetAccountID (string name, string proto) 
		{
			IPurpleObject prpl = GetPurpleObject ();
			int account;
			try {
				account = prpl.PurpleAccountsFindConnected (name, proto); 
			} catch { 
				account = -1;
			}			
			return account;
		}
		
		public static IEnumerable<int> FindBuddies (int account, string name)
		{
			IPurpleObject prpl = GetPurpleObject ();	

			return prpl.PurpleFindBuddies (account, name);
		}
		
		public static string GetBuddyLocalAlias (int buddy)
		{
			IPurpleObject prpl = GetPurpleObject ();
			
			if (!InstanceIsRunning)
				return null;
				
			//don't need to do too much error checking here,
			//this method should always return something and it's only used
			//for bonjour, which _will_ always return what I'm looking for.
			return prpl.PurpleBuddyGetLocalAlias (buddy);
		}
		
		public static string GetBuddyServerAlias (string name)
		{
			IPurpleObject prpl = GetPurpleObject ();
			int buddy;
			string alias;
			
			if (!InstanceIsRunning)
				return null;
			
			foreach (int account in ConnectedAccounts) {
				buddy = prpl.PurpleFindBuddy (account, name);
				
				if (buddy == 0) 
					continue;
				
				alias = prpl.PurpleBuddyGetServerAlias (buddy);
				return string.IsNullOrEmpty (alias) ? null : alias;
			}
			return null;
		}
		
		public static string GetBuddyServerAlias (int buddy)
		{
			IPurpleObject prpl = GetPurpleObject ();
			
			if (!InstanceIsRunning)
				return null;
			
			string alias = prpl.PurpleBuddyGetServerAlias (buddy);
			return string.IsNullOrEmpty (alias) ? null : alias;
		}

		public static bool BuddyIsOnline (string name)
		{
			int account;
			return GetBuddyIsOnlineAndAccount (name, out account);   
		}

		public static bool GetBuddyIsOnlineAndAccount (string name, out int account_out)
		{
			IPurpleObject prpl;
			int buddy;
		   
			prpl = GetPurpleObject ();
			account_out = -1;
			
			try {
				foreach (int account in ConnectedAccounts) {
					buddy = prpl.PurpleFindBuddy (account, name);
					if (buddy != 0)
						account_out = account;
					if (prpl.PurpleBuddyIsOnline (buddy))
						return true;
				}
			} catch { }
			return false;
		}
		
		public enum StatusType {
			Offline = 1,
			Available = 2,
			Unavailable = 3,
			Invisible = 4,
			Away = 5,
		}
		
		public static void PurpleSetAvailabilityStatus  (int kind, string message)
		{
			IPurpleObject prpl;
			
			prpl = GetPurpleObject ();
			int status;
			try {
				try {
					status = prpl.PurpleSavedstatusNew ("", (int) kind);
				}
				catch {
					status = prpl.PurpleSavedstatusNew ("", (uint) kind);
				}
				prpl.PurpleSavedstatusSetMessage (status, message);
				prpl.PurpleSavedstatusActivate (status);
			} catch (Exception e) {
				Log<Pidgin>.Error ("Could set Pidgin status: {0}", e.Message);
				Log<Pidgin>.Debug (e.StackTrace);
			}
		}
		
		public static void OpenConversationWithBuddy (string name, string message)
		{
			int account, conversation;
			IPurpleObject prpl;

			prpl = GetPurpleObject ();
			try {
				GetBuddyIsOnlineAndAccount (name, out account);
				if (account == -1)
					throw new ArgumentException ();
				try {
					conversation = prpl.PurpleConversationNew ((int) 1, account, name);
				}
				catch {
					conversation = prpl.PurpleConversationNew ((uint) 1, account, name);
				}
				if (!string.IsNullOrEmpty (message)) {
					int im = prpl.PurpleConversationGetImData (conversation);
					prpl.PurpleConvImSend (im, message);
				}
				prpl.PurpleConversationPresent (conversation);
			} catch (Exception e) {
				Log<Pidgin>.Error ("Could not create new Pidgin conversation: {0}", e.Message);
				Log<Pidgin>.Debug (e.StackTrace);
			}
		}

		public static void OpenConversationWithBuddy (string name)
		{
			OpenConversationWithBuddy (name, "");
		}

		public static bool InstanceIsRunning
		{
			get {
				Process pidof;
				ProcessStartInfo pidofInfo = new ProcessStartInfo ("pidof", "pidgin");
				pidofInfo.UseShellExecute = false;
				pidofInfo.RedirectStandardError = true;
				pidofInfo.RedirectStandardOutput = true;
								
				try {
					// Use pidof command to look for pidgin process. Exit
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

		public static void StartIfNeccessary ()
		{
			if (!InstanceIsRunning) {
				Process.Start ("pidgin");
				System.Threading.Thread.Sleep (4 * 1000);
			}
		}
	}
}

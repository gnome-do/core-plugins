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
using System.Diagnostics;
using System.Collections.Generic;

using NDesk.DBus;
using org.freedesktop.DBus;

using Do.Platform;

namespace PidginPlugin
{

	public class Pidgin
	{

		const string PurpleObjectPath = "/im/pidgin/purple/PurpleObject";
		const string PurpleServiceBusName = "im.pidgin.purple.PurpleService";

		[Interface ("im.pidgin.purple.PurpleInterface")]
		public interface IPurpleObject
		{
			string PurpleUserDir ();
			
			int[] PurpleAccountsGetAll ();
			int [] PurpleAccountsGetAllActive ();
			bool PurpleBuddyIsOnline (int buddy);
			int PurpleBuddyGetAccount (int buddy);
			string PurpleAccountGetAlias (int account);
			bool PurpleAccountIsConnected (int account);
			string PurpleBuddyGetServerAlias (int buddy);
			string PurpleAccountGetUsername (int account);
			int PurpleFindBuddy (int account, string name);
			string PurpleAccountGetProtocolName (int account);
			int PurpleAccountsFindConnected (string account, string proto);
			void PurpleAccountSetEnabled (int account, string ui, int value);

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
			void PurpleAccountSetEnabled (int account, string ui, uint value);
			
			#endregion
		}

		public static string ChatIcon 
		{
			get { return "internet-group-chat.svg@" + typeof (Pidgin).Assembly.FullName; }
		}
		
		public static string PurpleUserDir
		{
			get { return (GetPurpleObject () as IPurpleObject).PurpleUserDir (); }
		}

		public static string GetProtocolIcon (string proto)
		{
			string icon = null;

			proto = proto.ToLower ();
			string[] parts = proto.Split ('-');
			
			if (parts.Length >= 2) {
				if (parts[0] == "prpl")
					proto = parts[1];
				icon = Path.Combine ("/usr/share/pixmaps/pidgin/protocols/48", proto + ".png");
			}
			return File.Exists (icon) ? icon : Pidgin.ChatIcon;
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

		private static int [] ConnectedAccounts {
			get {
				List<int> connected;
				IPurpleObject prpl;

				prpl = GetPurpleObject ();
				connected = new List<int> ();
				try {
					foreach (int account in prpl.PurpleAccountsGetAllActive ()) {
						if (prpl.PurpleAccountIsConnected (account))
							connected.Add (account);
					}
				} catch { }
				return connected.ToArray ();
			}
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
				if (buddy == 0) continue;
				alias = prpl.PurpleBuddyGetServerAlias (buddy);
				return (string.IsNullOrEmpty (alias)) ? null : alias;
			}
			return null;
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
			try {
				foreach (int account in ConnectedAccounts) {
					buddy = prpl.PurpleFindBuddy (account, name);
					if (prpl.PurpleBuddyIsOnline (buddy)) {
						account_out = account;
						return true;
					}
				}
			} catch { }
			account_out = -1;
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
				Console.WriteLine ("in here, message: {0}", message);
				prpl.PurpleSavedstatusSetMessage (status, message);
				prpl.PurpleSavedstatusActivate (status);
			} catch (Exception e) {
				Console.WriteLine (e.ToString ());
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
					account = prpl.PurpleAccountsFindConnected ("", "");
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
				pidofInfo.UseShellExecute = true;
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

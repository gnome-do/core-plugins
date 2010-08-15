//  EmpathyPlugin.cs
//  
//  Author:
//       Xavier Calland <xavier.calland@gmail.com>
//  
//  Copyright (c) 2010 
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using NDesk.DBus;
using org.freedesktop.DBus;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AccountManagerQuery;
using Do.Universe;
using Telepathy;

namespace EmpathyPlugin
{

	public class EmpathyPlugin
	{
 		public const string ACCOUNTMANAGER_IFACE = "org.freedesktop.Telepathy.AccountManager";
		public const string ACCOUNTMANAGER_PATH = "/org/freedesktop/Telepathy/AccountManager";
		
 		public const string CHANNELDISPATCHER_IFACE = "org.freedesktop.Telepathy.ChannelDispatcher";
		public const string CHANNELDISPATCHER_PATH = "/org/freedesktop/Telepathy/ChannelDispatcher";
		
		public const string ACCOUNT_IFACE = "org.freedesktop.Telepathy.Account";
		
		public const string CONNMANAGER_GABBLE_IFACE = "org.freedesktop.Telepathy.ConnectionManager.gabble";
        public const string CONNMANAGER_GABBLE_PATH = "/org/freedesktop/Telepathy/ConnectionManager/gabble";
		
		public const string CONNECTION_IFACE = "org.freedesktop.Telepathy.Connection";
        public const string REQUESTS_IFACE = "org.freedesktop.Telepathy.Connection.Interface.Requests";
		
		public const string CHANNEL_TYPE = "org.freedesktop.Telepathy.Channel.ChannelType";
		public const string CHANNEL_TYPE_TEXT = "org.freedesktop.Telepathy.Channel.Type.Text";
		
        public const string CHANNEL_IFACE = "org.freedesktop.Telepathy.Channel";
		public const string CHANNEL_GROUP_IFACE = "org.freedesktop.Telepathy.Channel.Interface.Group";
		public const string CHANNEL_LIST_IFACE = "org.freedesktop.Telepathy.Channel.Interface.List";
		
		public const string CONNECTION_CONTACT_IFACE = "org.freedesktop.Telepathy.Connection.Interface.Contacts";
		
		public const string CONTACT_PROP_ID = "org.freedesktop.Telepathy.Connection/contact-id";
		public const string CONTACT_PROP_ALIAS = "org.freedesktop.Telepathy.Connection.Interface.Aliasing/alias";
		
		public const string CHANNEL_TARGETHANDLETYPE = "org.freedesktop.Telepathy.Channel.TargetHandleType";
		public const string CHANNEL_TARGETHANDLE = "org.freedesktop.Telepathy.Channel.TargetHandle";
        public const string CHANNEL_TYPE_CONTACTLIST = "org.freedesktop.Telepathy.Channel.Type.ContactList";
        
		public const string CONNECTION_CAPABILITIES_IFACE = "org.freedesktop.Telepathy.Connection.Interface.Capabilities";
		
        public const string DBUS_PROPERTIES = "org.freedesktop.DBus.Properties";
		
		public const string AVATAR_PATH = ".cache/telepathy/avatars";
		
		public const string PROTO_ICON_PATH = "/usr/share/empathy/icons/hicolor/48x48/apps";
		
		public const string PRESETS_STATUS_PLACE = ".config/Empathy/status-presets.xml";
		
		public static string ChatIcon
		{
			get { return "empathy"; }
		}
		
		public static bool IsTelepathy (Item item) 
		{
			return item.Equals (Do.Platform.Services.UniverseFactory.MaybeApplicationItemFromCommand ("empathy"));
		}
		
		public static List<Account> ConnectedAccounts {
			get {
				List<Account> res = new List<Account>();
				IAccountManagerQuery iAccountManagerQueryBus = Bus.Session.GetObject<IAccountManagerQuery> (ACCOUNTMANAGER_IFACE, new ObjectPath (ACCOUNTMANAGER_PATH));;
				ObjectPath[] accountPathArray = iAccountManagerQueryBus.FindAccounts (new Dictionary<string, object> ());
				
				foreach(ObjectPath accountPath in accountPathArray) {
					Account account = new Account(accountPath);
					if(account.IsConnected()) {
						res.Add(account);
					}
				}
				return res;
			}
		}
		
		public static List<Account> GetAllAccounts {
			get {
				List<Account> res = new List<Account>();
				IAccountManagerQuery iAccountManagerQueryBus = Bus.Session.GetObject<IAccountManagerQuery> (ACCOUNTMANAGER_IFACE, new ObjectPath (ACCOUNTMANAGER_PATH));;
				ObjectPath[] accountPathArray = iAccountManagerQueryBus.FindAccounts (new Dictionary<string, object> ());
				
				foreach(ObjectPath accountPath in accountPathArray) {
					Account account = new Account(accountPath);
					
					res.Add(account);
				}
				return res;
			}
		}
		
		public static List<Contact> GetAllContacts {
			get {
				List<Contact> res = new List<Contact>();
				foreach(Account account in ConnectedAccounts) {
					
					foreach(Contact contact in account.FindContact()) {
						res.Add(contact);
					}
				}
				return res;
			}
		}
		
		public static bool InstanceIsRunning
		{
			get {
				Process pidof;
				ProcessStartInfo pidofInfo = new ProcessStartInfo ("pidof", "empathy");
				pidofInfo.UseShellExecute = false;
				pidofInfo.RedirectStandardError = true;
				pidofInfo.RedirectStandardOutput = true;
								
				try {
					// Use pidof command to look for empathy process. Exit
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
		
		public static bool BuddyIsOnline (string contactName)
		{
			Account account;
			return GetBuddyIsOnlineAndAccount (contactName, out account);   
		}
		
		public static bool GetBuddyIsOnlineAndAccount (string contactName, out Account account_out)
		{
			account_out = null;
			
			try {
				foreach (Account account in ConnectedAccounts) {
					if (account.HasContact(contactName) )
					{
						account_out = account;
						return true;
					}
				}
			} catch (Exception e) { 
					Console.WriteLine ("Could not get Empathy contacts: {0}", e.Message);
					Console.WriteLine (e.StackTrace);
			}
			
			return false;
		}
		
		
		public static string GetProtocolIcon (string proto)
		{
			string icon = null;
			proto = proto.ToLower ();

			icon = Path.Combine (PROTO_ICON_PATH, proto + ".png");
			return File.Exists (icon) ? icon : EmpathyPlugin.ChatIcon;
		}
		
		public static void OpenConversationWithBuddy(string contactId, string message) 
		{
			Account account;
			if ( GetBuddyIsOnlineAndAccount(contactId, out account) ) 
			{
				account.OpenConversationWithBuddy(contactId, message);
			}
		}

		public static void OpenConversationWithBuddy(string contactId) 
		{
			OpenConversationWithBuddy(contactId, null);
		}
		
		public static void SetAvailabilityStatus(ConnectionPresenceType status, string message)
		{
			foreach (Account account in ConnectedAccounts) {
				account.SetStatus(status, message);
			} 
		}
	}
}



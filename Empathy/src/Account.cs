//  Account.cs
//
//  Author:
//       Xavier Calland <xavier.calland@gmail.com>
//
//  Copyright © 2010
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
using AccountManagerQuery;
using Telepathy;

namespace EmpathyPlugin
{
	public enum Criteria : uint
	{
		Channel = 0,
		ChannelType = 1,
		HandleType = 2,
		Handle = 3
	}

	public class Account
	{
		public ObjectPath accountPath {get; private set;}
		public IAccount iAccount {get; private set;}
		public Properties iAccountProp {get; private set;}

		public ObjectPath connectionPath {get; private set;}
		public string connectionBusIFace {get; private set;}
		public IConnection iConnection {get; private set;}

		private List<ConnectionPresenceType> AvailablePresencesType = new List<ConnectionPresenceType>( new ConnectionPresenceType[] {
			ConnectionPresenceType.Available,
			ConnectionPresenceType.Away,
			ConnectionPresenceType.Error,
			ConnectionPresenceType.Busy,
			ConnectionPresenceType.ExtendedAway,
			ConnectionPresenceType.Hidden,
			ConnectionPresenceType.Unknown,
			ConnectionPresenceType.Offline
		});

		public string name;
		public string proto;
		public string cm;

		public Account (ObjectPath accountPath)
		{
			this.accountPath = accountPath;

			this.iAccount = Bus.Session.GetObject<IAccount> (EmpathyPlugin.ACCOUNTMANAGER_IFACE, accountPath);
			this.iAccountProp = Bus.Session.GetObject<Properties> (EmpathyPlugin.ACCOUNTMANAGER_IFACE, accountPath);
			ConnectionStatus connectionStatus = (ConnectionStatus)this.iAccountProp.Get (EmpathyPlugin.ACCOUNT_IFACE, "ConnectionStatus");

			string[] tabStr = accountPath.ToString().Split("/".ToCharArray());
			int length = tabStr.Length;
			this.proto = tabStr[length - 2];
			this.cm = tabStr[length - 3];

			this.connectionPath = (ObjectPath)this.iAccountProp.Get (EmpathyPlugin.ACCOUNT_IFACE, "Connection");
			this.connectionBusIFace = connectionPath.ToString ().Replace ("/", ".").Substring (1);
			if (connectionStatus == ConnectionStatus.Connected) {
				this.iConnection = Bus.Session.GetObject<IConnection> (connectionBusIFace, connectionPath);
			} else {
				this.iConnection = null;
			}

//			 this.iAccount.Nickname
//				this.name = (string)this.iAccountProp.Get (TelepathyPlugin.ACCOUNT_IFACE, "Nickname");
//			 this.iAccount.DisplayName
			this.name = (string)this.iAccountProp.Get (EmpathyPlugin.ACCOUNT_IFACE, "DisplayName");
		}

		public bool IsConnected ()
		{
			ConnectionStatus connectionStatus = (ConnectionStatus)this.iAccountProp.Get (EmpathyPlugin.ACCOUNT_IFACE, "ConnectionStatus");
			return connectionStatus == ConnectionStatus.Connected;
		}

		public override string ToString ()
		{
			return this.name + " [" + this.proto + "] : " + IsConnected ();
		}

		public bool HasContact (string @name)
		{
			return this.FindContact(name) != null;
		}

		public Contact FindContact (string @name)
		{
			foreach (ChannelInfo channelInfo in this.iConnection.ListChannels ()) {

				Properties contactGroupProperties = Bus.Session.GetObject<Properties> (this.connectionBusIFace, channelInfo.Channel);

				if (channelInfo.ChannelType == EmpathyPlugin.CHANNEL_TYPE_CONTACTLIST &&
				    (channelInfo.HandleType == HandleType.List)) {
					string[] strTab = channelInfo.Channel.ToString().Split("/".ToCharArray());
					if(strTab[strTab.Length - 1] == "subscribe") 
					{
						foreach (UInt32 i in (UInt32[])contactGroupProperties.Get (EmpathyPlugin.CHANNEL_GROUP_IFACE, "Members")) {
							Contact contact = new Contact(i, this);
							if (contact.ContactId == name) {
								if(AvailablePresencesType.Contains(contact.SimplePresence.Type)) {
									return new Contact(i, this);
								}
							}
						}
					}
				}
			}
			return null;
		}

		public void SetStatus (ConnectionPresenceType status, string message)
		{
			ISimplePresence simplePresence = Bus.Session.GetObject<ISimplePresence> (connectionBusIFace, connectionPath);
//			Properties simplePresenceProperties = Bus.Session.GetObject<Properties> (connectionBusIFace, connectionPath);
//			IDictionary<string, object> lstStatus = (IDictionary<string, object>) simplePresenceProperties.Get("org.freedesktop.Telepathy.Connection.Interface.SimplePresence", "Statuses");

			List<BaseStatus> possibleStatusLst = EmpathyStatus.GetStatusList(status);

			// TODO: utiliser SimplePresence au lieu de Presence
			IPresence presence = Bus.Session.GetObject<IPresence> (connectionBusIFace, connectionPath);
			IDictionary<string, StatusSpec> lstStatus = presence.GetStatuses();

			foreach(BaseStatus oneStatus in possibleStatusLst)
			{
				StatusSpec statusSpec;
				if(lstStatus.TryGetValue(oneStatus.ToString(), out statusSpec) )
				{
					if (statusSpec.MaySetOnSelf)
					{
						simplePresence.SetPresence(oneStatus.ToString(), message);
						return;
					}
				}
			}
		}

		public IEnumerable<Contact> FindContact ()
		{
			List<Contact> res = new List<Contact>();
			if(! this.IsConnected()) {
				return res;
			}
			foreach (ChannelInfo channelInfo in iConnection.ListChannels ()) {
				Properties contactGroupProperties = Bus.Session.GetObject<Properties> (this.connectionBusIFace, channelInfo.Channel);

				if (channelInfo.ChannelType == EmpathyPlugin.CHANNEL_TYPE_CONTACTLIST &&
				    (channelInfo.HandleType == HandleType.List)) {
					string[] strTab = channelInfo.Channel.ToString().Split("/".ToCharArray());
					if(strTab[strTab.Length - 1] == "subscribe")
					{
						foreach (UInt32 i in (UInt32[])contactGroupProperties.Get (EmpathyPlugin.CHANNEL_GROUP_IFACE, "Members"))
						{
							Contact contact = new Contact(i, this);
							res.Add(contact);
						}
					}
				}
			}
			return res;
		}

		public void OpenConversationWithBuddy(string contactId, string message)
		{
			Contact contact = FindContact(contactId);

			ObjectPath opath = new ObjectPath (EmpathyPlugin.CHANNELDISPATCHER_PATH);
			IChannelDispatcher iChannelDispatcherBus = Bus.Session.GetObject<IChannelDispatcher> (EmpathyPlugin.CHANNELDISPATCHER_IFACE, opath);

			Dictionary<string, object> channelRequestParameters = new Dictionary<string, object> ();
			channelRequestParameters.Add (EmpathyPlugin.CHANNEL_TYPE, EmpathyPlugin.CHANNEL_TYPE_TEXT);
			channelRequestParameters.Add (EmpathyPlugin.CHANNEL_TARGETHANDLETYPE, HandleType.Contact);
			channelRequestParameters.Add (EmpathyPlugin.CHANNEL_TARGETHANDLE, contact.ContactUInt);

			ObjectPath messageChannelPath = iChannelDispatcherBus.EnsureChannel (this.accountPath, channelRequestParameters, DateTime.Now.Ticks, "org.freedesktop.Telepathy.Client.Empathy");

			IChannelRequest iChannelRequest = Bus.Session.GetObject<IChannelRequest> (EmpathyPlugin.ACCOUNTMANAGER_IFACE, messageChannelPath);
			iChannelRequest.Proceed();

			if(message == null)
			{
				return;
			}

			foreach (ChannelInfo channelInfo in this.iConnection.ListChannels ())
			{
				if (channelInfo.ChannelType == EmpathyPlugin.CHANNEL_TYPE_TEXT &&
				    channelInfo.HandleType == HandleType.Contact && channelInfo.Handle == contact.ContactUInt)
				{
					IMessages messageChannel = Bus.Session.GetObject<IMessages> (this.connectionBusIFace, channelInfo.Channel);
					IDictionary<string, object> header = new Dictionary<string, object>() {
					    {"message-type", ChannelTextMessageType.Notice}
					};
					IDictionary<string, object> alternative = new Dictionary<string, object>() {
						{"alternative", "main"},
					    {"content-type", "text/plain"},
					    {"content", message}
					};
					messageChannel.SendMessage(new IDictionary<string, object>[] { header, alternative}, MessageSendingFlags.None);
				}
			}
		}

		public void EnableAccount()
		{
			iAccountProp.Set("org.freedesktop.Telepathy.Account", "Enabled", true);
		}

		public void DisableAccount()
		{
			iAccountProp.Set("org.freedesktop.Telepathy.Account", "Enabled", false);
		}

		public string GetIconName()
		{
			return (string) iAccountProp.Get("org.freedesktop.Telepathy.Account", "Icon");
		}
	}
}

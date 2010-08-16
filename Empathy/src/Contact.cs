//  Contact.cs
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
using System.Text.RegularExpressions;
using AccountManagerQuery;
using Telepathy;

namespace EmpathyPlugin
{
	public class Contact
	{

		public UInt32 ContactUInt {get; private set;}
		public Account Account  {get; private set;}
		public IDictionary<string, object> Attributes {get; private set;}
		public SimplePresence SimplePresence ;
		public string Alias {get {return (string)GetPropertyValue(EmpathyPlugin.CONTACT_PROP_ALIAS);} }
		public string ContactId {get {return (string)GetPropertyValue(EmpathyPlugin.CONTACT_PROP_ID);} }
		public string AvatarToken {get; private set;}
		
		public Contact (UInt32 contactUInt, Account account)
		{
			ContactUInt = contactUInt;
			Account = account;
			
			IContacts contacts = Bus.Session.GetObject<IContacts> (Account.connectionBusIFace, Account.connectionPath);
			ISimplePresence presence = Bus.Session.GetObject<ISimplePresence> (Account.connectionBusIFace, Account.connectionPath);
			IAvatars avatars = Bus.Session.GetObject<IAvatars> (Account.connectionBusIFace, Account.connectionPath);
			Properties connectionProperties = Bus.Session.GetObject<Properties> (Account.connectionBusIFace, Account.connectionPath);
			
			// TODO: géré les protocols sans IAvatars
			IDictionary<uint, string> tokens = avatars.GetKnownAvatarTokens(new uint[] { ContactUInt });
			string strTmp = "";
			if(tokens.TryGetValue(contactUInt, out strTmp) && strTmp.Length > 0)
			{
				// ajout du préfix "_3" si le premier token commence par un nombre
				if(Regex.IsMatch(strTmp.Substring(0,1), "[0-9]"))
				{
					strTmp = "_3"+strTmp;
				}
			}
			AvatarToken = strTmp;
			SimplePresence sTmp;
			presence.GetPresences (new uint[] { ContactUInt }).TryGetValue (ContactUInt, out sTmp);
			SimplePresence = sTmp;
			
			IDictionary<string, object> tmp;
						
			string[] itf = (string[])connectionProperties.Get (EmpathyPlugin.CONNECTION_CONTACT_IFACE, "ContactAttributeInterfaces");
			// FIXME: cette interface fait planter DBus avec MSN, on remet un interface bidon
			int idx = Array.IndexOf(itf, EmpathyPlugin.CONNECTION_CAPABILITIES_IFACE);
			if(idx != -1) 
			{
				itf[idx] = EmpathyPlugin.CONNECTION_IFACE;
			}
			
			IDictionary<uint, IDictionary<string, object>> allAttributes = 
				contacts.GetContactAttributes (new uint[] { ContactUInt }, itf, false);
			allAttributes.TryGetValue (ContactUInt, out tmp);
			Attributes = tmp;
		}
		
		private object GetPropertyValue(string propName) 
		{
			object res;
			Attributes.TryGetValue(propName, out res);
			return res;
		}
	}
}


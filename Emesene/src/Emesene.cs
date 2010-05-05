/* Emesene.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
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
using NDesk.DBus;
using org.freedesktop.DBus;
using System.Collections.Generic;
using Do.Universe;
using Do.Platform;

namespace Emesene
{
	public class Emesene
	{
		
		const string emeseneObjectPath = "/org/emesene/dbus";
		const string emeseneServiceBusName = "org.emesene.dbus";
		public static List<Item> status;
		
		public static string getAvatarPathForUser()
		{
			return Emesene.getPathForUser()+"avatars";
		}
		
		public static string getCachePathForUser()
		{
			return Emesene.getPathForUser()+"cache";
		}
		
		private static string getPathForUser(){
			string user = Emesene.getCurrentEmeseneUser();
			user = user.Replace(".","_");
			user = Environment.GetFolderPath(Environment.SpecialFolder.Personal)+
							"/.config/emesene1.0/"+user.Replace("@","_")+
								"/";
			return user;
		}
		
		[Interface ("org.emesene.dbus")]
		public interface EmeseneInterface
		{
			void open_conversation(string email, bool weStarted);
			void open_conversation(string email);
			string get_last_display_picture(string account, bool cache);
			string get_user_account();
			string set_nick(string nick);
			bool set_avatar(string path);
			string set_psm(string psm);
			string set_status(string status);
			void get_conversation_history(string email);
			void get_avatar_history(string email);
			string get_email_page();
		}
		
		static Emesene()
		{
			//Populate emesene status list			
			status = new List<Item>();
			status.Add(new EmeseneStatusItem("online", "online status", "NLN"));
			status.Add(new EmeseneStatusItem("away", "away status", "AWY"));
			status.Add(new EmeseneStatusItem("brb", "brb status", "BRB"));
			status.Add(new EmeseneStatusItem("busy", "busy status", "BSY"));
			status.Add(new EmeseneStatusItem("idle", "idle status", "IDL"));
			status.Add(new EmeseneStatusItem("lunch", "lunch status", "LUN"));
			status.Add(new EmeseneStatusItem("invisible", "invisible status", "HDN"));
			status.Add(new EmeseneStatusItem("phone", "phone status", "PHN"));
			status.Add(new EmeseneStatusItem("offline", "offline status", "FLN"));
			
		}

		public static EmeseneInterface getEmeseneObject()
		{
			try 
			{
				return Bus.Session.GetObject<EmeseneInterface>
					(emeseneServiceBusName, new ObjectPath (emeseneObjectPath));
			}
			catch(Exception e)
			{
                Log<Emesene>.Error ("Emesene > Error getting EmeseneObject - {0}", e.Message);
				Log<Emesene>.Debug (e.StackTrace);
				return null;
			}
		}
		
		
		public static bool checkForEmesene()
		{
			try
			{
				EmeseneInterface em = Bus.Session.GetObject<EmeseneInterface>
					(emeseneServiceBusName, new ObjectPath (emeseneObjectPath));
				em.get_user_account();
				return true;
			} 
			catch(Exception e) 
			{
				return false;
			}
		}
		
		public static void openChatWith(string mail)
		{
			EmeseneInterface em = Emesene.getEmeseneObject();
			try{
			    em.open_conversation(mail, true);
			}catch(Exception e){
			    //User is using older emesene
			    //em.open_conversation(mail);
			    Log<Emesene>.Debug ("Old version of emesene");
		        Log<Emesene>.Debug (e.StackTrace);
			}
			
		}
		
		public static string getCurrentEmeseneUser()
		{
			EmeseneInterface em = Emesene.getEmeseneObject();
			return em.get_user_account();
		}
		
		public static string get_last_display_picture(string account, bool cache)
		{
			EmeseneInterface em = Emesene.getEmeseneObject();
			return em.get_last_display_picture(account, cache);
		}
		
		public static string set_nick(string nick)
		{
			EmeseneInterface em = Emesene.getEmeseneObject();
			return em.set_nick(nick);
		}
		
		public static bool set_avatar(string path)
		{
			EmeseneInterface em = Emesene.getEmeseneObject();
			return em.set_avatar(path);
		}
		
		public static string set_psm(string psm)
		{
			EmeseneInterface em = Emesene.getEmeseneObject();
			return em.set_psm(psm);
		}
		
		public static string set_status(string status)
		{
			EmeseneInterface em = Emesene.getEmeseneObject();
			return em.set_status(status);
		}
		
		public static void get_conversation_history(string email)
		{
			EmeseneInterface em = Emesene.getEmeseneObject();
			em.get_conversation_history(email);
		}
		
		public static void get_avatar_history(string email)
		{
			EmeseneInterface em = Emesene.getEmeseneObject();
			em.get_avatar_history(email);
		}
		
		public static string get_email_page()
		{
			EmeseneInterface em = Emesene.getEmeseneObject();
			return em.get_email_page();
		}
		
	}
}

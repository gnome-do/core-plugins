// Emesene.cs created with MonoDevelop
// User: luis at 11:53 a 05/07/2008

using System;
using NDesk.DBus;
using org.freedesktop.DBus;
using System.Collections.Generic;
using Do.Universe;

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
				System.Console.WriteLine("Exception: "+e.Message);
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
			em.open_conversation(mail, true);
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

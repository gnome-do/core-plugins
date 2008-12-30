// SkypeMain.cs
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, see <http://www.gnu.org/licenses/> or 
// write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, 
// Boston, MA 02111-1307 USA
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using NDesk.DBus;
using org.freedesktop.DBus;

using Do.Universe;


namespace Skype {

  ////////////////////////////////////////////////////
  // Skype API

  public class UserStatus {
    private string name,code,icon;
    public UserStatus(string n,string c,string i) {
      name = n; code = c; icon = i;
    }
    public UserStatus(string n,string c) {
      name = n; code = c; icon = "skype";
    }
    public string Name {
      get { return name; }
    }
    public string Code {
      get { return code; }
    }
    public string Icon {
      get { return icon; }
    }

  } // class UserStatus

  public class User {
    private string handle,fullname,mood_text,displayName;
    public User(string h,string f,string m,string d) {
      handle = h; fullname = f;
      mood_text = m; displayName = d;
    }
    public string Handle {
      get { return handle; }
    }
    public string DisplayName {
      get { return displayName; }
    }
    public string Fullname {
      get { return fullname; }
    }
    public string MoodText {
      get { return mood_text; }
    }
    public string Icon {
      get { return SkypeAPI.RES("Contact_128x128.png"); }
    }

  } // class Usre

  public class Chat {
    private string chatId,title;
    protected string icon;
    private DateTime time;
    public Chat(string id,string t,DateTime dt) {
      chatId = id; title = t; time = dt;
      icon = SkypeAPI.RES("Message_128x128.png");
    }
    public string ChatID {
      get { return chatId; }
    }
    public string Title {
      get { return title; }
    }
    public string Icon {
      get { return icon; }
    }
    public string TimeAsString {
      get { return time.ToString();}
    }
    public DateTime Time {
      get { return time; }
    }
  } // class Chat

  public class MissedChat : Chat {
    public MissedChat(string id,string t,DateTime d) : base(id,t,d) {
      icon = SkypeAPI.RES("MessageMissed_128x128.png");
    }
  } // class MissedChat

  ////////////////////////////////////////////////////
  // Skype API

  public class SkypeAPI {

    private static SkypeAPI skypeInstance = null;

    public static SkypeAPI Instance {
      get {
        if (skypeInstance == null) {
          skypeInstance = new SkypeAPI();
        }
        return skypeInstance;
      }
    }

    private static string ICON_BASE = "";
    private static string ICON_POST = "@" + typeof (SkypeAPI).Assembly.FullName;
    public static string RES (string name) {
		return ICON_BASE+name+ICON_POST;
    }
    
    public static readonly UserStatus[] STATUSES = {
      new UserStatus("Unknown","UNKNOWN",RES("StatusPending_128x128.png")),
      new UserStatus("Online","ONLINE",RES("StatusOnline_128x128.png")),
      new UserStatus("Offline","OFFLINE",RES("StatusOffline_128x128.png")),
      new UserStatus("Skype me","SKYPEME",RES("StatusSkypeMe_128x128.png")),
      new UserStatus("Away","AWAY",RES("StatusAway_128x128.png")),
      new UserStatus("Not available","NA",RES("StatusNotAvailable_128x128.png")),
      new UserStatus("Do not disturb","DND",RES("StatusDoNotDisturb_128x128.png")),
      new UserStatus("Invisible","INVISIBLE",RES("StatusInvisible_128x128.png")),
      new UserStatus("Logged out","LOGGEDOUT",RES("StatusOffline_128x128.png")),
    };

    public static readonly UserStatus STATUS_ERROR = new UserStatus("Error","ERROR");

    private SkypeDBus skypeDBus = new SkypeDBus();

    public UserStatus GetUserStatus() {
      string ret = skypeDBus.Send("GET USERSTATUS");
      if (ret == null) return STATUS_ERROR;
      for(int i=0;i<STATUSES.Length;i++) {
        if (STATUSES[i].Code == ret) {
          return STATUSES[i];
        }
      }
      return STATUS_ERROR;
    }

    private static readonly string[] EMPTY = new string[0];

    private static string[] Trims(string[] ids) {
      for(int i=0;i<ids.Length;i++) {
        ids[i] = ids[i].Trim();
      }
      return ids;
    }

    public string[] GetGroups() {
      string gs = skypeDBus.Send("SEARCH GROUPS ALL");
      if (gs == null || !gs.StartsWith("GROUPS ")) return EMPTY;
      
      return Trims(gs.Substring(7).Split(new char[]{','}));
    }

    public string[] GetUsers(string groupId) {
      string tag = "GROUP "+groupId+" USERS ";
      string us = skypeDBus.Send("GET "+tag);
      if (us == null || !us.StartsWith(tag)) return EMPTY;

      return Trims(us.Substring(tag.Length).Split(new char[]{','}));
    }

    private string GetObjectAttr(string id,string type,string attr) {
      string ret = skypeDBus.Send("GET "+type+" "+id+" "+attr);
      if (ret == null || ret.StartsWith("ERROR")) {
        return null;
      }
      string tag = type+" "+id+" "+attr+" ";
      string data = ret.Substring(tag.Length).Trim();
      return data;
    }

    private string GetUserAttr(string id,string attr) {
      return GetObjectAttr(id,"USER",attr);
    }

    private string GetChatAttr(string id,string attr) {
      return GetObjectAttr(id,"CHAT",attr);
    }

    private string GetChatMessageAttr(string id,string attr) {
      return GetObjectAttr(id,"CHATMESSAGE",attr);
    }

    private string Regex1(string regex,string src) {
      Regex r = new Regex(regex);
      Match m = r.Match(src);
      return m.Groups[1].Value;
    }

    public User GetUserObject(string handle) {
      return new User(handle, GetUserAttr(handle,"FULLNAME"),
                      GetUserAttr(handle,"MOOD_TEXT"),
                      GetUserAttr(handle,"DISPLAYNAME"));
    }

    public User[] GetAllContactUsers() {
      string[] groups = GetGroups();
      if (groups == null || groups.Length == 0) return null;
      string friendGroup = groups[0];
      foreach(string i in groups) {
        if ("ALL_FRIENDS" == GetObjectAttr(i,"GROUP","TYPE")) {
          friendGroup = i;
          break;
        }
      }
      string[] users = GetUsers(friendGroup);
      User[] ret = new User[users.Length];
      for(int i=0;i<users.Length;i++) {
        ret[i] = GetUserObject(users[i]);
      }
      return ret;
    }

    public string StartChat(string userHandle) {
      string ret = skypeDBus.Send("CHAT CREATE "+userHandle);
      if (ret == null || !ret.StartsWith("CHAT ")) return null;
      string cid = Regex1(@"CHAT (.+) STATUS DIALOG",ret);
      return OpenChat(cid);
    }

    public string OpenChat(string chatId) {
      string ret = skypeDBus.Send("OPEN CHAT "+chatId);
      if (ret == null || !ret.StartsWith("OK")) return null;
      return chatId;
    }

    public string StartCall(string userHandle) {
      string ret = skypeDBus.Send("CALL "+userHandle);
			//string ret = skypeDBus.Send("CALL +16265359429");
      if (ret == null || !ret.StartsWith("CALL")) return null;
      return Regex1(@"CALL ([^ ]+) ", ret);
    }

    public Chat[] GetRecentChats() {
      string ret = skypeDBus.Send("SEARCH RECENTCHATS");
      if (ret == null || !ret.StartsWith("CHATS ")) return null;
      string[] ids = Trims( ret.Substring(5).Split(new char[]{','}));
      Console.WriteLine("Recent Chat ["+ids.Length+"]: "+ret);
      return GetChatObjects(ids);
    }

    public Chat[] GetBookmarkedChats() {
      string ret = skypeDBus.Send("SEARCH BOOKMARKEDCHATS");
      if (ret == null || !ret.StartsWith("CHATS ")) return null;
      string[] ids = Trims( ret.Substring(5).Split(new char[]{','}));
      Console.WriteLine("Bookmarked Chat ["+ids.Length+"]: "+ret);
      return GetChatObjects(ids);
    }

    public Chat[] GetMissedChats() {
      string ret = skypeDBus.Send("SEARCH MISSEDCHATS");
      if (ret == null || !ret.StartsWith("CHATS ")) return null;
      string[] ids = Trims( ret.Substring(5).Split(new char[]{','}));
      Console.WriteLine("Missed Chat ["+ids.Length+"]: "+ret);
      return GetChatObjects(ids,true);
    }

    public Chat[] GetActiveChats() {
      string ret = skypeDBus.Send("SEARCH ACTIVECHATS");
      if (ret == null || !ret.StartsWith("CHATS ")) return null;
      string[] ids = Trims( ret.Substring(5).Split(new char[]{','}));
      Console.WriteLine("Active Chat ["+ids.Length+"]: "+ret);
      return GetChatObjects(ids);
    }

    private Chat[] GetChatObjects(string[] ids) {
      return GetChatObjects(ids,false);
    }
    private Chat[] GetChatObjects(string[] ids,bool missed) {
      if (ids.Length == 1 && ids[0].Length == 0) return new Chat[0];
      Chat[] chats = new Chat[ids.Length];
      for(int i=0;i<ids.Length;i++) {
        DateTime d = GetLastChatTime(ids[i]);
        string n = GetChatAttr(ids[i],"FRIENDLYNAME");
        chats[i] = (missed) ? new MissedChat(ids[i],n,d) : new Chat(ids[i],n,d);
      }
      return chats;
    }

    private DateTime GetLastChatTime(string id) {
      string ret = GetChatAttr(id,"RECENTCHATMESSAGES");
      if (ret == null || ret.Length == 0) return new DateTime();
      string rid = ret.Split(new char[]{','})[0];
      string strtime = GetChatMessageAttr(rid,"TIMESTAMP");
      return new DateTime(1970, 1, 1).AddSeconds(long.Parse(strtime));
    }

    public string SetUserStatus(string code) {
      return skypeDBus.Send("SET USERSTATUS "+code);
    }

  }

  ////////////////////////////////////////////////////
  // DBus low level API

  [Interface("com.Skype.API")]
  public interface ISkype {
    string Invoke(string commandLine);
  }

  public class SkypeDBus {
    
    private const string OBJECT_PATH = "/com/Skype";
    private const string BUS_NAME = "com.Skype.API";
    private static ISkype Skype;

    static private ISkype FindInstance () {
      if (!Bus.Session.NameHasOwner (BUS_NAME)) {
        throw new Exception (String.Format("Name {0} has no owner", BUS_NAME));
      }
      ISkype skype = Bus.Session.GetObject<ISkype>(BUS_NAME, new ObjectPath(OBJECT_PATH));
      string ret = skype.Invoke("NAME GNOME-Do_Skype");
      if (ret != "OK") {
        throw new Exception("Skype did not return OK...");
      }
      ret = skype.Invoke("PROTOCOL 7");
      if (ret != "PROTOCOL 7") {
        throw new Exception("Skype did not accept protocol 5...");
      }
      return skype;
    }
    
    public string Send(string commandLine) {
      if (Skype == null) {
        try {
          Skype = FindInstance();
        } catch (Exception) {
          Console.Error.WriteLine("Could not locate Skype on D-Bus. Make sure Skype is running");
          Skype = null;
          return null;
        }
      }
      Console.WriteLine(">> "+commandLine);//debug
      try {
        string ret = Skype.Invoke(commandLine);
        if (ret.StartsWith("ERROR")) {
          Skype = null;//try to reconnect for the next command
          Console.WriteLine("((ERROR: reconnect...))");
        }
        Console.WriteLine("<< "+ret);//debug
        return ret;
      } catch (Exception) {
        Console.WriteLine("((Exception: reconnect...))");
        Skype = null;
      }
      return null;
    }
  }

  ////////////////////////////////////////////////////
  // Skype API : Skype DBus console

  public class DBusTest {
    public static void Main(string[] args) {
      repl();
    }
    private static void repl() {
      SkypeDBus skype = new SkypeDBus();
      Console.WriteLine("Connect.");
      while(true) {
        Console.WriteLine("> "+skype.Send(Console.ReadLine()));
      }
    }
  }

}

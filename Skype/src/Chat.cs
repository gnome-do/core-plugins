// Chat.cs
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
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Do.Universe;

namespace Skype {

  public abstract class AbstractChatItem : Item {
    private Chat chat;
    public AbstractChatItem(Chat s) {
      chat = s;
    }
    public override string Name {
      get { return chat.Title; }
    }
    public override string Description {
      get {
        return "Last Message: "+chat.TimeAsString;
      }
    }
    public override string Icon {
      get { return chat.Icon; }
    }
    public string ChatID {
      get { return chat.ChatID; }
    }
    public DateTime Time {
      get { return chat.Time; }
    }
  }

  public abstract class AbstractChatItemSource : ItemSource {
    protected List<Item> items;
    public AbstractChatItemSource () {
      items = new List<Item> ();
      UpdateItems ();
    }

    public override string Description {
      get { return ""; }
    }
    public override string Icon {
      get { return "skype"; }
    }
    public override IEnumerable<Item> Items {
      get { return items; }
    }
    public override IEnumerable<Item> ChildrenOfItem (Item item) {
      return null;
    }
    public override IEnumerable<Type> SupportedItemTypes {
      get { return new Type[] { typeof (AbstractChatItem), }; }
    }
 
  }

  public abstract class AbstractChatAction : Act {
    public override string Description {
      get { return ""; }
    }
    public override bool SupportsItem (Item item) {
      return true;
    }
    public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem) {
      return true;
    }
    public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems) {
      AbstractChatItem i = items.First () as AbstractChatItem;
      SkypeAPI.Instance.OpenChat(i.ChatID);
      return null;
    }
  }

  ////////////////////////////////////////////////////
  // Recent Chat Item, Action

  public class RecentChatItem : AbstractChatItem {
    public RecentChatItem(Chat s) : base(s) {}
  }

  public class RecentChatItemSource : AbstractChatItemSource {
    public override string Name {
      get { return "Source: Recent Chats"; }
    }
    public override void UpdateItems () {
      items.Clear();
      Chat[] cs = SkypeAPI.Instance.GetRecentChats();
      if (cs == null) return;
      foreach (Chat i in cs){
        items.Add(new RecentChatItem(i));
      }
    }
  }

  public class OpenRecentChatAction : AbstractChatAction {
    public OpenRecentChatAction() {}
    public override string Name {
      get { return "Open Recent Chat"; }
    }
    public override string Icon {
      get { return SkypeAPI.RES("CallList_128x128.png"); }
    }
    public override IEnumerable<Type> SupportedItemTypes {
      get { return new Type[] { typeof (RecentChatItem) }; }
    }
  }

  ////////////////////////////////////////////////////
  // Missed Item, Source and Action

  public class MissedChatItem : AbstractChatItem {
    public MissedChatItem(Chat s) : base(s) {}
  }

  public class MissedAllChat : Chat {
    public static string CHAT_ID = "AllMissedItems";
    public MissedAllChat() : base(CHAT_ID,"All missed chats",DateTime.Now) {
      icon = SkypeAPI.RES("MessageMultipleUsers_128x128.png");
    }
  }

  public class MissedChatItemSource : AbstractChatItemSource {
    public override string Name {
      get { return "Source: Missed Chats"; }
    }
    public override void UpdateItems () {
      items.Clear();
      items.Add(new MissedChatItem(new MissedAllChat()));
      Chat[] cs = SkypeAPI.Instance.GetMissedChats();
      if (cs == null || cs.Length == 0) return;
      foreach (Chat i in cs){
        items.Add(new MissedChatItem(i));
      }
    }
  }

  public class OpenMissedChatAction : AbstractChatAction {
    public OpenMissedChatAction() { }
    public override string Name {
      get { return "Open Missed Chat"; }
    }
    public override string Icon {
      get { return SkypeAPI.RES("Events_128x128.png"); }
    }
    public override IEnumerable<Type> SupportedItemTypes {
      get { return new Type[] { typeof (MissedChatItem) }; }
    }
    public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems) {
      AbstractChatItem i = items.First () as AbstractChatItem;
      if (i.ChatID == MissedAllChat.CHAT_ID) {
        Chat[] cs = SkypeAPI.Instance.GetMissedChats();
        foreach (Chat j in cs){
          SkypeAPI.Instance.OpenChat(j.ChatID);
        }
      } else {
        SkypeAPI.Instance.OpenChat(i.ChatID);
      }
      return null;
    }
  }

  ////////////////////////////////////////////////////
  // Bookmarked Item, Source and Action

  public class BookmarkedChatItem : AbstractChatItem {
    public BookmarkedChatItem(Chat s) : base(s) {}
  }

  public class BookmarkedChatItemSource : AbstractChatItemSource {
    public override string Name {
      get { return "Source: Bookmarked Chats"; }
    }
    public override void UpdateItems () {
      items.Clear();
      Chat[] cs = SkypeAPI.Instance.GetBookmarkedChats();
      if (cs == null) return;
      foreach (Chat i in cs){
        items.Add(new BookmarkedChatItem(i));
      }
    }
  }

  public class OpenBookmarkedChatAction : AbstractChatAction {
    public OpenBookmarkedChatAction() { }
    public override string Name {
      get { return "Open Bookmarked Chat"; }
    }
    public override string Icon {
      get { return SkypeAPI.RES("History_128x128.png"); }
    }
    public override IEnumerable<Type> SupportedItemTypes {
      get { return new Type[] { typeof (BookmarkedChatItem) }; }
    }
  }

}

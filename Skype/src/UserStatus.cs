// UserStatus.cs
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

  ////////////////////////////////////////////////////
  // Status Item, Source and Action

  public class UserStatusItem : Item {

    private UserStatus status;

    public UserStatusItem(UserStatus s) {
      status = s;
    }

    public override string Name {
      get { return status.Name; }
    }
    public override string Description {
      get { return "User Status"; }
    }
    public override string Icon {
      get { return status.Icon; }
    }
    public string Code {
      get { return status.Code; }
    }

  }

  public class UserStatusItemSource : ItemSource {

    List<Item> items;
                
    public UserStatusItemSource () {
      items = new List<Item> ();
      UpdateItems ();
    }
                
    public override string Name {
      get { return "Source: User Status"; }
    }
		
    public override string Description {
      get { return ""; }
    }
		
    public override string Icon {
      get { return "skype"; }
    }
                
    public override IEnumerable<Type> SupportedItemTypes {
      get {
        return new Type[] { typeof (UserStatusItem), };
      }
    }

    public override IEnumerable<Item> Items {
      get { return items; }
    }
		
    public override IEnumerable<Item> ChildrenOfItem (Item item) {
      yield break;
    }
		
    public override void UpdateItems () {
      items.Clear();
      foreach (UserStatus i in SkypeAPI.STATUSES){
        items.Add(new UserStatusItem(i));
      }
    }

  }

  public class UserStatusAction : Act {
    
    public UserStatusAction() {
    }

    public override string Name {
      get { return "Change User Status"; }
    }

    public override string Description {
      get { return ""; }
    }

    public override string Icon {
      get { return SkypeAPI.RES("Okay_128x128.png"); }
    }
    
    public override bool SupportsItem (Item item) {
      return true;
    }
    public override IEnumerable<Type> SupportedItemTypes {
      get {
        return new Type[] { typeof (UserStatusItem) };
      }
    }

    public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem) {
      return false;
    }

    public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems) {
      UserStatusItem i = items.First () as UserStatusItem;
      SkypeAPI.Instance.SetUserStatus(i.Code);
      yield break;
    }

  }

}

// User.cs
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
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

using Do.Universe;

namespace Skype {

  ////////////////////////////////////////////////////
  // Item Source and Action

  public class UserItemSource : ItemSource {

    List<Item> items;

    public UserItemSource () {
      items = new List<Item> ();
    }
                
    public override string Name {
      get { return "Skype Users"; }
    }
		
    public override string Description {
      get { return "Skype Users"; }
    }
		
    public override string Icon {
      get { return "skype"; }
    }
                
    public override IEnumerable<Type> SupportedItemTypes {
      get {
        return new Type[] { typeof (ContactItem), };
      }
    }

    public override IEnumerable<Item> Items {
      get { return items; }
    }
		
    public override IEnumerable<Item> ChildrenOfItem (Item item) {
      return null;
    }
		
    public override void UpdateItems () {
      items.Clear();
      try {
        User[] us = SkypeAPI.Instance.GetAllContactUsers();
        if (us == null) return;
        foreach (User i in us) {
			ContactItem c = null;
			if (!string.IsNullOrEmpty (i.Fullname))
				c = ContactItem.CreateWithName (i.Fullname);
			else if (!string.IsNullOrEmpty (i.DisplayName))
				c = ContactItem.CreateWithName (i.DisplayName);
			else
				c = ContactItem.CreateWithName (i.Handle);
			c ["skype.handle"] = i.Handle;
        	items.Add (c);
        }
      } catch (Exception e) {
        Console.WriteLine("Exception: User.cs :"+e);
      }
      Console.WriteLine("Users : "+items.Count);
    }

  }

  public class StartChatAction : Act {
    
    public StartChatAction() {
    }

    public override string Name {
      get { return "Start Skype Chat"; }
    }

    public override string Description {
      get { return "Start a new chat with a Skype contact."; }
    }

    public override string Icon {
      get { return SkypeAPI.RES("Message_128x128.png"); }
    }
    
    public override bool SupportsItem (Item item) {
     	return null != (item as ContactItem) ["skype.handle"];
    }
    
    public override IEnumerable<Type> SupportedItemTypes {
      get {
        return new Type[] { typeof (ContactItem) };
      }
    }

    public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem) {
      return false;
    }

    public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems) {
      ContactItem i = items.First () as ContactItem;
      SkypeAPI.Instance.StartChat (i ["skype.handle"]);
      yield break;
    }

  }

  public class StartCallAction : Act {
    
    public StartCallAction() {
    }

    public override string Name {
      get { return "Start Skype Call"; }
    }
		
    public override string Description {
      get { return "Start a new Skype call with a contact."; }
    }

    public override string Icon {
      get { return SkypeAPI.RES("CallStart_128x128.png"); }
    }
		
	private string stripchars (string input) {
		string[] elim = {"(", ")", ".", "-", " "};
		foreach (string bad in elim)
			input = input.Replace (bad, "");
		return input;
	}
		
    
    public override bool SupportsItem (Item item) {		if (item is ITextItem) {
			Match m = Regex.Match (stripchars ((item as ITextItem).Text), "^[+]?\\d*$");
			return (m.Success) ? true : false;
		}
		if (item is ContactItem)
			return null != (item as ContactItem) ["skype.handle"];
		if (item is IContactDetailItem) {
			Match m = Regex.Match (stripchars ((item as IContactDetailItem).Description), "^[+]?\\d*$");
			return (m.Success) ? true : false;
		}
		return false;
    }
    
    public override IEnumerable<Type> SupportedItemTypes {
      get {
        return new Type[] { typeof (ContactItem), typeof (ITextItem), typeof (IContactDetailItem) };
      }
    }

    public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem) {
      return false;
    }

    public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems) {
			string number = "";
		
			Item item = items.First ();
		
		if (item is ITextItem) {
			number = stripchars ((item as ITextItem).Text);
			if (number[0] != '+')
				number = "+" + number;
			SkypeAPI.Instance.StartCall (number);
		}
		if (item is ContactItem) {
			ContactItem i = item as ContactItem;
			SkypeAPI.Instance.StartCall (i ["skype.handle"]);
		}
		if (item is IContactDetailItem) {
			IContactDetailItem i = item as IContactDetailItem;
			number = stripchars (i.Description);
			if (number[0] != '+')
				number = "+" + number;
			SkypeAPI.Instance.StartCall (number);
		}
      yield break;
    }

  }

}


using System;
using System.Linq;
using System.Collections.Generic;

using Mono.Addins;

using Do.Universe;

namespace Skype
{

	public class SkypeChatAction : Act
	{

		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Chat"); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Chat with a buddy using Skype"); }
		}
		
		public override string Icon {
			get { return  string.Format ("{0}@{1}", "Message_128x128.png", typeof (Skype).Assembly.FullName); }
		}
		
		public override bool SupportsItem (Item item) {
			if (item is ContactItem)
				return null != (item as ContactItem) ["handle.skype"];
			return true;
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (ContactItem);
				yield return typeof (SkypeContactDetailItem);
			}
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { yield return typeof (ITextItem); }
		}

		public override bool ModifierItemsOptional {
			get { return true; }
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems) {
			Item user = items.First ();
			string message = "";
			
			if (modItems.Any ())
				message = (modItems.First() as ITextItem).Text;
			
			if (user is ContactItem) {
				Skype.ChatWith ((user as ContactItem) ["handle.skype"], message);
				yield break;
			} else if (user is SkypeContactDetailItem) {
				Skype.ChatWith ((user as SkypeContactDetailItem).Handle, message);
				yield break;
			}
			yield break;
		}
	}
}

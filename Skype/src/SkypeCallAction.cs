
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Mono.Addins;

using Do.Universe;

namespace Skype
{
	
	
	public class SkypeCallAction : Act
	{
		
		public SkypeCallAction()
		{
		}
		
	    public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Call"); }
	    }
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Call a contact using Skype"); }
		}
		
		public override string Icon {
			get { return string.Format ("{0}@{1}", "CallStart_128x128.png", typeof (Skype).Assembly.FullName); }
		}
			
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (ContactItem);
				yield return typeof (ITextItem);
				yield return typeof (IContactDetailItem);
				yield return typeof (SkypeContactDetailItem);
			}
		}
    
		public override bool SupportsItem (Item item) 
		{
			if (item is ITextItem)
				return Regex.Match (Skype.StripPhoneChars ((item as ITextItem).Text), "^[+]?\\d*$").Success;
			if (item is ContactItem)
				return null != (item as ContactItem) ["handle.skype"];
			if (item is SkypeContactDetailItem)
				return true;
			if (item is IContactDetailItem) {
				IContactDetailItem detail = (item as IContactDetailItem);
				if (detail.Key.Contains ("phone"))
					return true;
				
				return Regex.Match (Skype.StripPhoneChars (detail.Description), "^[+]?\\d*$").Success;
			}
			return false;
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems) 
		{
			string number;
			
			Item item = items.First ();
			
			if (item is ITextItem) {
				number = Skype.StripPhoneChars ((item as ITextItem).Text);
				if (!number.StartsWith ("+"))
					number = string.Format ("+{0}", number);
				Skype.Call (number);
			} else if (item is SkypeContactDetailItem) {
				Skype.Call ((item  as SkypeContactDetailItem).Handle);
			} else if (item is ContactItem) {
				Skype.Call (item as ContactItem);
			} else if (item is IContactDetailItem) {
				number = Skype.StripPhoneChars ((item as IContactDetailItem).Description);
				if (!number.StartsWith ("+"))
					number = string.Format ("+{0}", number);
				Skype.Call (number);
			}
			yield break;
		}		
	}
}

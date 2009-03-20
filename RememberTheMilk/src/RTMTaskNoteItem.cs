using System;

using Do.Universe;
using Do.Platform;

namespace RememberTheMilk
{
	
	
	public class RTMTaskNoteItem : Item, ITextItem
	{
		string title, text;
		
		public RTMTaskNoteItem (string title, string text)
		{
			this.title = title;
			this.text = text;
		}
		
		public override string Name {
			get { return title; }
		}
		
		public override string Description {
			get { return text; }
		}
		
		public override string Icon {
			get { return "rtm.png@" + GetType ().Assembly.FullName; }
		}
		
		public string Text {
			get { return text; }
		}
	}
}

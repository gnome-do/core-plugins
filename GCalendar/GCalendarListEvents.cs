using System;
using System.Collections;

using Do.Universe;

namespace Do.GCalendar
{
	public class GCalendarListEvents : IAction
	{
		public GCalendarListEvents()
		{
		}
		
		public string Name {
			get { return "List Calendar Events"; }
		}
		
		public string Description {
			get { return "List events from Google Calendar"; }
		}
		
		public string Icon {
			get { return "date"; }
		}
		
		public Type[] SupportedItemTypes {
			get {
				return new Type[] {
					//typeof (GCalCalendar),
					typeof (ITextItem),
				};
			}
		}
		
		public Type[] SupportedModifierItemTypes {
			get { return new Type[] {
					//typeof (GCalEvent),
					typeof (ITextItem),
				};
			}
		}
		
		public bool ModifierItemsOptional {
			get { return false; }
		}
		
		public bool SupportsItem (IItem item) {
			return true;
		}
		
		public bool SupportsModifierItemForItems (IItem[] items, IItem modItem) {
	      return true;
	    }
			
	    public IItem[] DynamicModifierItemsForItem (IItem item) {
	      return null;
	    }
		
		public IItem[] Perform (IItem[] items, IItem[] modifierItems)
		{
			DoGCal cal = new DoGCal ();
			cal.Connect ();
			cal.ListCalendars ();
			return null;
		}
	}
}

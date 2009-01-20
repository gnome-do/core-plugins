using System;
using System.Collections.Generic;
using Do.Universe;
using Do.Platform;

namespace YouTube
{
	public class YouTubeSearchAction : Act
	{
					
		public YouTubeSearchAction()
		{
		}
		 
		public override string Name
		{
			get { return "Search in YouTube"; }
		}
		
		public override string Description
		{
			get { return "Searches in YouTube"; }
		}
		
		public override string Icon
		{
			get { return "youtube_logo.png@" + GetType ().Assembly.FullName; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return	typeof (ITextItem); }
		}

		public override bool SupportsItem (Item item)
		{
			if (item is ITextItem) 
				return true;
			return false;
		}
	
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{				
			string url = "http://www.youtube.com/results?search_query=";
			string search = "";
			
			foreach (Item item in items) {
				if (item is IUrlItem) {
					search = (item as IUrlItem).Url;
				} else if (item is ITextItem) {
					search = (item as ITextItem).Text;
				}
				search = search.Replace (" ", "%20");
				Services.Environment.OpenUrl(url+search);
			}
			return null;
		}
	}
}

using System;
using Do.Universe;

namespace Do.Universe
{	
	public class YouTubeSubscriptionItem : Item, IUrlItem
	{
		protected string name;
		protected string url;
		protected string description;
		
		public YouTubeSubscriptionItem(string name, string url, string description)
		{
			this.name= name;
			this.url = url;
			this.description = description;
		}
		
		public override string Name
		{
			get { return name; }
		}
		
		public override string Description
		{
			get { return description; }
		}
		
		public override string Icon
		{
			get { return "youtube_user.png@" + GetType ().Assembly.FullName; }
		}
		
		public string Url
		{
			get { return url; }
		}
		
	}
}

// YoutubeVideoItem.cs created with MonoDevelop
// User: luis at 05:56 p 06/09/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using Do.Universe;

namespace YouTube
{	
	
	public class YoutubeVideoItem : Item, IUrlItem
	{
		protected string name;
		protected string url;
		protected string description;
		
		public YoutubeVideoItem(string name, string url, string description)
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
			get { return "youtube_logo.png@" + GetType ().Assembly.FullName; }
		}
		
		public string Url
		{
			get { return url; }
		}
		
	}
}

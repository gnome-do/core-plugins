using System;
using System.Collections.Generic;
using Mono.Unix;
using Gtk;
using Do.Universe;
using Do.Platform.Linux;
using System.Threading;

namespace Do.Universe
{
	public class YouTubeFavoriteItemSource : ItemSource, IConfigurable
	{
		public YouTubeFavoriteItemSource()
		{
		}
		
		public override IEnumerable<Type> SupportedItemTypes 
		{
			get { yield return typeof (YoutubeVideoItem);}
		}
		
		public override string Name { get { return "Youtube Favorites"; } }
		public override string Description { get { return "Videos on your Youtube favorites list."; } }
		public override string Icon {get { return "youtube_logo.png@" + GetType ().Assembly.FullName; } }
		
		public override IEnumerable<Do.Universe.Item> Items 
		{
			get { return Youtube.favorites; }
		}
		
		public override void UpdateItems ()
		{
			Thread t = new Thread((ThreadStart) Youtube.updateFavorites);
			t.IsBackground = true;
			t.Start();
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new YouTubeConfig ();
		}
		
	}
}

// YouTubeFavoriteItemSource.cs created with MonoDevelop
// User: luis at 05:38 p 06/09/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;
using System.Threading;
using Mono.Unix;

using Gtk;
using Do.Universe;
using Do.Platform.Linux;

namespace YouTube
{
	
	
	public class YouTubeFavoriteItemSource : ItemSource, IConfigurable
	{
		public YouTubeFavoriteItemSource()
		{
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] {
					typeof (YoutubeVideoItem),
				};
			}
		}
		
		public override string Name { get { return "Youtube Favorites"; } }
		public override string Description { get { return "Videos on your Youtube favorites list."; } }
		public override string Icon {get { return "youtube_logo.png@" + GetType ().Assembly.FullName; } }
		
		public override IEnumerable<Do.Universe.Item> Items {
			get { return Youtube.favorites; }
		}
		
		public override IEnumerable<Do.Universe.Item> ChildrenOfItem (Do.Universe.Item item)
		{
			return null;
		}
		
		public override void UpdateItems ()
		{
			try {
				Thread thread = new Thread ((ThreadStart) (Youtube.updateFavorites));
				thread.IsBackground = true;
				thread.Start ();
			} catch (Exception e) {
				Console.Error.WriteLine (e.Message);
			}
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new YouTubeConfig ();
		}
		
	}
}

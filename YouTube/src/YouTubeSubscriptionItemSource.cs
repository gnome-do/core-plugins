// YouTubeSubscriptionItemSource.cs created with MonoDevelop
// User: luis at 01:57 p 07/09/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;
using System.Threading;
using Mono.Unix;
using Do.Universe;

namespace YouTube
{
	
	
	public class YouTubeSubscriptionItemSource : ItemSource
	{
		public YouTubeSubscriptionItemSource()
		{
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] {
					typeof (YouTubeSubscriptionItem),
				};
			}
		}
		
		public override string Name { get { return "Youtube Subscriptions"; } }
		public override string Description { get { return "Your YouTube subscriptions"; } }
		public override string Icon {get { return "youtube_user.png@" + GetType ().Assembly.FullName; } }
		
		public override IEnumerable<Item> Items {
			get { return Youtube.subscriptions; }
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			return null;
		}
		
		public override void UpdateItems ()
		{
			try {
				Thread thread = new Thread ((ThreadStart) (Youtube.updateSubscriptions));
				thread.IsBackground = true;
				thread.Start ();
			} catch (Exception e) {
				Console.Error.WriteLine (e.Message);
			}
		}		
	}
}


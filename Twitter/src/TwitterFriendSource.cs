// TwitterFriendSource.cs created with MonoDevelop
// User: alex at 5:19 PM 4/15/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Threading;
using System.Collections.Generic;

using Do.Universe;

namespace Twitter
{	
	public sealed class TwitterFriendSource : IItemSource
	{
		List<IItem> items;
		
		public TwitterFriendSource()
		{
			items = new List<IItem> ();
			UpdateItems ();
		}
		
		public string Name { get { return "Twitter Friends"; } }
		public string Description { get { return "Indexes your Twitter Friends"; } }
		public string Icon { get { return "system-users"; } }
		
		public Type[] SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (ContactItem),
				};
			}
		}
		
		public ICollection<IItem> Items
		{
			get { return items; }
		}
		
		public ICollection<IItem> ChildrenOfItem (IItem parent)
		{
			return null;
		}
		
		public void UpdateItems ()
		{
			Thread updateRunner = new Thread( new ThreadStart(Twitter.GetTwitterFriends));
			updateRunner.Start ();
			items = Twitter.Friends;
		}
	}
}

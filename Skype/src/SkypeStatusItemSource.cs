// SkypeStatusItemSource.cs
//
// GNOME Do is the legal property of its developers, whose names are too
// numerous to list here.  Please refer to the COPYRIGHT file distributed with
// this source distribution.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Linq;
using System.Collections.Generic;

using Mono.Addins;

using Do.Universe;

namespace Skype
{

	public class SkypeStatusItemSource : ItemSource
	{

		List<Item> statuses;		
		public SkypeStatusItemSource ()
		{
			statuses = new List<Item> ();
			Skype.Statuses.Values.Where (st => st.Showable == true)
				.ForEach (status => statuses.Add (status));
		}
		
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Skype Statuses"); }
		}

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Skype Statuses"); }
		}

		public override string Icon {
			get { return "skype"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (StatusItem);
				yield return typeof (IApplicationItem);
				yield return typeof (SkypeBrowseStatusItem);
			}
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			if (Skype.IsSkype (item)) {
				yield return new SkypeBrowseStatusItem ();
			} else if (item is SkypeBrowseStatusItem) {
				foreach (Item status in statuses)
					yield return status;
			}
		}
		
		public override IEnumerable<Item> Items {
			get { return statuses; }
		}
		
		public override void UpdateItems () 
		{
		}
	}
}

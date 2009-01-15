// PlaySlideshowAction.cs
//
// GNOME Do is the legal property of its developers. Please refer to the
// COPYRIGHT file distributed with this source distribution.
//
// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with
// this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

using Mono.Unix;

using Do.Universe;
using Do.Platform;

namespace EOG
{
	public class PlaySlideshowAction : Act
	{
		public PlaySlideshowAction ()
		{
		}

		public override string Name {
			get { return Catalog.GetString ("Play Slideshow"); }
		}

		public override string Description {
			get { return Catalog.GetString ("Plays a slideshow of images in a folder."); }
		}

		public override string Icon {
			get { return "eog"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (IFileItem); }
		}

		public override bool SupportsItem (Item item)
		{
			return Directory.Exists ((item as IFileItem).Path);
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems)
		{
			items.OfType<IFileItem> ().Select (file => file.Path).ForEach (PlaySlideshow);
			yield break;
		}

		void PlaySlideshow (string path)
		{
			try {
				Process.Start ("eog", "--slide-show \"" + path + "\"");
			} catch (Exception e) {
				Log.Error ("Could not play slideshow for {0}: {1}", path, e.Message);
				Log.Debug (e.StackTrace);
			}
		}
	}
}

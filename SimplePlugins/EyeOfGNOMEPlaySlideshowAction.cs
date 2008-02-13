/* EpiphanyBookmarkItemSource.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Diagnostics;

using Do.Universe;

namespace EyeOfGNOME
{
	public class PlaySlideshowAction : AbstractAction
	{
		public PlaySlideshowAction ()
		{
		}

		public override string Name
		{
			get { return "Play Slideshow"; }
		}

		public override string Description
		{
			get { return "Plays a slideshow of images in a folder."; }
		}

		public override string Icon
		{
			get { return "eog"; }
		}

		public override Type[] SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (IFileItem),
				};
			}
		}

		public override bool SupportsItem (IItem item)
		{
			return System.IO.Directory.Exists ((item as IFileItem).Path);
		}

		public override IItem[] Perform (IItem[] items, IItem[] modifierItems)
		{
			string path;

			path = (items[0] as IFileItem).Path;
			try {
				Process.Start ("eog", "--slide-show \"" + path + "\"");
			} catch (Exception e) {
				Console.Error.WriteLine
					("Could not play slideshow for {0}: {1}", path, e.Message);
			}
			return null;
		}
	}
}

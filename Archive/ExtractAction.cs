/* ExtractAction.cs
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
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Do.Universe;

namespace GnomeDoArchive {
	public class ExtractAction : AbstractAction {

		public override string Name {
			get {
				return "Extract";
			}
		}
		
		public override string Description {
			get {
				return "Extract an archive";
			}
		}
		
		public override string Icon {
			get {
				return "gnome-mime-application-x-archive";
			}
		}
		
		public override Type[] SupportedItemTypes {
            get {
                return new Type[] {
                    typeof(IFileItem),
                };
            }
		}
		
		public override bool SupportsItem (IItem item)
		{
			// Check for archive types
			return true;
		}

		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			IFileItem fi = items[0] as IFileItem;

			Process process = new Process ();
			process.StartInfo.FileName = "/usr/bin/file-roller";
			process.StartInfo.Arguments = "-f " + fi.Path;
			process.Start ();
			return null;
		}
	}

	// TODO: This action doesn't work, because there is an problem with file-roller.
	public class ExtractHereAction : AbstractAction {

		public override string Name {
			get {
				return "Extract Here";
			}
		}
		
		public override string Description {
			get {
				return "Extract an archive in the current dir";
			}
		}
		
		public override string Icon {
			get {
				return "gnome-mime-application-x-archive";
			}
		}
		
		public override Type[] SupportedItemTypes {
            get {
                return new Type[] {
                    typeof(IFileItem),
                };
            }
		}
		
		public override bool SupportsItem (IItem item)
		{
			// Check for archive types
			return true;
		}

		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			IFileItem fi = items[0] as IFileItem;

			Process process = new Process ();
			process.StartInfo.FileName = "/usr/bin/file-roller";
			process.StartInfo.Arguments = "-h " + fi.Path;
			process.Start ();
			return null;
		}
	}
}


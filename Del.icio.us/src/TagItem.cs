/* TagItem.cs
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
using Do.Universe;
using Mono.Addins;

namespace Delicious
{
	public class TagItem : Item
	{
		string tag;
		public TagItem (string tag)
		{
			this.tag = tag;
		}
		
		public override string Name {
			get {
				return tag;
			}
		}
		
		public override string Description {
			get { 
				if (tag.Equals ("Untagged"))
					return AddinManager.CurrentLocalizer.GetString ("Untagged del.ico.us bookmarks");				
				return string.Format (
					AddinManager.CurrentLocalizer.GetString ("del.icio.us bookmarks tagged with {0}"), Name);
			}
		}
		
		public override string Icon {
			get { return "delicious.png@" + GetType ().Assembly.FullName; }
		}
	}
}

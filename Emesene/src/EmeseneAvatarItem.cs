/* EmeseneAvatarItem.cs
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
using Do.Platform;

namespace Do.Universe
{
	public class EmeseneAvatarItem : Item, IFileItem
	{
		
		private string path;
		
		public EmeseneAvatarItem(string path)
		{			
			this.path = path;
		}		
		
		
		public override string Name
		{
			get { return path; }
		}
		
		public override string Description
		{
			get { return "Emesene avatar."; }
		}
		
		public  string Uri
		{
			get { return "file://" + Path; }
		}
		
		public  string Path
		{
			get { return this.path; }
		}
		
		public override string Icon
		{
			get { return this.path; }
		}
		
		public override bool Equals(object obj)
		{
		    if (obj == null) return false;

		    if (this.GetType() != obj.GetType()) return false;
		    EmeseneAvatarItem avatar = (EmeseneAvatarItem) obj;     

		    // Check for paths
		    if (!Object.Equals(this.path, avatar.path)) return false;

		    return true;
		} 
		
	}
}

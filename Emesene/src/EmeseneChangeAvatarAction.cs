/* EmeseneChangeAvatarAction.cs
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
	
namespace Emesene
{	
	public class EmeseneChangeAvatarAction : Act
	{
		public static Dictionary<string, string> imageMimeTypeMap = new Dictionary<string,string>
			{
				{".jpg", "image/jpeg"},
				{".jpeg", "image/jpeg"},
				{".png", "image/png"}, 
				{".gif", "image/gif"}, 
				{".bmp", "image/bmp"}, 
				{".tif", "image/tiff"}, 
				{".tiff", "image/tiff"}
			};
		
		public EmeseneChangeAvatarAction()
		{
		}
		
		private bool IsImageFile (IFileItem file)
		{
			return imageMimeTypeMap.ContainsKey (Path.GetExtension (file.Path));
		}
		
		public override string Name
		{
			get { return "Change emesene display picture"; }
		}
		
		public override string Description
		{
			get { return "Change your emesene display picture"; }
		}
		
		public override string Icon 
		{
			get { return "emesene"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return	typeof (IFileItem); }
		}
			
		public override bool SupportsItem (Item item)
		{	
			if (item is IFileItem)
				    return IsImageFile((item as IFileItem));
			return false;
		}	
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			Emesene.set_avatar((items.First() as IFileItem).Path);
			yield break;
		}
	}
}

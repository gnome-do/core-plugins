/* EmeseneAvatarItemSource.cs
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
using System.IO;
using Do.Universe;
using Do.Platform;
using System.Collections.Generic;

namespace Do.Universe
{	
	public class EmeseneAvatarItemSource : ItemSource
	{
		private List<Item> avatars;
		
		public EmeseneAvatarItemSource()
		{
			this.avatars = new List<Item>();
		}
	
	public override void UpdateItems ()
		{
			Log<EmeseneAvatarItemSource>.Debug ("Checking for emesene Dbus...");
			if (Emesene.checkForEmesene())
			{
    			Log<EmeseneAvatarItemSource>.Debug ("Emesene Dbus is ON");
				string avatarsPath = Emesene.getAvatarPathForUser();				
    			Log<EmeseneAvatarItemSource>.Debug ("Folder with emesene avatars: {0}", avatarsPath);
				string [] fileEntries = Directory.GetFiles(avatarsPath);
			    foreach(string fileName in fileEntries)
			    {	       
					if(!fileName.Contains("_thumb") && !this.avatars.Contains(new EmeseneAvatarItem(fileName))) 
					{
						this.avatars.Add(new EmeseneAvatarItem(fileName));
					}
			    }
			} 
			else 
			{
    			Log<EmeseneAvatarItemSource>.Debug ("Emesene Dbus is OFF");
			}			
		}
		
		public override string Name { get { return "Emesene Avatars"; } }
		public override string Description { get { return "Avaliable emesene avatars for your account."; }}
		public override string Icon {get { return "emesene"; } }
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get { yield return typeof (EmeseneAvatarItem); }
		}
		
		public override IEnumerable<Do.Universe.Item> Items 
		{
			get { return this.avatars; }
		}
	}
}

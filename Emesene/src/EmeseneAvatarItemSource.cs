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

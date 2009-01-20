using System;
using System.IO;
using Do.Universe;
using System.Collections.Generic;

namespace Emesene
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
			Log.Debug ("Emesene > Checking for emesene Dbus...");
			if (Emesene.checkForEmesene())
			{
    			Log.Debug ("Emesene > Emesene Dbus is ON");
				string avatarsPath = Emesene.getAvatarPathForUser();				
    			Log.Debug ("Emesene > Folder with emesene avatars: {0}", avatarsPath);
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
    			Log.Debug ("Emesene > Emesene Dbus is OFF");
			}			
		}
		
		public override string Name { get { return "Emesene Avatars"; } }
		public override string Description { get { return "Avaliable emesene avatars for your account."; }}
		public override string Icon {get { return "emesene"; } }
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (EmeseneAvatarItem)
				};
			}
		}
		
		public override IEnumerable<Do.Universe.Item> Items 
		{
			get { return this.avatars; }
		}
	}
}

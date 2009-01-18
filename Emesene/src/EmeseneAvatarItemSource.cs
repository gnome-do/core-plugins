// EmeseneAvatarItemSource.cs created with MonoDevelop
// User: luis at 03:56 p 21/11/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

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
			System.Console.WriteLine("---Checking for emesene---");
			if (Emesene.checkForEmesene())
			{
				System.Console.WriteLine("---emesene Dbus is ON---");
				string avatarsPath = Emesene.getAvatarPathForUser();				
				System.Console.WriteLine("Folder with emesene avatars: " + avatarsPath);
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
				System.Console.WriteLine("---emesene Dbus is OFF---");
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

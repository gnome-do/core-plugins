using System;
using Do.Universe;
using System.Collections.Generic;
using System.IO;
	
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
			foreach(Item avatar in items)
			{
				string path = (avatar as IFileItem).Path;
				Emesene.set_avatar(path);
			}
			return null;
		}
	}
}

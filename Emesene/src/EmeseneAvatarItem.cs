// EmeseneAvatarItem.cs created with MonoDevelop
// User: luis at 03:06 p 21/11/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using Do.Universe;
using Do.Platform;

namespace Emesene
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

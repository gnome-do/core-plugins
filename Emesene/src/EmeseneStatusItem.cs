// EmeseneStatusItem.cs created with MonoDevelop
// User: luis at 12:01 p 21/11/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using Do.Universe;

namespace Emesene
{	
	public class EmeseneStatusItem : Item
	{
		private string name;
		private string description;
		private string icon;
		private string abbreviation;
		
		public EmeseneStatusItem(string name, string description, string abbreviation)
		{
			this.name = name;
			this.description = description;
			this.icon = name+".png@" + GetType ().Assembly.FullName;
			this.abbreviation = abbreviation;
		}
		
		public string GetAbbreviation()
		{
			return this.abbreviation;
		}
		
		public override string Name
		{
			get { return this.name; }
		}
		
		public override string Description
		{
			get { return this.description; }
		}
		
		public override string Icon
		{
			get { return this.icon; }
		}
	}
}

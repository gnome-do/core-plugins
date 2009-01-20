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

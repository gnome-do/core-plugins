
using System;

using Do.Universe;

namespace Skype
{
	
	
	public class StatusItem : Item
	{
		string name, icon;
		public StatusItem (string name, string code, string icon)
		{
			this.name = name;
			this.icon = icon;
			this.Code = code;
		}
		
		public override string Name {
			get { return name; }
		}
		
		public override string Description {
			get { return name; }
		}

		public override string Icon {
			get { return string.Format ("{0}@{1}", icon, typeof (Skype).Assembly.FullName); }
		}
		
		public string Code { get; private set; }
	}
}

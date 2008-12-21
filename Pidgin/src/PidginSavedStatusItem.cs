// PidginStatusItem.cs created with MonoDevelop
// User: alex at 12:28 PM 4/8/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Text.RegularExpressions;
using Do.Universe;

namespace Do.Addins.Pidgin
{
	public sealed class PidginSavedStatusItem : Item
	{
		private string name, message, iconBase;
		private int status, id;

		public PidginSavedStatusItem(string name, string message, int id, int status)
		{
			this.name = name;
			this.message = message;
			this.status = status;
			this.id = id;
			this.iconBase = "/usr/share/pixmaps/pidgin/status/48/";
		}
		
		public override string Name { get { return name; } }
		public override string Description { get { return StripHTML (message); } }
		public int Status { get { return status; } }
		public int ID { get { return id; } }
		public override string Icon { 
			get  { 
				switch (status) {
				case 2: return iconBase + "available.png";
				case 3: return iconBase + "busy.png";
				//there is not a 48px invisible icon.
				case 4: return "/usr/share/pixmaps/pidgin/status/32/invisible.png";
				case 5: return iconBase + "away.png";
				default: return "pidgin";
				}
			}
		}
		
		private string StripHTML (string message)
		{
			return Regex.Replace(message, @"<(.|\n)*?>", string.Empty);
		}
	}
}

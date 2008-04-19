// PidginStatusItem.cs created with MonoDevelop
// User: alex at 12:28 PM 4/8/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;

namespace Pidgin
{
	public class PidginStatusItem
	{
		private string name, message;
		private int status;

		public PidginStatusItem(string name, string message, int status)
		{
			this.status = status;
			this.message = message;
		}
		
		public string Name { get { return name; } }
		public string Description { get { return message; } }
		public int Status { get { return status; } }
		public string Icon { get { return "pidgin"; } }
	}
}

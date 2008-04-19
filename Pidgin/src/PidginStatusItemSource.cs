// PidginStatusItemSource.cs created with MonoDevelop
// User: alex at 12:19 PM 4/8/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using Do.Universe;

namespace Pidgin
{
	public class PidginStatusItemSource : IItemSource
	{
		private static readonly string status_file;
		private List<IItem> statuses;
		
		static PidginStatusItemSource() {
			string home =  Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			status_file = "~/.purple/status.xml".Replace("~", home);
		}
		
		public PidginStatusItemSource() {
			statuses = new List<IItem> ();
			UpdateItems();
		}
		
		public Type[] SupportedItemTypes {
			get {
				return new Type[] {
					typeof (PidginStatusItem),
				};
			}
		}
		
		public enum status {
			offline = 1, available, unavailable, invisible, away,
			extended_away, mobile, tune
		};
		
		public string Name { get { return "Pidgin Statuses"; } }
		public string Description { get { return ""; } }
		public string Icon {get { return "pidgin"; } }
		
		public ICollection<IItem> Items {
			get { return statuses; }
		}
		
		public ICollection<IItem> ChildrenOfItem (IItem item)
		{
			return null;
		}
		
		public void UpdateItems () {
			XmlDocument statuses_xml = new XmlDocument ();
			statuses.Clear ();
			try {
				statuses_xml.Load (status_file);
				foreach (XmlNode status in statuses_xml.GetElementsByTagName ("status")) {
				}
			} catch { }
		}
	}
}

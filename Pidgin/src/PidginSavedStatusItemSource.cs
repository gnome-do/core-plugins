// PidginStatusItemSource.cs created with MonoDevelop
// User: alex at 12:19 PM 4/8/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;

using Do.Universe;

namespace Do.Addins.Pidgin
{
	public class PidginSavedStatusItemSource : IItemSource
	{
		private List<IItem> statuses;
		
		public PidginSavedStatusItemSource() {
			statuses = new List<IItem> ();
			UpdateItems();
		}
		
		public string Name { get { return "Pidgin Statuses"; } }
		public string Description { get { return "Saved Pidgin statuses"; } }
		public string Icon {get { return "pidgin"; } }
		
		public Type[] SupportedItemTypes {
			get {
				return new Type[] {
					typeof (PidginSavedStatusItem),
				};
			}
		}
		
		public ICollection<IItem> Items {
			get { return statuses; }
		}
		
		public ICollection<IItem> ChildrenOfItem (IItem item)
		{
			return null;
		}
		
		public void UpdateItems () 
		{			
			Pidgin.IPurpleObject prpl;
			int [] rawStatuses;
			try {
				prpl = Pidgin.GetPurpleObject ();
				statuses.Clear ();
				rawStatuses = prpl.PurpleSavedstatusesGetAll ();
				foreach (int status in rawStatuses) {
					if (!prpl.PurpleSavedstatusIsTransient (status)) {
						string title, message;
						int id, statId;
						
						title = prpl.PurpleSavedstatusGetTitle (status);
						message = prpl.PurpleSavedstatusGetMessage (status);
						id = prpl.PurpleSavedstatusFind (title);
						statId = prpl.PurpleSavedstatusGetType (status);
						
						statuses.Add (new PidginSavedStatusItem (title,message,id,statId));
					}
				}
			} catch { }
		}
	}
}

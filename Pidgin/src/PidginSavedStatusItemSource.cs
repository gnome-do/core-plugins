// PidginSavedStatusItemSource.cs
//
// GNOME Do is the legal property of its developers, whose names are too
// numerous to list here.  Please refer to the COPYRIGHT file distributed with
// this source distribution.
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Linq;
using System.Collections.Generic;

using Mono.Unix;

using Do.Universe;
using Do.Platform;

namespace PidginPlugin
{

	public class PidginSavedStatusItemSource : ItemSource
	{

		List<Item> statuses;		
		public PidginSavedStatusItemSource ()
		{
			statuses = new List<Item> ();
			statuses.Add (new PidginStatusTypeItem (1));
			statuses.Add (new PidginStatusTypeItem (2));
			statuses.Add (new PidginStatusTypeItem (3));
			statuses.Add (new PidginStatusTypeItem (4));
			statuses.Add (new PidginStatusTypeItem (5));
		}
		
		public override string Name {
			get { return Catalog.GetString ("Pidgin Statuses"); }
		}

		public override string Description {
			get { return Catalog.GetString ("Saved Pidgin statuses"); }
		}

		public override string Icon {
			get { return "pidgin"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { 
				yield return typeof (PidginSavedStatusItem);
				yield return typeof (PidginStatusTypeItem);
			}
		}
		
		public override IEnumerable<Item> Items {
			get { return statuses; }
		}
		
		public override void UpdateItems () 
		{			
			Pidgin.IPurpleObject prpl;
			int [] rawStatuses;
			if (Pidgin.InstanceIsRunning) {
				try {
					prpl = Pidgin.GetPurpleObject ();
					foreach (Item status in statuses.Where (i => i is PidginSavedStatusItem).ToArray ())
						statuses.Remove (status);
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
				} catch (Exception e) {
					Log<PidginSavedStatusItemSource>.Error ("Could not read saved statuses: {0}", e.Message);
					Log<PidginSavedStatusItemSource>.Debug (e.StackTrace);
				}
			}
		}
	}
}

//  EmpathySavedStatusItemSource.cs
//
//  Author:
//       Xavier Calland <xavier.calland@gmail.com>
//
//  Copyright © 2010
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Linq;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using Mono.Addins;

using Do.Universe;
using Do.Platform;

using Telepathy;

namespace EmpathyPlugin
{
	public class EmpathySavedStatusItemSource : ItemSource
	{
		static readonly string PresetsFile;
		List<Item> statuses;

		static EmpathySavedStatusItemSource ()
		{
			string home = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
			string[] PresetsFileParts = new string[] {home, EmpathyPlugin.PRESETS_STATUS_PLACE};
		 	PresetsFile = PresetsFileParts.Aggregate ((aggregation, val) => Path.Combine (aggregation, val));
		}

		public EmpathySavedStatusItemSource ()
		{
			statuses = new List<Item> ();
			statuses.Add (new EmpathyStatusItem(ConnectionPresenceType.Offline));
			statuses.Add (new EmpathyStatusItem(ConnectionPresenceType.Available));
			statuses.Add (new EmpathyStatusItem(ConnectionPresenceType.Away));
			statuses.Add (new EmpathyStatusItem(ConnectionPresenceType.Busy));
			statuses.Add (new EmpathyStatusItem(ConnectionPresenceType.Hidden));
		}

		public override string Name
		{
			get { return AddinManager.CurrentLocalizer.GetString ("Empathy Statuses"); }
		}

		public override string Description
		{
			get { return AddinManager.CurrentLocalizer.GetString ("Saved Empathy statuses"); }
		}

		public override string Icon
		{
			get { return "empathy"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get { 
				yield return typeof (EmpathySavedStatusItem);
				yield return typeof (EmpathyStatusItem);
				yield return typeof (EmpathyBrowseStatusItem);
				yield return typeof (IApplicationItem);
			}
		}

		public override IEnumerable<Item> ChildrenOfItem (Item item)
		{
			if (EmpathyPlugin.IsTelepathy (item))
			{
				yield return new EmpathyBrowseStatusItem ();
			} 
			else if (item is EmpathyBrowseStatusItem)
			{
				foreach (Item status in statuses)
					yield return status;
			}
		}

		public override IEnumerable<Item> Items
		{
			get { return statuses; }
		}

		public override void UpdateItems () 
		{
			// suppression des preset status déja lus
			statuses.RemoveAll(new System.Predicate<Item>( delegate(Item val) { return (val is EmpathySavedStatusItem); }));

			// lire les status enregistrés
			XmlDocument statusList = new XmlDocument ();
			try {
				statusList.Load (PresetsFile);

				foreach (XmlNode statusNode in statusList.GetElementsByTagName ("status")) {
					string pres = statusNode.Attributes.GetNamedItem("presence").Value;
					string message = statusNode.InnerText;

					statuses.Add (new EmpathySavedStatusItem (EmpathyStatus.GetPresence(pres), message));
				}

			} catch (Exception e) {
				Log<EmpathySavedStatusItemSource>.Error ("Error reading presets statuses: {0}", e.Message);
				Log<EmpathySavedStatusItemSource>.Debug (e.StackTrace);
			}
		}
	}
}

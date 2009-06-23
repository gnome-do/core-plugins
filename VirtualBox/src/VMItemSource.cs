// VMItemSource.cs
//
//  GNOME Do is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this
//  source distribution.
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using Mono.Addins;


using Do.Universe;
using Do.Platform;

namespace VirtualBox {

public class VMItemSource : ItemSource {
		List<Item> items;
		public VMItemSource ()
		{
			items = new List<Item> ();
		}
		
		public override string Name { 
			get { 
				return AddinManager.CurrentLocalizer.GetString ("VirtualBox VMs"); 
			} 
		}
		
		public override string Description { 
			get { 
				return AddinManager.CurrentLocalizer.GetString ("Virtual Machines created with VirtualBox"); 
			} 
		}
		
		public override string Icon { 
			get { 
				return "VirtualBox_64px.png@" + GetType ().Assembly.FullName; 
			} 
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (VMItem);
				yield return typeof (IApplicationItem);
				yield return typeof (VBoxBrowseVMSItem);
			}
		}

		public override IEnumerable<Item> Items {
			get { return items; }
		}

		public override IEnumerable<Item> ChildrenOfItem (Item parent)
		{
			if (parent is IApplicationItem && (parent as IApplicationItem).Exec.Contains ("VirtualBox"))
				yield return new VBoxBrowseVMSItem ();
			if (parent is VBoxBrowseVMSItem) {
				foreach (VMItem item in Items)
					yield return item;
			}
			yield break;
		}

		public override void UpdateItems ()
		{
			items.Clear ();
			try {
				string home = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
				string xml_file = Path.Combine (home, ".VirtualBox/VirtualBox.xml");
				XmlDocument VboxXML = new XmlDocument();
				VboxXML.Load (xml_file);
				XmlNodeList MachineEntries = VboxXML.GetElementsByTagName ("MachineEntry");
				//add each VM as a VMItem
				foreach (XmlNode Machine in MachineEntries)
					items.Add (new VMItem (Machine.Attributes));	
			} catch (Exception e) {
				//meltdown
				Log<VMItemSource>.Error ("Error parsing VBox XML file.");
				Log<VMItemSource>.Debug (e.ToString ());
			}
		}
	}
}


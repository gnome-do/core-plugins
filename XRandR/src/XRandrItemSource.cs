/* XRandrItemSource.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this source distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * Enumerates outputs and modes using xrandr lib wrapper.
 */

using System;
using Do.Universe;
using System.Collections.Generic;
using Mono.Unix;

namespace XRandR
{
	public class XRandRItemSource : ItemSource
	{
		List<Item> items;

		public XRandRItemSource ()
		{
			items = new List<Item> ();
		}		
		
		public override string Name {
			get { return Catalog.GetString ("Displays"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Set your resolution"); }
		}
		
		public override string Icon {
			get { return "system-config-display"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { return new Type [] {
					typeof (OutputItem)
				};
			}
		}
		
		public override IEnumerable<Item> Items {
			get { return items; }
		}

		public override IEnumerable<Item> ChildrenOfItem (Item parent)
		{
			if (parent is OutputItem){
				OutputItem outputItem = parent as OutputItem;
				foreach(ScreenResources res in External.ScreenResources())
					foreach(XRROutputInfo output in res.Outputs.doWith(outputItem.Id)){
						foreach(XRRModeInfo mode in res.ModesOfOutput(output))
							yield return new OutputModeItem(outputItem.Id,mode);
						
						if (output.crtc_id != 0)
							yield return new OutputModeItem(outputItem.Id,0,Catalog.GetString("Off"));
					}
			}
			else
				yield break;
		}
		
		public override void UpdateItems ()
		{
			items.Clear ();
			foreach(ScreenResources res in External.ScreenResources()){
				res.Outputs.AllWithId(delegate(int id,XRROutputInfo output){
					items.Add (new OutputItem(id,output));
				});
			}
		}
	}
}

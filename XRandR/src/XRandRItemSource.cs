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
using System.Collections.Generic;

using Mono.Unix;

using Do.Universe;

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
			get { 
				yield return typeof (OutputItem);
			}
		}
		
		public override IEnumerable<Item> Items {
			get { return items; }
		}

		public override IEnumerable<Item> ChildrenOfItem (Item parent)
		{
			if (parent is OutputItem) {
				OutputItem outputItem = parent as OutputItem;
				foreach(ScreenResources res in Wrapper.ScreenResources ()) {
					foreach(XRROutputInfo output in res.Outputs.DoWith (outputItem.Id)){
						foreach(XRRModeInfo mode in res.ModesOfOutput (output)) {
							yield return new OutputModeItem (outputItem.Id, mode);
						}
						
						if (output.crtc_id != 0)
							yield return new OutputModeItem (outputItem.Id, 0, Catalog.GetString ("Off"));
					}
				}
			}
			
			yield break;
		}
		
		public override void UpdateItems ()
		{
			try {
				items.Clear ();
				foreach (ScreenResources res in Wrapper.ScreenResources ()){
					res.Outputs.AllWithId (delegate (int id, XRROutputInfo output){
						Do.Platform.Log<XRandRItemSource>.Debug ("Found output: 0x{0:x} - {1}", id, output.name); 
						items.Add (new OutputItem (id, output, output.connection == 0));
					});
				}
			} catch (Exception e) {
				// Necessary, since Do.Universe.SafeElement.LogSafeError does not output a StackTrace 
				Do.Platform.Log<XRandRItemSource>.Error ("Error in UpdateItems: {0}\n{1}", e.Message, e.StackTrace);
				throw e;
			}
		}
	}
}

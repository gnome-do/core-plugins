/* OutputModeItem.cs
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
 * OutputModeItem is an item representing a possible mode for an output.
 */

using System;

using Mono.Addins;
using Mono.Unix;

using Do.Universe;

namespace XRandR
{
	public class OutputModeItem : Item,IRunnableItem
	{
		string name;
		int output_id, mode_id;
	
		public OutputModeItem (int output_id, XRRModeInfo mode)
		{
			this.output_id = output_id;
			this.mode_id = mode.id.ToInt32 ();
			this.name = mode.name + " " + mode.dotClock.ToInt64 () / mode.vTotal / mode.hTotal + "Hz";
		}
		
		public OutputModeItem (int output_id, int mode_id, string name)
		{
			this.name = name;
			this.mode_id = mode_id;
			this.output_id = output_id;
		}			
		
		public override string Name {
			get { return name; }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Set your resolution"); }
		}
		
		public override string Icon {
			get { return "system-config-display"; }
		}
		
		public void Run () 
		{
			foreach (ScreenResources res in Wrapper.ScreenResources ())
				res.setMode (output_id, mode_id);
		}
	}
}

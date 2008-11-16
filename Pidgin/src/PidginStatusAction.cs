// PidginStatusAction.cs
//
//  GNOME Do is the legal property of its developers, whose names are too numerous
//  to list here.  Please refer to the COPYRIGHT file distributed with this
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

using Do.Universe;
using Do.Addins;

namespace Pidgin
{
	public class PidginStatusAction : IAction
	{	
		public PidginStatusAction()
		{
		}
		
		public string Name {
			get {
				return "Set Status";
			}
		}
		
		public string Description {
			get {
				return "Set Pidgin availability";
			}
		}
		
		public string Icon {
			get {
				return "pidgin";
			}
		}
		
		public IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] {
					typeof (PidginStatusItem),
					typeof (ITextItem),
				};
			}
		}
		
		public bool SupportsItem (IItem item) {
			return true;
		}
		
		public bool ModifierItemsOptional {
			get { return true; }
		}
		
		public Type[] SupportedModifierItemTypes {
			get {
				return null;
			}
		}
		
		public bool SupportsModifierItemForItems (IEnumerable<IItem> items, IItem modItem) {
			return true;
		}
		
		public IItem[] DynamicModifierItemsForItem (IItem item) {
			return null;
		}
		
		public IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			return null;
		}
	}
}

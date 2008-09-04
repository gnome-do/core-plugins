// ClickableItemSource.cs
// 
// Copyright (C) 2008 Idealab
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
//

using System;
using System.Collections.Generic;
using Mono.Unix;

using Do.Addins;
using Do.Universe;

namespace Cl.ickable
{
	public class ClickableItemSource : AbstractItemSource
	{
		
		public override string Name {
			get { return Catalog.GetString ("Cl.ickable Items"); }
		}
		
		public override string Description { 
			get { return Catalog.GetString ("Usefull Cl.ickable Items"); }
		}
		
		public override string Icon {
			get { return "www"; }
		}

		public override Type[] SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (IItem),
				};
			}
		}

		public override ICollection<IItem> Items
		{
			get {
				return new IItem[] {
					new WebClipsItem (),
				};
			}
		}

	}
}

// GDocsTrashDocument.cs
// 
// Copyright (C) 2009 GNOME Do
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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Mono.Addins;

using Do.Platform;
using Do.Universe;

using Google.GData.Client;
using Google.GData.Documents;

namespace GDocs
{
	public sealed class GDocsTrashDocument : Act
	{
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Delete Document"); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Move a document into Trash at Google Docs"); }
		}
			
		public override string Icon {
			get { return "user-trash";}
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (GDocsAbstractItem); }
		}
		
		public override bool SupportsItem (Item item) 
		{
			return true;
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems) 
		{
			Services.Application.RunOnThread (() => {
				GDocs.TrashDocument (items.First () as GDocsAbstractItem);
			});
			yield break;
		}
	}
}

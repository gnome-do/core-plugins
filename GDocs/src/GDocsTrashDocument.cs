/* GDocsTrashDocument.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
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
 */

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Mono.Unix;


using Do.Universe;

using Google.GData.Client;
using Google.GData.Documents;

namespace GDocs
{
	public sealed class GDocsTrashDocument : Act
	{
		public override string Name {
			get { return Catalog.GetString ("Delete Document"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Move a document into Trash at Google Docs"); }
        }
			
		public override string Icon {
			//get { return "gDocsTrashIcon.png@" + GetType ().Assembly.FullName; }
			get { return "user-trash";}
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] {
					typeof (GDocsItem),	
				};
			}
		}
        
		public override IEnumerable<Type> SupportedModifierItemTypes {
		    get { return null; }
        }		
		
        public bool SupportsItem (Item item) 
        {
			return true;
        }
        
        public bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
        {
			return false;
		}        
		
        public bool ModifierItemsOptional {
            get { return true; }
        }
		
		public IEnumerable<Item> DynamicModifierItemsForItem (Item item) 
        {
            return null;
        }
        
        public IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems) 
        {						
            GDocs.TrashDocument (items.First () as GDocsItem);
			return null;
        }
	}
}

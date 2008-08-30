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
using Mono.Unix;

using Do.Addins;
using Do.Universe;

using Google.GData.Client;
using Google.GData.Documents;

namespace GDocs
{
	public sealed class GDocsTrashDocument : IAction
	{
		public string Name {
			get { return Catalog.GetString ("Delete Document"); }
		}
		
		public string Description {
			get { return Catalog.GetString ("Move a document into Trash at Google Docs"); }
        }
			
		public string Icon {
			get { return "gDocsTrashIcon.png@" + GetType ().Assembly.FullName; }
		}
		
		public Type[] SupportedItemTypes {
			get {
				return new Type[] {
					typeof (GDocsItem),	
				};
			}
		}
        
		public Type[] SupportedModifierItemTypes {
		    get { return null; }
        }		
		
        public bool SupportsItem (IItem item) 
        {
			return true;
        }
        
        public bool SupportsModifierItemForItems (IItem[] items, IItem modItem)
        {
			return false;
		}        
		
        public bool ModifierItemsOptional {
            get { return true; }
        }
		
		public IItem[] DynamicModifierItemsForItem (IItem item) 
        {
            return null;
        }
        
        public IItem[] Perform (IItem[] items, IItem[] modifierItems) 
        {						
            GDocs.TrashDocument (items[0] as GDocsItem);
			return null;
        }

	}
}

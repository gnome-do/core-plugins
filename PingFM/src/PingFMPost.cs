/* PingFMPost.cs
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
 */

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Mono.Unix;
using PingFM.API;


using Do.Universe;

namespace Do.Addins.PingFM
{
	
	public sealed class PingFMPost : Act
	{
		public override string Name {
			get { return Catalog.GetString ("Post via Ping.FM"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Post a text message as microblog or status update to your social network"); }
        }
			
		public override  string Icon {
			get { return "pingfm.png@" + GetType ().Assembly.FullName; }
		}
		
		public override  IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type [] {
					typeof (ITextItem),
				};
			}
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
		    get {
		        return new Type [] {
		            typeof (PingFMServiceItem),
                };
            }
        }
        
		// Although Ping.Fm provides a "default" method, I think the so-called "default" method
		// should be the most frequently used one, which in Do's interface will be the first selected
		// item. This is the reason why ModifierItem is not set as optional.
        public override bool ModifierItemsOptional {
            get {return false; }
        }
        
        public override bool SupportsItem (Item item) 
        {	
			return PingFM.CheckLength ((item as ITextItem).Text);
        }

		public override bool SupportsModifierItemForItems (IEnumerable<Item> item, Item modItem) 
        {       		
            return true;
        }
        
        public override IEnumerable<Item> DynamicModifierItemsForItem (Item item) 
        {
            return null;
        }
        
        public override  IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems) 
        {
            string service = (modifierItems.First () as PingFMServiceItem).Id;
            string body = (items.First () as ITextItem).Text;
			string method = (modifierItems.First () as PingFMServiceItem).Method;
			
			Do.Addins.PingFM.PingFM.Post (service, body, method);      
            return null;
        }
		
	}
}

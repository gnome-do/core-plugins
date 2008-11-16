// TasqueAction.cs
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, see <http://www.gnu.org/licenses/> or 
// write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, 
// Boston, MA 02111-1307 USA
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GConf;

using Do.Universe;
using Tasque;
using Tasque.DBus;
using Tasque.Category.Item;
using Mono.Unix;

namespace Tasque
{
	
        public class TasqueCreateTask : AbstractAction
        {
		
                public TasqueCreateTask ()
                {
                }
		
                public override string Name {
                        get { return Catalog.GetString ("Create a new task"); }
                }
		
                public override string Description {
                        get { return Catalog.GetString ("Create a new task in Tasque"); }
		}
		
		public override string Icon {
			get { return "tasque"; } 
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type[] {
					typeof (ITextItem),
				};
			}
		}
			
		public override bool SupportsItem (IItem item) {
			return true;
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get {
				return new Type[] {
					typeof(TasqueCategoryItem),
				};
			}
		}
			
		public override IEnumerable<IItem> DynamicModifierItemsForItem (IItem item)
		{
			List<IItem> items = new List<IItem> ();
			
			try {
				items = Tasque.GetCategoryItems ();
				return items.ToArray();
			} catch {
				return null;
			}
			
		}
		
		public override bool ModifierItemsOptional {
			get { return true; }
		}

		public override IEnumerable<IItem> Perform ( IEnumerable<IItem> items, IEnumerable<IItem> modifierItems )
		{
			if ( modifierItems.Any ())	
				TextItemPerform (items.First () as ITextItem, modifierItems.First () as TasqueCategoryItem);
			else 
				TextItemPerform (items.First () as ITextItem, new TasqueCategoryItem (""));
			
			return null;
		}
		
		protected IItem[] TextItemPerform (ITextItem item, TasqueCategoryItem category)
		{
			string defaultCategory;

			ArrayList list = new ArrayList ();
			GConf.Client conf = new GConf.Client ();
			TasqueDBus tasque = new TasqueDBus ();
			
			try {
				defaultCategory = conf.Get ("/apps/gnome-do/plugins/tasque/default_category") as string;
			} catch (GConf.NoSuchKeyException) {
				conf.Set ("/apps/gnome-do/plugins/tasque/default_category", "");
				return null;
			}
			
			if (category.Name != "" ) {
				tasque.CreateTask(category.Name, item.Text);
			}
			else if (defaultCategory == String.Empty) {
				
				string[] split = item.Text.Split (':');
				if (split.GetValue(0).ToString() == item.Text) {
					list = tasque.GetCategoryNames ();
					tasque.CreateTask (list[0].ToString(), item.Text);
				}
				else {
					tasque.CreateTask (split.GetValue(0).ToString(), split.GetValue(1).ToString());
				}
			}
			else {
				
				string[] split = item.Text.Split (':');
				
				if (split.GetValue(0).ToString() == item.Text)
					tasque.CreateTask (defaultCategory, item.Text);
				else 
					tasque.CreateTask (split.GetValue(0).ToString(), split.GetValue(1).ToString());
			}
			return null;
		}

	}
}

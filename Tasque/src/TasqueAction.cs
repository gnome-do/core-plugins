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
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using GConf;
using Mono.Unix;

using Do.Universe;

using Tasque.DBus;

namespace Tasque
{

	public class TasqueCreateTask : Act
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
			get { yield return typeof (ITextItem); }
		}

		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { yield return typeof(TasqueCategoryItem); }
		}

		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{
			return Tasque.GetCategoryItems ().OfType<Item> ();
		}

		public override bool ModifierItemsOptional {
			get { return true; }
		}

		public override IEnumerable<Item> Perform ( IEnumerable<Item> items, IEnumerable<Item> modItems )
		{
			if (modItems.Any ())	
				TextItemPerform (items.First () as ITextItem, modItems.First () as TasqueCategoryItem);
			else 
				TextItemPerform (items.First () as ITextItem, new TasqueCategoryItem (""));

			yield break;
		}

		protected Item[] TextItemPerform (ITextItem item, TasqueCategoryItem category)
		{
			string defaultCategory;
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
			} else if (defaultCategory == String.Empty) {

				string[] split = item.Text.Split (':');
				if (split [0] == item.Text) {
					IEnumerable<string> categories = tasque.GetCategoryNames ();
					tasque.CreateTask (categories.First (), item.Text);
				} else {
					tasque.CreateTask (split [0], split [1]);
				}
			} else {

				string[] split = item.Text.Split (':');

				if (split [0] == item.Text)
					tasque.CreateTask (defaultCategory, item.Text);
				else 
					tasque.CreateTask (split [0], split [1]);
			}
			return null;
		}

	}
}

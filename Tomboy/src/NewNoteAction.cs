//  NewNoteAction.cs
//
//  GNOME Do is the legal property of its developers, whose names are too
//  numerous to list here.  Please refer to the COPYRIGHT file distributed with
//  this source distribution.
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
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

using Mono.Addins;

using Do.Universe;
using Do.Platform.Linux;

namespace Tomboy
{	

	public class NewNoteAction : Act, IConfigurable
	{
		
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("New Tomboy Note"); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Create a new Tomboy note."); }
		}
		
		public override string Icon {
			get { return "tomboy"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ITextItem); }
		}
		
		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { yield return typeof (ITextItem); }
		}
		
		public override bool ModifierItemsOptional {
			get { return true; }
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			ITextItem mainItem = items.First () as ITextItem;
			
			ITextItem modItem = null;
			if (modItems.Any ()) {
				modItem = modItems.First () as ITextItem;
			}
			
			string title = null, content = null;
			// Check prefs to see if first text item should be
			// note title or content. The modifier item can provide
			// content or title, respectively.
			if (TomboyConfiguration.TitleFirst) {
				title = mainItem.Text;
				if (modItem != null)
					content = modItem.Text;
			} else {
				content = mainItem.Text;
				if (modItem != null)
					title = modItem.Text;
			}
			
			TomboyDBus tb = new TomboyDBus ();
			// Null values are acceptable here.
			tb.CreateNewNote (title, content);
			
			yield break;
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new TomboyConfiguration ();
		}
	}
}

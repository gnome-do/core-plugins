// GDocsItemSource.cs
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
using System.Threading;
using System.Collections.Generic;

using Mono.Addins;

using Do.Universe;
using Do.Platform;
using Do.Platform.Linux;

namespace GDocs
{
	public sealed class GDocsItemSource : ItemSource, IConfigurable
    {
		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Google Docs"); }
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Indexes your documents stored at Google Docs"); }
		}
		
		public override string Icon {
			get { return "gDocsIcon.png@" + GetType ().Assembly.FullName; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (GDocsAbstractItem); }
		}
		
		public override IEnumerable<Item> Items {
			get { return GDocs.Docs; }
		}
		
		public override IEnumerable<Item> ChildrenOfItem (Item parent)
		{
			yield break;
		}
		
		public override void UpdateItems ()
		{
			Thread updateDocs = new Thread (new ThreadStart (GDocs.UpdateDocs));
			updateDocs.IsBackground = true;
			updateDocs.Start ();
		}
		
		public Gtk.Bin GetConfiguration ()
		{
			return new Configuration ();
		}
	}
}

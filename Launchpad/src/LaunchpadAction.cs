/* LaunchpadAction.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Do.Universe;


using Mono.Unix;

namespace Do.Launchpad
{
	/// <summary>
	/// Given an ITextItem, GoogleSearchAction will search google for the
	/// text contents of the ITextItem.
	/// </summary>
	class LaunchpadAction : Act
	{

		static readonly IEnumerable<LaunchpadItem> LaunchpadItems = new LaunchpadItem [] {
			new LaunchpadAnswerSearchItem(),
			new LaunchpadProjectAnswersItem(),
			new LaunchpadBlueprintsItem(),
			new LaunchpadBlueprintSearchItem(),
			new LaunchpadBlueprintsRegisterItem(),
			new LaunchpadBugNumberItem(),
			new LaunchpadBugReportItem(),
			new LaunchpadPackageBugsItem(),
			new LaunchpadBugSearchItem(),
			new LaunchpadCodeOverviewItem(),
			new LaunchpadRegisterItem(),
			new LaunchpadTranslationSearchItem(),
			new LaunchpadTranslationReleaseItem(),
			new LaunchpadProjectPageItem(),
			new LaunchpadUserPageItem(),
			new LaunchpadUserSearchItem()
		};
			
		public override string Name
		{
			get { return Catalog.GetString ("Launchpad"); }
		}

		public override string Description
		{
			get { return Catalog.GetString ("Launchpad Shortcuts"); }
		}

		public override string Icon
		{ 
			get { return "Launchpad.png@" + GetType ().Assembly.FullName; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ITextItem); }
		}

		public override bool SupportsModifierItemForItems (IEnumerable<Item> items, Item modItem)
		{
			return (modItem as LaunchpadItem).SupportsItems (items.OfType<ITextItem> ());
		}

		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { yield return typeof (LaunchpadItem);}
		}

		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{
			return LaunchpadItems.OfType<Item> ();
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			foreach (LaunchpadItem lp in modItems)
				lp.Perform (items.OfType<ITextItem> ());
			
			yield break;
		}
	}

	/// <summary>
	/// LaunchpadItems are used as modifier items to LaunchpadAction, and they
	/// are responsible for telling LaunchpadAction whether they support a
	/// given Item, as well as implementing the actual action on a given Item.
	///
	/// They are meant to behave just like Actions, but they need to be Items
	/// to be listed in the right-hand box.
	/// </summary>
	public abstract class LaunchpadItem : Item
	{
		public abstract bool SupportsItems (IEnumerable<ITextItem> items);
		public abstract void Perform (IEnumerable<ITextItem> items);
	}
}

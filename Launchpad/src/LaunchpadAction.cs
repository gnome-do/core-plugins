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
using System.Text.RegularExpressions;
using Do.Universe;

using Do.Addins;

namespace Do.Launchpad
{
	/// <summary>
	/// Given an ITextItem, GoogleSearchAction will search google for the
	/// text contents of the ITextItem.
	/// </summary>
	class LaunchpadAction : AbstractAction
	{
		public LaunchpadAction ()
		{
		}

		public override string Name
		{
			get { return "Launchpad"; }
		}

		public override string Description
		{
			get { return "Launchpad Shortcuts"; }
		}

		public override string Icon
		{ 
			get { return LaunchpadIcons.Instance.GetIconPath("Launchpad.png"); }
		}

		public override Type[] SupportedItemTypes
		{
			get
			{
				return new Type[] {
					typeof (ITextItem)
				};
			}
		}

    public override bool ModifierItemsOptional { get { return false; } }

		public override bool SupportsModifierItemForItems (IItem[] items, IItem modItem)
		{
			return (modItem as LaunchpadItem).SupportsItems(items);
		}

		public override Type[] SupportedModifierItemTypes
		{
			get
			{
				return new Type[] {
					typeof(LaunchpadAnswerSearchItem),
						typeof(LaunchpadProjectAnswersItem),
						typeof(LaunchpadBlueprintsItem),
						typeof(LaunchpadBlueprintSearchItem),
						typeof(LaunchpadBlueprintsRegisterItem),
						typeof(LaunchpadBugNumberItem),
						typeof(LaunchpadBugReportItem),
						typeof(LaunchpadPackageBugsItem),
						typeof(LaunchpadBugSearchItem),
						typeof(LaunchpadCodeBrowseItem),
						typeof(LaunchpadCodeOverviewItem),
						typeof(LaunchpadRegisterItem),
						typeof(LaunchpadTranslationSearchItem),
						typeof(LaunchpadTranslationReleaseItem)
				};
			}
		}

		public override IItem[] DynamicModifierItemsForItem (IItem item)
		{
			return new IItem[] {
				new LaunchpadAnswerSearchItem(),
					new LaunchpadProjectAnswersItem(),
					new LaunchpadBlueprintsItem(),
					new LaunchpadBlueprintSearchItem(),
					new LaunchpadBlueprintsRegisterItem(),
					new LaunchpadBugNumberItem(),
					new LaunchpadBugReportItem(),
					new LaunchpadPackageBugsItem(),
					new LaunchpadBugSearchItem(),
					new LaunchpadCodeBrowseItem(),
					new LaunchpadCodeOverviewItem(),
					new LaunchpadRegisterItem(),
					new LaunchpadTranslationSearchItem(),
					new LaunchpadTranslationReleaseItem()
			};
		}

    public override bool SupportsItem (IItem item)
    {
        return true;
    }

		public override IItem[] Perform (IItem[] items, IItem[] modItems) {
			if (modItems.Length > 0) {
				(modItems[0] as LaunchpadItem).Perform(items[0]);
			}
			return null;
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
	interface LaunchpadItem : IItem
	{
		bool SupportsItems(IItem[] items);
		void Perform (IItem item);
	}
}

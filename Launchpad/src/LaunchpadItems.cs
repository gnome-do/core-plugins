/* LaunchpadItems.cs
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

namespace Launchpad
{

	class LaunchpadItems
	{

		public static readonly IEnumerable<LaunchpadItem> Items = new [] {
			new LaunchpadItem (
				"Answers Search",
				"Search for Answers on Launchpad",
				"LaunchpadAnswers.png",
				"https://answers.launchpad.net/questions/+questions?field.search_text={0}"),
			new LaunchpadItem (
				"Project Answers",
				"Answers for a particular project on Launchpad",
				"LaunchpadAnswers.png",
				"https://answers.launchpad.net/{0}"),
			new LaunchpadItem (
				"Project Blueprints",
				"Show blueprints for specified project on Launchpad",
				"LaunchpadBlueprints.png",
				"https://blueprints.launchpad.net/{0}"),
			new LaunchpadItem (
				"Blueprint Search",
				"Search for blueprints on Launchpad",
				"LaunchpadBlueprints.png",
				"https://blueprints.launchpad.net/?searchtext={0}"),
			new LaunchpadItem (
				"Register Blueprints",
				"Register a blueprint on Launchpad",
				"LaunchpadBlueprints.png",
				"https://blueprints.launchpad.net/specs/+new"),
			new LaunchpadItem (
				"Bug Number",
				"Find bug by number",
				"LaunchpadBugs.png",
				"https://bugs.launchpad.net/bugs/{0}"),
			new LaunchpadItem (
				"Bug Report",
				"Report a bug at Launchpad",
				"LaunchpadBugs.png",
				"https://bugs.launchpad.net/bugs/+filebug/{0}"),
			new LaunchpadItem (
				"Project Bugs",
				"Show open bugs in a project at Launchpad",
				"LaunchpadBugs.png",
				"https://bugs.launchpad.net/{0}"),
			new LaunchpadItem (
				"Bug Search",
				"Search for bugs at Launchpad",
				"LaunchpadBugs.png",
				"https://bugs.launchpad.net/bugs/+bugs?field.searchtext={0}"),
			new LaunchpadItem (
				"Code Browse",
				"Browse Code For Launchpad Project",
				"LaunchpadCode.png",
				"https://codebrowse.launchpad.net/~vcs-imports/{0}/main/files"),
			new LaunchpadItem (
				"Code Overview",
				"Browse project code at Launchpad",
				"LaunchpadCode.png",
				"https://code.launchpad.net/{0}"),
			new LaunchpadItem (
				"Translation Search",
				"Search for Translations in Launchpad",
				"LaunchpadTranslations.png",
				"https://translations.launchpad.net/projects/?text={0}"),
			new LaunchpadItem (
				"Release Translations",
				"Translations for Ubuntu Release Name",
				"LaunchpadTranslations.png",
				"https://translations.lauchpad.net/ubuntu/{0}/+translations"),
			new LaunchpadItem (
				"Project Page",
				"Go to project's page in launchpad",
				"LaunchpadRegister.png",
				"https://launchpad.net/{0}"),
			new LaunchpadItem (
				"User Page",
				"Go to user's page in Launchpad",
				"LaunchpadUser.png",
				"https://launchpad.net/~{0}"),
			new LaunchpadItem (
				"User Search",
				"Search for a user in Launchpad",
				"LaunchpadUser.png",
				"https://launchpad.net/people?name={0}&searchfor=peopleonly"),
		};
			
	}
}


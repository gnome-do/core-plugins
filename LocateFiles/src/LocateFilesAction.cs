/* LocateFilesAction.cs
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
using System.IO; 
using System.Linq;
using System.Collections.Generic;
using Mono.Addins;

using Do.Platform;
using Do.Universe;
using Do.Universe.Common;

namespace Locate
{
	public class LocateFilesAction : Act
	{
		string Error = AddinManager.CurrentLocalizer.GetString ("Locate found 0 files for: ");
		
		public override string Name
		{
			get { return AddinManager.CurrentLocalizer.GetString ("Locate Files"); }
		}
		
		public override string Description
		{
			get { return AddinManager.CurrentLocalizer.GetString ("Search your filesystem using locate."); }
		}
		
		public override string Icon
		{
			get { return "search"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes
		{
			get { yield return typeof (ITextItem); }
		}
		
		string Arguments {
			get { return Services.Preferences.Get<LocateFilesAction> ().Get<string> ("Arguments", "-b -i"); }
		}
		
		bool AllowHidden {
			get { return Services.Preferences.Get<LocateFilesAction> ().Get<bool> ("AllowHidden", true); }
		}
		
		uint MaxResults {
			get { return (uint) Services.Preferences.Get<LocateFilesAction> ().Get<int> ("MaxResults", 500); }
		}
		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modifierItems)
		{
			List<Item> files;
			System.Diagnostics.Process locate;
			string query;
				
			files = new List<Item> ();
			query = (items.First () as ITextItem).Text;

			if (string.IsNullOrEmpty(query)) {
				Services.Notifications.Notify(AddinManager.CurrentLocalizer.GetString ("Locate Files"),
					AddinManager.CurrentLocalizer.GetString ("No text provided for searching."));
				yield break;
			}
			
			locate = new System.Diagnostics.Process ();
			locate.StartInfo.FileName = "locate";
			locate.StartInfo.Arguments = string.Format ("{0} -l {1} \"{2}\"", Arguments, MaxResults, query);
			locate.StartInfo.RedirectStandardOutput = true;
			locate.StartInfo.RedirectStandardError = true;
			locate.StartInfo.UseShellExecute = false;
			locate.Start ();

			string path;
			uint results = 0;
			query = query.ToLower ();
			while (null != (path = locate.StandardOutput.ReadLine ())) {
				// Disallow hidden directories in the absolute path.
				// This gets rid of messy .svn directories and their contents.
				if (!AllowHidden && Path.GetDirectoryName (path).Contains ("/."))
					continue;

				results++;
				files.Add (Services.UniverseFactory.NewFileItem (path) as Item);
			}
			
			if (results > 0) {
				files.Sort (new IFileItemNameComparer (query));
				foreach (Item file in files)
					yield return file;
			} else {
				Services.Notifications.Notify (AddinManager.CurrentLocalizer.GetString ("Locate Files"),
					Error + query);
				yield break;
			}
		}

		// Order files by (A) position of query in the file name and
		// (B) by name length.
		private class IFileItemNameComparer : IComparer<Item>
		{
			string query;

			public IFileItemNameComparer (string query)
			{
				this.query = query.ToLower ();
			}

			public int Compare (Item a, Item b)
			{
				string a_name_lower, b_name_lower;
				int a_score, b_score;

				a_name_lower = (a as IFileItem).Path;
				a_name_lower = Path.GetFileName (a_name_lower).ToLower (); 
				b_name_lower = (b as IFileItem).Path;
				b_name_lower = Path.GetFileName (b_name_lower).ToLower (); 

				a_score = a_name_lower.IndexOf (query);
				b_score = b_name_lower.IndexOf (query);

				if (a_score == b_score)
					return a_name_lower.Length - b_name_lower.Length;

				return a_score - b_score;
			}
		}
	}
}

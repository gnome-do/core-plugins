// AbstractFileAction.cs
//
// GNOME Do is the legal property of its developers. Please refer to the
// COPYRIGHT file distributed with this source distribution.
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
//

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using Mono.Unix;

using Do.Universe;
using Do.Platform;

namespace Do.FilesAndFolders
{
	
	public abstract class AbstractFileAction : Act
	{

		protected const int MaxPathLength = 256;

		// How much time should these PerformWait pause for?
		// Right now, we do not pause, because it will annoy the user if the Do
		// interface remains exposed and frozen.
		static readonly TimeSpan PerformWaitSpan = new TimeSpan (0, 0, 0);
		
		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (ITextItem);
				yield return typeof (IFileItem);
			}
		}

		public override IEnumerable<Type> SupportedModifierItemTypes {
			get {
				yield return typeof (ITextItem);
				yield return typeof (IFileItem);
			}
		}

		protected virtual bool SupportsItem (IFileItem item)
		{
			return item.Path.Length < MaxPathLength && item.Exists ();
		}

		protected virtual bool SupportsItem (ITextItem item)
		{
			string path = item.Text.Replace ("~", Plugin.ImportantFolders.UserHome);
			return path.Length < MaxPathLength && (File.Exists (path) || Directory.Exists (path));
		}
		
		public override bool SupportsItem (Item item)
		{
			return 
				(item is ITextItem && SupportsItem (item as ITextItem)) ||
				(item is IFileItem && SupportsItem (item as IFileItem));
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			foreach (Item source in items) {
				if (modItems.Any ()) {
					foreach (Item destination in modItems)
						foreach (Item result in Perform (source, destination))
							yield return result;
				} else {
					foreach (Item result in Perform (source))
						yield return result;
				}
			}
		}

		protected string GetPath (Item item)
		{
			if (item is IFileItem)
				return GetPath (item as IFileItem);
			if (item is ITextItem)
				return GetPath (item as ITextItem);
			throw new Exception ("Inappropriate Item type.");
		}

		protected string GetPath (IFileItem item)
		{
			return item.Path;
		}

		protected string GetPath (ITextItem item)
		{
			return item.Text.Replace ("~", Plugin.ImportantFolders.UserHome);
		}

		protected virtual IEnumerable<Item> Perform (string source, string destination)
		{
			yield break;
		}

		protected virtual IEnumerable<Item> Perform (Item source, Item destination)
		{
			return Perform (GetPath (source), GetPath (destination));
		}

		protected virtual IEnumerable<Item> Perform (string source)
		{
			yield break;
		}

		protected virtual IEnumerable<Item> Perform (Item source)
		{
			return Perform (GetPath (source));
		}

		protected void PerformOnThread (Action action)
		{
			new Thread ((ThreadStart) delegate {
				action ();
			}).Start ();
		}

		protected void PerformWait ()
		{
			Thread.Sleep (PerformWaitSpan);
		}

		protected string Move (string source, string destination)
		{
			Process mv = Process.Start ("mv", source + " " + destination);
			mv.WaitForExit ();
			return Path.Combine (destination, Path.GetFileName (source));
		}

		protected string Copy (string source, string destination)
		{
			Process cp = Process.Start ("cp -r", source + " " + destination);
			cp.WaitForExit ();
			return Path.Combine (destination, Path.GetFileName (source));
		}

	}

}


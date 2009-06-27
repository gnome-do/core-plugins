// RunInTerminalAction.cs
// 
// GNOME Do is the legal property of its developers, whose names are too
// numerous to list here.  Please refer to the COPYRIGHT file distributed with
// this source distribution.
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
using System.Collections.Generic;
using System.Diagnostics;

using Mono.Addins;

using Do.Universe;
using Do.Platform;

namespace GNOME.Terminal
{
	public class RunInTerminalAction : Act
	{

		public RunInTerminalAction()
		{
		}

		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Run in Terminal"); }
		}

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Runs a command in GNOME Terminal."); }
		}

		public override string Icon {
			get { return "gnome-terminal"; }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (ITextItem);
				yield return typeof (IFileItem);
			}
		}

		public override bool SupportsItem (Item item)
		{
			return
				(item is ITextItem && SupportsItem (item as ITextItem)) ||
				(item is IFileItem && SupportsItem (item as IFileItem));
		}

		bool SupportsItem (ITextItem command)
		{
			return Services.Environment.IsExecutable (command.Text);
		}

		bool SupportsItem (IFileItem command)
		{
			return Services.Environment.IsExecutable (command.Path);
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			items.OfType<ITextItem> ().ForEach (Perform);
			items.OfType<IFileItem> ().ForEach (Perform);
			yield break;
		}

		void Perform (ITextItem command)
		{
			Perform (command.Text);
		}

		void Perform (IFileItem command)
		{
			Perform (command.Path);
		}

		void Perform (string command)
		{
			try {
				Process.Start ("gnome-terminal", "-x " + command);
			} catch (Exception e) {
				Log.Error ("Could not open gnome-terminal with command {0}: {1}", command, e.Message );
				Log.Debug (e.StackTrace);
			}
		}
	}
}

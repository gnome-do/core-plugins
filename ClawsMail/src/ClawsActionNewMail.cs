// ClawsActionNewMail.cs 
// User: Karol Będkowski at 22:16 2008-10-14
//
// Copyright Karol Będkowski 2008

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
using System.Linq;
using System.Collections.Generic;
using Mono.Unix;
using Do.Universe;
using Do.Platform;

namespace Claws
{
	
	/// <summary>
	/// New mail action (with or without email).
	/// </summary>
	public class ClawsActionNewMail : Act {

		const string ClawsMailCommand = "claws-mail";
		const string ComposeCommandParams = "--compose ";
		const string EmailsSeparator = ",";
		

		#region std properties
		
		public override string Name {
			get { return Catalog.GetString ("Compose Email"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Compose a new email with ClawsMail"); }
		}
		
		public override string Icon {
			get { return "mail_new"; }
		}
		
		public override IEnumerable<Type> SupportedItemTypes {
			get { 
				yield return typeof (ContactItem);
				yield return typeof (ITextItem);
			}
		}

		# endregion

		
		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			string emails = string.Join(EmailsSeparator, GetEmails (items).ToArray ());							
			StartClawsNewMail (emails);
			yield break;	
		}


		static IEnumerable<string> GetEmails (IEnumerable<Item> items) {
			foreach (Item item in items) {
				ContactItem contactItem = item as ContactItem;
				if (contactItem != null) {
					yield return contactItem.AnEmailAddress;
				} else {
					ITextItem textItem = item as ITextItem;
					if (textItem != null) {
						yield return textItem.Text;
					}
				}
			}
			yield break;
		}

		static void StartClawsNewMail(string emails) {
			try {
				// Log.Debug("ClawsActionNewMail.StartClawsNewMail: {0} {1}", clawsMailCommand, composeCommandParams + email);
				System.Diagnostics.Process.Start (ClawsMailCommand, ComposeCommandParams + emails);
			} catch (Exception e) {
				Log.Error("ClawsActionNewMail.StartClawsNewMail {0} {1} {2}: {3}", ClawsMailCommand, ComposeCommandParams, 
				          emails, e.Message);
				Log.Debug("ClawsActionNewMail.StartClawsNewMail {0}", e.StackTrace);
			}
		}
	}
}

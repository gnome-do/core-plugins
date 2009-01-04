// ClawsActionReceiveAll.cs 
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
using Mono.Unix;

namespace Claws
{
	
	/// <summary>
	/// Get all mail.
	/// </summary>
	public class ClawsActionReceiveAll : ClawsActionBase {
		
		protected override string Command {
			get { return "claws-mail --receive-all"; }
		}
		
		public override string Name {
			get { return Catalog.GetString ("Receive all"); }
		}
		
		public override string Description {
			get { return Catalog.GetString ("Receive Mail on all Accounts"); }
		}
		
		public override string Icon {
			get { return "mail-send-receive"; }
		}
	}
}

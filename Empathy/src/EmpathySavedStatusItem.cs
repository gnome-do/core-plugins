//  EmpathySavedStatusItem.cs
//  
//  Author:
//       Xavier Calland <xavier.calland@gmail.com>
//  
//  Copyright (c) 2010 
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Text.RegularExpressions;

using Do.Universe;

using Telepathy;

namespace EmpathyPlugin
{
	public  class EmpathySavedStatusItem : EmpathyStatusItem
	{
		public EmpathySavedStatusItem (ConnectionPresenceType status, string message) : base(status)
		{
			Message = message;
		}

		
		public override string Name {
			get { return StripHTML(Message); }
		}
		
		public string Message { get; private set; }
		
		string StripHTML (string message)
		{
			return Regex.Replace(message, @"<(.|\n)*?>", string.Empty);
		}
	}

}

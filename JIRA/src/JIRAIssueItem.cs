//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using Do.Universe;

namespace JIRA
{
	/// <summary>
	/// Class representing a JIRA Issue
	/// </summary>
	public class JIRAIssueItem : Item
	{
		/// <summary>
		/// The base JIRA installation url
		/// </summary>
		private string _baseUrl;
		
		/// <summary>
		/// The issue code. ie. NET-141
		/// </summary>
		private string _issueCode;
		
		/// <summary>
		/// The title of the bug. ie. Problem with Component X under condition Y
		/// </summary>
		private string _title;
		
		/// <summary>
		/// JIRA Status Code. ie. 1= Open, 2=In Progress, 5=Resolved, 6=Closed
		/// </summary>
		private int _status;
		
		/// Known JIRA Status codes
		public const int STATUS_RESOLVED= 5;
		public const int STATUS_CLOSED= 6;
		
		public JIRAIssueItem( string issueCode, string baseUrl )
		{
			_issueCode= issueCode;
			_baseUrl= baseUrl;
		}

		public override string Description 
		{
			get { return _title; }
		}

		public void setDescription(string newDescription) {
			_title = newDescription;
		}
		
		public int Status
		{
			get { return _status; }
			set { _status= value; }
		}
		
		public bool IsClosed
		{
			get { return _status==STATUS_CLOSED || _status==STATUS_RESOLVED; }
		}
		
		public override string Name { get { return _issueCode; } }
		public override string Icon { get { return "jira.png@"+GetType().Assembly.FullName; } }
		public string Url { get { return _baseUrl+"/browse/"+_issueCode; } }	
	}
}

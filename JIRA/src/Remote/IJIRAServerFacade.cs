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
//
using System;
using Atlassian;
using System.Collections.Generic;

namespace JIRA.Remote
{
	/// <summary>
	/// Interface holding all the various methods I need for the JIRA plugin, I'll expand
	/// this as I need more methods.
	/// </summary>
	public interface IJIRAServerFacade
	{
		/// <summary>
		/// Return a representation of a JIRA project identified by the string key. ie. NET
		/// </summary>
		RemoteProject getProjectByKey( string projectKey ); 
		
		/// <summary>
		/// Get the available statuses for the given JIRA install
		/// </summary>
		IList<RemoteStatus> getStatuses();
		
		/// <summary>
		/// Get a list of issues which have the given status id set
		/// </summary>
		IList<RemoteIssue> GetAllIssuesWithStatus( IList<long> projectIds, IList<int> statusIds );
		
		/// <summary>
		/// Get a list of recently updated issues (within the last hour)
		/// </summary>
		IList<RemoteIssue> GetRecentlyUpdatedIssues( IList<long> projectIds );
	}
}

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
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Atlassian;

namespace JIRA.Remote
{
	/// <summary>
	/// Interface holding all the various methods I need for the JIRA plugin, I'll expand
	/// this as I need more methods.
	/// </summary>
	public class JIRAServerFacade : IJIRAServerFacade
	{
		private string _baseUrl;
		private string _username;
		private string _password;
		
		private JIRARssClient _rssClient;
		
		/// <summary>
		/// Constructor
		/// </summary>
		public JIRAServerFacade( string baseUrl, string username, string password )
		{
			_baseUrl= baseUrl;
			_username= username;
			_password= password;
			
			_rssClient= new JIRARssClient( baseUrl );
			_rssClient.Username= username;
			_rssClient.Password= password;
			
			// Allow self-signed certificates
			ServicePointManager.CertificatePolicy= new MyCertificatePolicy();
		} 
		
		private JiraSoapServiceService getServiceEndPoint()
		{
			JiraSoapServiceService service= new JiraSoapServiceService();
			service.Url= _baseUrl+"/rpc/soap/jirasoapservice-v2";
			return service;			
		}
		
		
		/// <summary>
		/// Return a representation of a JIRA project identified by the string key. ie. NET
		/// </summary>
		public RemoteProject getProjectByKey( string projectKey )
		{
			JiraSoapServiceService service= getServiceEndPoint();
			string token= service.login( _username, _password );
			
			return service.getProjectByKey( token, projectKey );
		}
		
		/// <summary>
		/// Get the available statuses for the given JIRA install
		/// </summary>
		public IList<RemoteStatus> getStatuses()
		{
			JiraSoapServiceService service= getServiceEndPoint();
			string token= service.login( _username, _password );
			
			return new List<RemoteStatus>( service.getStatuses( token ) );
		}
		
		/// <summary>
		/// Get a list of issues which have the given status id set
		/// </summary>
		public IList<RemoteIssue> GetAllIssuesWithStatus( IList<long> projectIds, IList<int> statusIds )
		{
			return _rssClient.GetAllIssuesWithStatus( projectIds, statusIds );
		}
		
		/// <summary>
		/// Get a list of recently updated issues (within the last hour)
		/// </summary>
		public IList<RemoteIssue> GetRecentlyUpdatedIssues( IList<long> projectIds )
		{
			return _rssClient.GetRecentlyUpdatedIssues( projectIds );
		}
	}
	
	
	/// <summary>
	/// Open Policy for not worrying about self-signed JIRA instances
	/// </summary>
	public class MyCertificatePolicy : ICertificatePolicy
	{
		public bool CheckValidationResult( ServicePoint sp, X509Certificate cert, WebRequest chain, int errors )
		{
			return true;
		}
	}
}

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
using System.Net;
using System.Net.Security;
using System.Text;
using System.Xml;
using System.Web;
using System.Collections.Generic;
using Atlassian;

namespace JIRA.Remote
{
	/// <summary>
	/// Class which makes queries to JIRA via the RSS interface
	/// </summary>
	class JIRARssClient
	{
		/// <summary>
		/// The JIRA base url
		/// ie. http://issues.apache.org/jira
		/// </summary>
		private string _url;
		
		private string _username;
		private string _password;
		
		/// <summary>
		/// Maximum results we can retrieve in a query
		/// </summary>
		private int _maxResults= 10000;

		
		public JIRARssClient( string url )
		{
			_url= url;
		}
		
		public string Username
		{
			set { _username= value; }
		}
		
		public string Password
		{
			set { _password= value; }
		}
		
		/// <summary>
		/// Get a list of all issues, open or closed
		/// </summary>
		public IList<RemoteIssue> GetAllIssues( IList<long> projectIds )
		{
			return DoQuery( GetBaseQueryUrl( projectIds ) );
		}
		
		/// <summary>
		/// Get a list of all issues that have the given status ids
		/// </summary>
		public IList<RemoteIssue> GetAllIssuesWithStatus( IList<long> projectIds, IList<int> statusIds )
		{
			StringBuilder sb= new StringBuilder();
			sb.Append( GetBaseQueryUrl( projectIds ) );
			
			foreach( int statusId in statusIds )
			{
				sb.Append( "&status=" );
				sb.Append( statusId );
			}
			return DoQuery( sb.ToString() );
		}
		
		/// <summary>
		/// Get a list of all issues (open and closed), that have been updated in the last hour
		/// </summary>
		public IList<RemoteIssue> GetRecentlyUpdatedIssues( IList<long> projectIds )
		{
			return DoQuery( GetBaseQueryUrl( projectIds )+"&updated%3Aprevious=-1h" );
		}
		
		/// <summary>
		/// Do the actual query by converting the rss into a list of issue items
		/// </summary>
		private IList<RemoteIssue> DoQuery( String queryUrl )
		{
			List<RemoteIssue> result= new List<RemoteIssue>();			
			
			// Make the request
			HttpWebRequest request= WebRequest.Create( queryUrl ) as HttpWebRequest;
			
			// Handle credentials
			if( _username!=null && _password!=null )
			{
				CredentialCache creds= new CredentialCache();
				creds.Add( new Uri( queryUrl ), "Basic", new NetworkCredential( _username, _password ) );
				
				request.Credentials= creds;
			}
			
			// Console.WriteLine( "Query: "+queryUrl );
			
			HttpWebResponse response= request.GetResponse() as HttpWebResponse;
			
			// Parse the xml from the get all open issues query
			XmlDocument doc= new XmlDocument();
			doc.Load( response.GetResponseStream() );
						
			foreach( XmlNode itemNode in doc.SelectNodes( "//rss/channel/item" ) )
			{
				RemoteIssue issue= new RemoteIssue();
				issue.key= itemNode.SelectSingleNode( "key" ).InnerText;
				issue.summary= itemNode.SelectSingleNode( "title" ).InnerText;
				issue.status= itemNode.SelectSingleNode( "status" ).Attributes[ "id" ].InnerText;
				
				result.Add( issue );
			}
			
			return result;
		}
		
		/// <summary>
		/// Use the RSS End-point to find issues that are resolved/closed for the given project ids 
		/// </summary>
		private string GetBaseQueryUrl( IList<long> projectIds )
		{
			StringBuilder sb= new StringBuilder();
			sb.Append( _url );
			sb.Append( "/sr/jira.issueviews:searchrequest-xml/temp/SearchRequest.xml?" );

			// Append the request to authenticate
			if( _username!=null && _password!=null )
			{
				sb.Append( "os_authType=basic" );
			}
			
			// Add each of the project ids to the query
			foreach( long projectId in projectIds )
			{
				sb.Append( "&pid=" );
				sb.Append( projectId );
			}
			
			// Descending sort by issue id
			sb.Append( "&sorter/field=issuekey" );
			sb.Append( "&sorter/order=DESC" );
			
			// Cap on the max results...
			sb.Append( "&tempMax=" );
			sb.Append( _maxResults );
			
			return sb.ToString();
		}
	}
}

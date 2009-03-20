using System;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Text;
using System.Collections;
using System.Collections.Specialized;

namespace RtmNet
{
	/// <summary>
	/// The main Rtm class.
	/// </summary>
	/// <remarks>
	/// Create an instance of this class and then call its methods to perform methods on Rtm.
	/// </remarks>
	/// <example>
	/// <code>RtmNet.Rtm Rtm = new RtmNet.Rtm();
	/// User user = Rtm.PeopleFindByEmail("cal@iamcal.com");
	/// Console.WriteLine("User Id is " + u.UserId);</code>
	/// </example>
	//[System.Net.WebPermission(System.Security.Permissions.SecurityAction.Demand, ConnectPattern="http://www.Rtm.com/.*")]
	public class Rtm
	{
#region [ Private Variables ]
		private const string AuthUrl = "http://api.rememberthemilk.com/services/auth/";
		private const string BaseUrl = "http://api.rememberthemilk.com/services/rest";

		private string apiKey;
		private string apiToken;
		private string sharedSecret;
		private int timeout = 30000;
		private const string UserAgent = "Mozilla/4.0 RtmNet API (compatible; MSIE 6.0; Windows NT 5.1)";
		private string lastRequest;
		private string lastResponse;

		private WebProxy proxy;// = WebProxy.GetDefaultProxy();

#endregion

#region [ Public Properties ]
		/// <summary>
		/// Get or set the API Key to be used by all calls. API key is mandatory for all 
		/// calls to Rtm.
		/// </summary>
		public string ApiKey 
		{ 
			get { return apiKey; } 
			set { apiKey = (value==null||value.Length==0?null:value); }
		}

		/// <summary>
		/// API shared secret is required for all calls that require signing, which includes
		/// all methods that require authentication, as well as the actual Rtm.auth.* calls.
		/// </summary>
		public string ApiSecret
		{
			get { return sharedSecret; }
			set { sharedSecret = (value==null||value.Length==0?null:value); }
		}

		/// <summary>
		/// The API token is required for all calls that require authentication.
		/// A <see cref="RtmException"/> will be raised by Rtm if the API token is
		/// not set when required.
		/// </summary>
		/// <remarks>
		/// It should be noted that some methods will work without the API token, but
		/// will return different results if used with them (such as group pool requests, 
		/// and results which include private pictures the authenticated user is allowed to see
		/// (their own, or others).
		/// </remarks>
		[Obsolete("Renamed to AuthToken to be more consistent with the Rtm API")]
			public string ApiToken 
			{
				get { return apiToken; }
				set { apiToken = (value==null||value.Length==0?null:value); }
			}

		/// <summary>
		/// The authentication token is required for all calls that require authentication.
		/// A <see cref="RtmException"/> will be raised by Rtm if the authentication token is
		/// not set when required.
		/// </summary>
		/// <remarks>
		/// It should be noted that some methods will work without the authentication token, but
		/// will return different results if used with them (such as group pool requests, 
		/// and results which include private pictures the authenticated user is allowed to see
		/// (their own, or others).
		/// </remarks>
		public string AuthToken 
		{
			get { return apiToken; }
			set { apiToken = (value==null||value.Length==0?null:value); }
		}

		/// <summary>
		/// The default service to use for new Rtm instances
		/// </summary>
		public static SupportedService DefaultService
		{
			get { return SupportedService.Rtm; }
		}

		/// <summary>
		/// The current service that the Rtm API is using.
		/// </summary>
		public SupportedService CurrentService
		{
			get { return SupportedService.Rtm; } 
		}

		/// <summary>
		/// Internal timeout for all web requests in milliseconds. Defaults to 30 seconds.
		/// </summary>
		public int HttpTimeout
		{
			get { return timeout; } 
			set { timeout = value; }
		}

		/// <summary>
		/// Checks to see if a shared secret and an api token are stored in the object.
		/// Does not check if these values are valid values.
		/// </summary>
		public bool IsAuthenticated
		{
			get { return (sharedSecret != null && apiToken != null); }
		}

		/// <summary>
		/// Returns the raw XML returned from the last response.
		/// Only set it the response was not returned from cache.
		/// </summary>
		public string LastResponse
		{
			get { return lastResponse; }
		}

		/// <summary>
		/// Returns the last URL requested. Includes API signing.
		/// </summary>
		public string LastRequest
		{
			get { return lastRequest; }
		}

		/// <summary>
		/// You can set the <see cref="WebProxy"/> or alter its properties.
		/// It defaults to your internet explorer proxy settings.
		/// </summary>
		public WebProxy Proxy { get { return proxy; } set { proxy = value; } }
#endregion

#region [ Constructors ]

		/// <summary>
		/// Constructor loads configuration settings from app.config or web.config file if they exist.
		/// </summary>
		public Rtm()
		{
		}

		/// <summary>
		/// Create a new instance of the <see cref="Rtm"/> class with no authentication.
		/// </summary>
		/// <param name="apiKey">Your Rtm API Key.</param>
		public Rtm(string apiKey) : this(apiKey, "", "")
		{
		}

		/// <summary>
		/// Creates a new instance of the <see cref="Rtm"/> class with an API key and a Shared Secret.
		/// This is only useful really useful for calling the Auth functions as all other
		/// authenticationed methods also require the API Token.
		/// </summary>
		/// <param name="apiKey">Your Rtm API Key.</param>
		/// <param name="sharedSecret">Your Rtm Shared Secret.</param>
		public Rtm(string apiKey, string sharedSecret) : this(apiKey, sharedSecret, "")
		{
		}

		/// <summary>
		/// Create a new instance of the <see cref="Rtm"/> class with the email address and password given
		/// </summary>
		/// <param name="apiKey">Your Rtm API Key</param>
		/// <param name="sharedSecret">Your Rtm Shared Secret.</param>
		/// <param name="token">The token for the user who has been authenticated.</param>
		public Rtm(string apiKey, string sharedSecret, string token) : this()
		{
			this.apiKey = apiKey;
			this.sharedSecret = sharedSecret;
			this.apiToken = token;
		}
#endregion

#region [ Private Methods ]
		/// <summary>
		/// A private method which performs the actual HTTP web request if
		/// the details are not found within the cache.
		/// </summary>
		/// <param name="url">The URL to download.</param>
		/// <param name="variables">The query string parameters to be added to the end of the URL.</param>
		/// <returns>A <see cref="RtmNet.Response"/> object.</returns>
		/// <remarks>If the final length of the URL would be greater than 2000 characters 
		/// then they are sent as part of the body instead.</remarks>
		private string DoGetResponse(string url, string variables)
		{
			HttpWebRequest req = null;
			HttpWebResponse res = null;

			if( variables.Length < 2000 )
			{
				url += "?" + variables;
				variables = "";
			}

			// Initialise the web request
			req = (HttpWebRequest)HttpWebRequest.Create(url);
			req.Method = "POST";

			if (req.Method == "POST") req.ContentLength = variables.Length;

			req.UserAgent = UserAgent;
			if( Proxy != null ) req.Proxy = Proxy;
			req.Timeout = HttpTimeout;
			req.KeepAlive = false;
			if (variables.Length > 0)
			{
				req.ContentType = "application/x-www-form-urlencoded";
				StreamWriter sw = null;
				try
				{
					sw = new StreamWriter(req.GetRequestStream());
					sw.Write(variables);
					sw.Close();
				}
				catch(WebException ex)
				{
					throw new RtmWebException(ex.Message, ex);
				}
				finally
				{
					if (sw != null)
						sw.Close();
				}
			}
			else
			{
				// This is needed in the Compact Framework
				// See for more details: http://msdn2.microsoft.com/en-us/library/1afx2b0f.aspx
				try {
					req.GetRequestStream().Close();
				} catch (WebException ex) {
					throw new RtmWebException (ex.Message, ex);
				}
			}

			try
			{
				// Get response from the internet
				res = (HttpWebResponse)req.GetResponse();
			}
			catch(WebException ex)
			{
				if( ex.Status == WebExceptionStatus.ProtocolError )
				{
					HttpWebResponse res2 = (HttpWebResponse)ex.Response;
					if( res2 != null )
					{
						throw new RtmWebException(String.Format("HTTP Error {0}, {1}", (int)res2.StatusCode, res2.StatusDescription), ex);
					}
				}
				throw new RtmWebException(ex.Message, ex);
			}

			string responseString = string.Empty;

			using (StreamReader sr = new StreamReader(res.GetResponseStream()))
			{
				responseString = sr.ReadToEnd();
			}

			return responseString;
		}

#endregion

#region [ GetResponse methods ]
		private Response GetResponse(Hashtable parameters)
		{
			return GetResponse (parameters, false);
		}
		private Response GetResponse(Hashtable parameters, bool debug)
		{
			CheckApiKey();

			// Calulate URL 
			string url = BaseUrl;
			
			StringBuilder UrlStringBuilder = new StringBuilder("", 2 * 1024);
			StringBuilder HashStringBuilder = new StringBuilder(sharedSecret, 2 * 1024);

			parameters["api_key"] = apiKey;

			if( apiToken != null && apiToken.Length > 0 )
			{
				parameters["auth_token"] = apiToken;
			}

			string[] keys = new string[parameters.Keys.Count];
			parameters.Keys.CopyTo(keys, 0);
			Array.Sort(keys);

			foreach(string key in keys)
			{
				if( UrlStringBuilder.Length > 0 ) UrlStringBuilder.Append("&");
				UrlStringBuilder.Append(key);
				UrlStringBuilder.Append("=");
				UrlStringBuilder.Append(Utils.UrlEncode(Convert.ToString(parameters[key])));
				HashStringBuilder.Append(key);
				HashStringBuilder.Append(parameters[key]);
			}

			if (sharedSecret != null && sharedSecret.Length > 0) 
			{
				if (UrlStringBuilder.Length > BaseUrl.Length + 1)
				{
					UrlStringBuilder.Append("&");
				}
				UrlStringBuilder.Append("api_sig=");
				UrlStringBuilder.Append(Md5Hash(HashStringBuilder.ToString()));
			}

			string variables = UrlStringBuilder.ToString();
			lastRequest = url;
			lastResponse = string.Empty;

			string responseXml = DoGetResponse(url, variables);
			if (debug) Console.WriteLine (responseXml);

			lastResponse = responseXml;
			return Utils.Deserialize(responseXml);
		}

#endregion


#region [ Auth ]
		/// <summary>
		/// Retrieve a temporary FROB from the Rtm service, to be used in redirecting the
		/// user to the Rtm web site for authentication. Only required for desktop authentication.
		/// </summary>
		/// <remarks>
		/// Pass the FROB to the <see cref="AuthCalcUrl"/> method to calculate the url.
		/// </remarks>
		/// <example>
		/// <code>
		/// string frob = Rtm.AuthGetFrob();
		/// string url = Rtm.AuthCalcUrl(frob, AuthLevel.Read);
		/// 
		/// // redirect the user to the url above and then wait till they have authenticated and return to the app.
		/// 
		/// Auth auth = Rtm.AuthGetToken(frob);
		/// 
		/// // then store the auth.Token for later use.
		/// string token = auth.Token;
		/// </code>
		/// </example>
		/// <returns>The FROB.</returns>
		public string AuthGetFrob()
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.auth.getFrob");

			RtmNet.Response response = GetResponse(parameters);
			if( response.Status == ResponseStatus.OK )
			{
				return response.AllElements[0].InnerText;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}

		/// <summary>
		/// Calculates the URL to redirect the user to Rtm web site for
		/// authentication. Used by desktop application. 
		/// See <see cref="AuthGetFrob"/> for example code.
		/// </summary>
		/// <param name="frob">The FROB to be used for authentication.</param>
		/// <param name="authLevel">The <see cref="AuthLevel"/> stating the maximum authentication level your application requires.</param>
		/// <returns>The url to redirect the user to.</returns>
		public string AuthCalcUrl(string frob, AuthLevel authLevel)
		{
			if( sharedSecret == null ) throw new SignatureRequiredException();

			string hash = sharedSecret + "api_key" + apiKey + "frob" + frob + "perms" + authLevel.ToString().ToLower();
			hash = Md5Hash(hash);
			string url = AuthUrl + "?api_key=" + apiKey + "&perms=" + authLevel.ToString().ToLower() + "&frob=" + frob;
			url += "&api_sig=" + hash;

			return url;
		}

		/// <summary>
		/// Calculates the URL to redirect the user to Rtm web site for
		/// auehtntication. Used by Web applications. 
		/// See <see cref="AuthGetFrob"/> for example code.
		/// </summary>
		/// <remarks>
		/// The Rtm web site provides 'tiny urls' that can be used in place
		/// of this URL when you specify your return url in the API key page.
		/// It is recommended that you use these instead as they do not include
		/// your API or shared secret.
		/// </remarks>
		/// <param name="authLevel">The <see cref="AuthLevel"/> stating the maximum authentication level your application requires.</param>
		/// <returns>The url to redirect the user to.</returns>
		public string AuthCalcWebUrl(AuthLevel authLevel)
		{
			if( sharedSecret == null ) throw new SignatureRequiredException();

			string hash = sharedSecret + "api_key" + apiKey + "perms" + authLevel.ToString().ToLower();
			hash = Md5Hash(hash);
			string url = AuthUrl + "?api_key=" + apiKey + "&perms=" + authLevel.ToString().ToLower();
			url += "&api_sig=" + hash;

			return url;
		}

		/// <summary>
		/// After the user has authenticated your application on the Rtm web site call this 
		/// method with the FROB (either stored from <see cref="AuthGetFrob"/> or returned in the URL
		/// from the Rtm web site) to get the users token.
		/// </summary>
		/// <param name="frob">The string containing the FROB.</param>
		/// <returns>A <see cref="Auth"/> object containing user and token details.</returns>
		public Auth AuthGetToken(string frob)
		{
			if( sharedSecret == null ) throw new SignatureRequiredException();

			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.auth.getToken");
			parameters.Add("frob", frob);

			RtmNet.Response response = GetResponse(parameters);
			if( response.Status == ResponseStatus.OK )
			{
				Auth auth = new Auth(response.AllElements[0]);
				return auth;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}

		/// <summary>
		/// Gets the full token details for a given mini token, entered by the user following a 
		/// web based authentication.
		/// </summary>
		/// <param name="miniToken">The mini token.</param>
		/// <returns>An instance <see cref="Auth"/> class, detailing the user and their full token.</returns>
		public Auth AuthGetFullToken(string miniToken)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.auth.getFullToken");
			parameters.Add("mini_token", miniToken.Replace("-", ""));
			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				Auth auth = new Auth(response.AllElements[0]);
				return auth;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}

		/// <summary>
		/// Checks a authentication token with the Rtm service to make
		/// sure it is still valid.
		/// </summary>
		/// <param name="token">The authentication token to check.</param>
		/// <returns>The <see cref="Auth"/> object detailing the user for the token.</returns>
		public Auth AuthCheckToken(string token)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.auth.checkToken");
			parameters.Add("auth_token", token);

			RtmNet.Response response = GetResponse(parameters);
			if( response.Status == ResponseStatus.OK )
			{
				Auth auth = new Auth(response.AllElements[0]);
				return auth;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}

		}		
#endregion


#region [ Timeline ]
		public string TimelineCreate()
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.timelines.create");
			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.Timeline;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}
#endregion


#region [ Lists ]
		/// <summary>
		/// Gets a list of contacts for the logged in user.
		/// Requires authentication.
		/// </summary>
		/// <returns>An instance of the <see cref="Contacts"/> class containing the list of contacts.</returns>
		public Lists ListsGetList()
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.lists.getList");
			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.Lists;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}
		
		public Lists ListsRename(string timeline, string listId, string newName)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.lists.setName");
			parameters.Add("timeline", timeline);
			parameters.Add("list_id", listId);
			parameters.Add("name", newName);

			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.Lists;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}
#endregion

		
#region [ Tasks ]
		/// <summary>
		/// Gets a list of contacts for the logged in user.
		/// Requires authentication.
		/// </summary>
		/// <returns>An instance of the <see cref="Contacts"/> class containing the list of contacts.</returns>
		public Tasks TasksGetList() {
			return TasksGetList(null);
		}
		public Tasks TasksGetList(string lastSync) {
			return TasksGetList(null, lastSync);
		}
		public Tasks TasksGetList(string listID, string lastSync)
		{
			return TasksGetList (listID, lastSync, null);
		}
		public Tasks TasksGetList (string listID, string lastSync, string filter)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.tasks.getList");
			if(listID != null)
				parameters.Add ("list_id", listID);
			if(filter != null)
				parameters.Add ("filter", filter);
			if(lastSync != null)
				parameters.Add ("last_sync", lastSync);
			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.Tasks;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}


		/// <summary>
		/// Sets the priority on a task
		/// </summary>
		/// <param name="listID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="taskSeriesID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="taskID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="priority">
		/// A <see cref="System.String"/>
		/// </param>
		public List TasksSetPriority (string timeline, string listID, string taskSeriesID, string taskID, string priority)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.tasks.setPriority");
			parameters.Add("timeline", timeline);
			parameters.Add("list_id", listID);
			parameters.Add("taskseries_id", taskSeriesID);
			parameters.Add("task_id", taskID);
			if(priority.CompareTo("N") != 0)
				parameters.Add("priority", priority);

			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.List;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}
		
		public List TasksMovePriority (string timeline, string listID, string taskSeriesID, string taskID, string direction)
		{
			Hashtable parameters = new Hashtable ();
			parameters.Add("method", "rtm.tasks.movePriority");
			parameters.Add("timeline", timeline);
			parameters.Add("list_id", listID);
			parameters.Add("taskseries_id", taskSeriesID);
			parameters.Add("task_id", taskID);
			parameters.Add("direction", direction);
			
			RtmNet.Response response = GetResponse(parameters);
			
			if (response.Status == ResponseStatus.OK)
				return response.List;
			else
				throw new RtmApiException(response.Error);
		}

		
		/// <summary>
		/// Sets the priority on a task
		/// </summary>
		/// <param name="listID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="taskSeriesID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="taskID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="priority">
		/// A <see cref="System.String"/>
		/// </param>
		public List TasksSetName(string timeline, string listID, string taskSeriesID, string taskID, string name)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.tasks.setName");
			parameters.Add("timeline", timeline);
			parameters.Add("list_id", listID);
			parameters.Add("taskseries_id", taskSeriesID);
			parameters.Add("task_id", taskID);
			parameters.Add("name", name);

			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.List;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}

		/// <summary>
		/// Sets the due date of a task
		/// </summary>
		/// <param name="timeline">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="listID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="taskSeriesID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="taskID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="name">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="List"/>
		/// </returns>
		public List TasksSetDueDate(string timeline, string listID, string taskSeriesID, string taskID)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.tasks.setDueDate");
			parameters.Add("timeline", timeline);
			parameters.Add("list_id", listID);
			parameters.Add("taskseries_id", taskSeriesID);
			parameters.Add("task_id", taskID);

			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.List;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}



		/// <summary>
		/// Sets the due date of a task
		/// </summary>
		/// <param name="timeline">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="listID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="taskSeriesID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="taskID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="name">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="List"/>
		/// </returns>
		public List TasksSetDueDate(string timeline, string listID, string taskSeriesID, string taskID, string due)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.tasks.setDueDate");
			parameters.Add("timeline", timeline);
			parameters.Add("list_id", listID);
			parameters.Add("taskseries_id", taskSeriesID);
			parameters.Add("task_id", taskID);
			parameters.Add("due", due);

			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.List;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}


		/// <summary>
		/// Sets the due date of a task
		/// </summary>
		/// <param name="timeline">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="listID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="taskSeriesID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="taskID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="name">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="List"/>
		/// </returns>
		public List TasksSetDueDateParse(string timeline, string listID, string taskSeriesID, string taskID, string due)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.tasks.setDueDate");
			parameters.Add("timeline", timeline);
			parameters.Add("list_id", listID);
			parameters.Add("taskseries_id", taskSeriesID);
			parameters.Add("task_id", taskID);
			if (!String.IsNullOrEmpty (due))
				parameters.Add("due", due);
			parameters.Add("parse", "1");
			parameters.Add("has_due_time", "1");

			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.List;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}


		/// <summary>
		/// Marks a task complete
		/// </summary>
		/// <param name="timeline">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="listID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="taskSeriesID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="taskID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="List"/>
		/// </returns>
		public List TasksComplete(string timeline, string listID, string taskSeriesID, string taskID)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.tasks.complete");
			parameters.Add("timeline", timeline);
			parameters.Add("list_id", listID);
			parameters.Add("taskseries_id", taskSeriesID);
			parameters.Add("task_id", taskID);

			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.List;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}
		
		
		/// <summary>
		/// Marks a task as uncomplete
		/// </summary>
		/// <param name="timeline">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="listID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="taskSeriesID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="taskID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="List"/>
		/// </returns>
		public List TasksUncomplete(string timeline, string listID, string taskSeriesID, string taskID)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.tasks.uncomplete");
			parameters.Add("timeline", timeline);
			parameters.Add("list_id", listID);
			parameters.Add("taskseries_id", taskSeriesID);
			parameters.Add("task_id", taskID);

			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.List;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}
		
		/// <summary>
		/// Moves a task from one list to another
		/// </summary>
		/// <param name="timeline">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="fromListID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="toListID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="taskSeriesID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="taskID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="List"/>
		/// </returns>
		public List TasksMoveTo(string timeline, string fromListID, string toListID, string taskSeriesID, string taskID)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.tasks.moveTo");
			parameters.Add("timeline", timeline);
			parameters.Add("from_list_id", fromListID);
			parameters.Add("to_list_id", toListID);
			parameters.Add("taskseries_id", taskSeriesID);
			parameters.Add("task_id", taskID);

			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.List;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}	
		
	
		public List TasksAdd(string timeline, string name)
		{
/*			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.tasks.add");
			parameters.Add("timeline", timeline);
			parameters.Add("name", name);

			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.List;
			}
			else
			{
				throw new RtmApiException(response.Error);
			} */
			return TasksAdd (timeline, name, null, false);
		}

		public List TasksAdd (string timeline, string name, bool parse)
		{
			return TasksAdd (timeline, name, null, parse);
		}
			
		public List TasksAdd (string timeline, string name, string listID)
		{
			return TasksAdd (timeline, name, listID, false);
		}
		public List TasksAdd(string timeline, string name, string listID, bool parse)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.tasks.add");
			parameters.Add("timeline", timeline);
			parameters.Add("name", name);
			if (!String.IsNullOrEmpty (listID))
				parameters.Add("list_id", listID);
			if (parse)
				parameters.Add("parse", "1");

			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.List;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}


		public List TasksDelete(string timeline, string listID, string taskSeriesID, string taskID)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.tasks.delete");
			parameters.Add("timeline", timeline);
			parameters.Add("list_id", listID);	
			parameters.Add("taskseries_id", taskSeriesID);
			parameters.Add("task_id", taskID);

			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.List;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}

		
		/// <summary>
		/// Add a selection of tags to a photo.
		/// </summary>
		/// <param name="photoId">The photo id of the photo.</param>
		/// <param name="tags">An array of strings containing the tags.</param>
		/// <returns>True if the tags are added successfully.</returns>
		public void TasksAddTags(string photoId, string[] tags)
		{	
			string s = string.Join(",", tags);
			TasksAddTags(photoId, s);
		}

		/// <summary>
		/// Add a selection of tags to a photo.
		/// </summary>
		/// <param name="photoId">The photo id of the photo.</param>
		/// <param name="tags">An string of comma delimited tags.</param>
		/// <returns>True if the tags are added successfully.</returns>
		public void TasksAddTags(string photoId, string tags)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.tasks.addTags");
			parameters.Add("photo_id", photoId);
			parameters.Add("tags", tags);

			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}
 
		/// <summary>
		/// Postpone a task
		/// </summary>
		/// <param name="timeline">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="listID">
		/// A <see cref="System.String"/> defines the ID of the list the task belongs to 
		/// </param>
		/// <param name="taskSeriesID">
		/// A <see cref="System.String"/> defines the ID of the task series the task belongs to
		/// </param>
		/// <param name="taskID">
		/// A <see cref="System.String"/> defines the task ID
		/// </param>
		/// <returns>
		/// A <see cref="List"/>
		/// </returns>
		public List TasksPostpone (string timeline, string listID, string taskSeriesID, string taskID)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.tasks.postpone");
			parameters.Add("timeline", timeline);
			parameters.Add("list_id", listID);	
			parameters.Add("taskseries_id", taskSeriesID);
			parameters.Add("task_id", taskID);

			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.List;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}
		
		public List TasksSetRecurrence (string timeline, string listID, string taskSeriesID, string taskID, string repeat)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.tasks.setRecurrence");
			parameters.Add("timeline", timeline);
			parameters.Add("list_id", listID);	
			parameters.Add("taskseries_id", taskSeriesID);
			parameters.Add("task_id", taskID);
			if (repeat != null)
				parameters.Add("repeat", repeat);

			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.List;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}

		}
		
		public List TasksSetUrl(string timeline, string listID, string taskSeriesID, string taskID, string url)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.tasks.setURL");
			parameters.Add("timeline", timeline);
			parameters.Add("list_id", listID);	
			parameters.Add("taskseries_id", taskSeriesID);
			parameters.Add("task_id", taskID);
			parameters.Add("url", url);

			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.List;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}

		}
#endregion

		
		
		
		
		
		

#region [ Contacts ]
		/// <summary>
		/// Gets a list of contacts for the logged in user.
		/// Requires authentication.
		/// </summary>
		/// <returns>An instance of the <see cref="Contacts"/> class containing the list of contacts.</returns>
		public Contacts ContactsGetList()
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.contacts.getList");
			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.Contacts;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}

		/// <summary>
		/// Gets a list of the given users contact, or those that are publically avaiable.
		/// </summary>
		/// <param name="userId">The Id of the user who's contacts you want to return.</param>
		/// <returns>An instance of the <see cref="Contacts"/> class containing the list of contacts.</returns>
		public Contacts ContactsGetPublicList(string userId)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.contacts.getPublicList");
			parameters.Add("user_id", userId);
			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.Contacts;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}
#endregion



#region [ Notes ]
		/// <summary>
		/// Adds a note to a task
		/// </summary>
		/// <param name="timeline">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="listID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="taskSeriesID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="taskID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="noteTitle">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="noteText">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="Note"/>
		/// </returns>
		public Note NotesAdd(string timeline, string listID, string taskSeriesID, string taskID, string noteTitle, string noteText)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.tasks.notes.add");
			parameters.Add("timeline", timeline);
			parameters.Add("list_id", listID);
			parameters.Add("taskseries_id", taskSeriesID);
			parameters.Add("task_id", taskID);
			parameters.Add("note_title", noteTitle);
			parameters.Add("note_text", noteText);

			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.Note;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}


		/// <summary>
		/// Deletes a note
		/// </summary>
		/// <param name="timeline">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="noteID">
		/// A <see cref="System.String"/>
		/// </param>
		public void NotesDelete(string timeline, string noteID)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.tasks.notes.delete");
			parameters.Add("timeline", timeline);
			parameters.Add("note_id", noteID);

			RtmNet.Response response = GetResponse(parameters);

			if( response.Status != ResponseStatus.OK )
			{
				throw new RtmApiException(response.Error);
			}
		}


		/// <summary>
		/// Modifies an existing note
		/// </summary>
		/// <param name="timeline">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="noteID">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="noteTitle">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="noteText">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="Note"/>
		/// </returns>
		public Note NotesEdit(string timeline, string noteID, string noteTitle, string noteText)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "rtm.tasks.notes.edit");
			parameters.Add("timeline", timeline);
			parameters.Add("note_id", noteID);			
			parameters.Add("note_title", noteTitle);
			parameters.Add("note_text", noteText);

			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.Note;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}


#endregion




#region [ Tests ]
		/// <summary>
		/// Can be used to call unsupported methods in the Rtm API.
		/// </summary>
		/// <remarks>
		/// Use of this method is not supported. 
		/// The way the RtmNet API Library works may mean that some methods do not return an expected result 
		/// when using this method.
		/// </remarks>
		/// <param name="method">The method name, e.g. "Rtm.test.null".</param>
		/// <param name="parameters">A list of parameters. Note, api_key is added by default and is not included. Can be null.</param>
		/// <returns>An array of <see cref="XmlElement"/> instances which is the expected response.</returns>
		public XmlElement[] TestGeneric(string method, NameValueCollection parameters)
		{
			Hashtable _parameters = new Hashtable();
			if( parameters != null )
			{
				foreach(string key in parameters.AllKeys)
				{
					_parameters.Add(key, parameters[key]);
				}
			}
			_parameters.Add("method", method);

			RtmNet.Response response = GetResponse(_parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return response.AllElements;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}
		/// <summary>
		/// Runs the Rtm.test.echo method and returned an array of <see cref="XmlElement"/> items.
		/// </summary>
		/// <param name="echoParameter">The parameter to pass to the method.</param>
		/// <param name="echoValue">The value to pass to the method with the parameter.</param>
		/// <returns>An array of <see cref="XmlElement"/> items.</returns>
		/// <remarks>
		/// The APi Key has been removed from the returned array and will not be shown.
		/// </remarks>
		/// <example>
		/// <code>
		/// XmlElement[] elements = Rtm.TestEcho("&amp;param=value");
		/// foreach(XmlElement element in elements)
		/// {
		///		if( element.Name = "method" )
		///			Console.WriteLine("Method = " + element.InnerXml);
		///		if( element.Name = "param" )
		///			Console.WriteLine("Param = " + element.InnerXml);
		/// }
		/// </code>
		/// </example>
		public XmlElement[] TestEcho(string echoParameter, string echoValue)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "Rtm.test.echo");
			parameters.Add("api_key", apiKey);
			if( echoParameter != null && echoParameter.Length > 0 )
			{
				parameters.Add(echoParameter, echoValue);
			}

			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				// Remove the api_key element from the array.
				XmlElement[] elements = new XmlElement[response.AllElements.Length - 1];
				int c = 0;
				foreach(XmlElement element in response.AllElements)
				{
					if(element.Name != "api_key" )
						elements[c++] = element;
				}
				return elements;
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}

		/// <summary>
		/// Test the logged in state of the current Filckr object.
		/// </summary>
		/// <returns>The <see cref="FoundUser"/> object containing the username and userid of the current user.</returns>
		public FoundUser TestLogin()
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("method", "Rtm.test.login");

			RtmNet.Response response = GetResponse(parameters);

			if( response.Status == ResponseStatus.OK )
			{
				return new FoundUser(response.AllElements[0]);
			}
			else
			{
				throw new RtmApiException(response.Error);
			}
		}
#endregion

#region [ MD5 Hash ]
		private static string Md5Hash(string unhashed)
		{
			System.Security.Cryptography.MD5CryptoServiceProvider csp = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] bytes = System.Text.Encoding.UTF8.GetBytes(unhashed);
			byte[] hashedBytes = csp.ComputeHash(bytes, 0, bytes.Length);
			return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
		}
#endregion

		private void CheckApiKey()
		{
			if( ApiKey == null || ApiKey.Length == 0 )
				throw new ApiKeyRequiredException();
		}
		/*private void CheckRequiresAuthentication()
		  {
		  CheckApiKey();

		  if( ApiSecret == null || ApiSecret.Length == 0 )
		  throw new SignatureRequiredException();
		  if( AuthToken == null || AuthToken.Length == 0 )
		  throw new AuthenticationRequiredException();

		  }
		 */
	}

}

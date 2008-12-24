using System;

namespace RtmNet
{
	/// <summary>
	/// Exception thrown when method requires authentication but no authentication token is supplied.
	/// </summary>
	public class AuthenticationRequiredException : RtmException
	{
		internal AuthenticationRequiredException() : base("Method requires authentication but no token supplied.")
		{
		}
	}
}

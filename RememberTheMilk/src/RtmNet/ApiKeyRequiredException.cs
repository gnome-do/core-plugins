using System;

namespace RtmNet
{
	/// <summary>
	/// Exception thrown is no API key is supplied.
	/// </summary>
	public class ApiKeyRequiredException : RtmException
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ApiKeyRequiredException() : base("API Key is required for all method calls")
		{
		}
	}
}

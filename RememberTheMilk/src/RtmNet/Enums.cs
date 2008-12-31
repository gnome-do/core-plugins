using System;
using System.Xml.Serialization;

namespace RtmNet
{
	/// <summary>
	/// A list of service the Rtm.Net API Supports.
	/// </summary>
	/// <remarks>
	/// Not all methods are supported by all service. Behaviour of the library may be unpredictable if not using Rtm
	/// as your service.
	/// </remarks>
	public enum SupportedService
	{
		/// <summary>
		/// Rtm - http://www.Rtm.com/services/api
		/// </summary>
		Rtm = 0
	}
	
	/// <summary>
	/// Used to specify where all tags must be matched or any tag to be matched.
	/// </summary>
	[Serializable]
	public enum TagMode
	{
		/// <summary>
		/// No tag mode specified.
		/// </summary>
		None,
		/// <summary>
		/// Any tag must match, but not all.
		/// </summary>
		AnyTag,
		/// <summary>
		/// All tags must be found.
		/// </summary>
		AllTags,
		/// <summary>
		/// Uncodumented and unsupported tag mode where boolean operators are supported.
		/// </summary>
		Boolean
	}

}

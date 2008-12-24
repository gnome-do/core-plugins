using System;

namespace RtmNet
{
	/// <summary>
	/// Generic Rtm.Net Exception.
	/// </summary>
	[Serializable]
	public class RtmException : Exception
	{
		internal RtmException()
		{
		}

		internal RtmException(string message) : base(message)
		{
		}

		internal RtmException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}

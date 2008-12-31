using System;

namespace RtmNet
{
	/// <summary>
	/// Exception thrown when a communication error occurs with a web call.
	/// </summary>
	public class RtmWebException : RtmException
	{
		internal RtmWebException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}

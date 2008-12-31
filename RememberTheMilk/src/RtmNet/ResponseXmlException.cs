using System;

namespace RtmNet
{
	/// <summary>
	/// Exception thrown when an error parsing the returned XML.
	/// </summary>
	public class ResponseXmlException : RtmException
	{
		internal ResponseXmlException(string message) : base(message)
		{
		}

		internal ResponseXmlException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}

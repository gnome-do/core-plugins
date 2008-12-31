using System;

namespace RtmNet
{
	/// <summary>
	/// Thrown when a method requires a valid signature but no shared secret has been supplied.
	/// </summary>
	public class SignatureRequiredException : RtmException
	{
		internal SignatureRequiredException() : base("Method requires signing but no shared secret supplied.")
		{
		}
	}
}

using System;

namespace RtmNet
{
	/// <summary>
	/// Exception thrown when the Rtm API returned a specifi error code.
	/// </summary>
	public class RtmApiException : RtmException
	{
		private int code;
		private string msg = "";

		internal RtmApiException(ResponseError error)
		{
			code = error.Code;
			msg = error.Message;
		}

		/// <summary>
		/// Get the code of the Rtm error.
		/// </summary>
		public int Code
		{
			get { return code; }
		}

		/// <summary>
		/// Gets the verbose message returned by Rtm.
		/// </summary>
		public string Verbose
		{
			get { return msg; }
		}
		
		/// <summary>
		/// Overrides the message to return custom error message.
		/// </summary>
		public override string Message
		{
			get
			{
				return msg + " (" + code + ")";
			}
		}
	}
}

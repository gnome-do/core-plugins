using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace RtmNet
{
	/// <summary>
	/// Internal class providing certain utility functions to other classes.
	/// </summary>
	internal sealed class Utils
	{
		private static readonly DateTime unixStartDate = new DateTime(1970, 1, 1, 0, 0, 0);

		private Utils()
		{
		}

#if !WindowsCE
		internal static string UrlEncode(string oldString)
		{
			if( oldString == null ) return null;

			string a = System.Web.HttpUtility.UrlEncode(oldString);
			a = a.Replace("&", "%26");
			a = a.Replace("=", "%3D");
			a = a.Replace(" ", "%20");
			return a;
		}
#else
        internal static string UrlEncode(string oldString)
        {
            if (oldString == null) return String.Empty;
            StringBuilder sb = new StringBuilder(oldString.Length * 2);
            Regex reg = new Regex("[a-zA-Z0-9$-_.+!*'(),]");

            foreach (char c in oldString)
            {
                if (reg.IsMatch(c.ToString()))
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append(ToHex(c));
                }
            }
            return sb.ToString();
        }

        private static string ToHex(char c)
        {
            return ((int)c).ToString("X");
        }
#endif

		/// <summary>
		/// Converts a <see cref="DateTime"/> object into a unix timestamp number.
		/// </summary>
		/// <param name="date">The date to convert.</param>
		/// <returns>A long for the number of seconds since 1st January 1970, as per unix specification.</returns>
		internal static long DateToUnixTimestamp(DateTime date)
		{
			TimeSpan ts = date - unixStartDate;
			return (long)ts.TotalSeconds;
		}

		/// <summary>
		/// Converts a string, representing a unix timestamp number into a <see cref="DateTime"/> object.
		/// </summary>
		/// <param name="timestamp">The timestamp, as a string.</param>
		/// <returns>The <see cref="DateTime"/> object the time represents.</returns>
		internal static DateTime UnixTimestampToDate(string timestamp)
		{
			if( timestamp == null || timestamp.Length == 0 ) return DateTime.MinValue;

			
			return UnixTimestampToDate(long.Parse(timestamp));
		}

		/// <summary>
		/// Converts a <see cref="long"/>, representing a unix timestamp number into a <see cref="DateTime"/> object.
		/// </summary>
		/// <param name="timestamp">The unix timestamp.</param>
		/// <returns>The <see cref="DateTime"/> object the time represents.</returns>
		internal static DateTime UnixTimestampToDate(long timestamp)
		{
			return unixStartDate.AddSeconds(timestamp);
		}


		internal static DateTime DateStringToDateTime(string timestring)
		{
			if (timestring == null | timestring.Length == 0) return DateTime.MinValue;
			DateTime dt = DateTime.Parse(timestring);
			return dt;
		}

		internal static void WriteInt32(Stream s, int i)
		{
			s.WriteByte((byte) (i & 0xFF));
			s.WriteByte((byte) ((i >> 8) & 0xFF));
			s.WriteByte((byte) ((i >> 16) & 0xFF));
			s.WriteByte((byte) ((i >> 24) & 0xFF));
		}

		internal static void WriteString(Stream s, string str)
		{
			WriteInt32(s, str.Length);
			foreach (char c in str)
			{
				s.WriteByte((byte) (c & 0xFF));
				s.WriteByte((byte) ((c >> 8) & 0xFF));
			}
		}

		internal static void WriteAsciiString(Stream s, string str)
		{
			WriteInt32(s, str.Length);
			foreach (char c in str)
			{
				s.WriteByte((byte) (c & 0x7F));
			}
		}

		internal static int ReadInt32(Stream s)
		{
			int i = 0, b;
			for (int j = 0; j < 4; j++)
			{
				b = s.ReadByte();
				if (b == -1)
					throw new IOException("Unexpected EOF encountered");
				i |= (b << (j * 8));
			}
			return i;
		}

		internal static string ReadString(Stream s)
		{
			int len = ReadInt32(s);
			char[] chars = new char[len];
			for (int i = 0; i < len; i++)
			{
				int hi, lo;
				lo = s.ReadByte();
				hi = s.ReadByte();
				if (lo == -1 || hi == -1)
					throw new IOException("Unexpected EOF encountered");
				chars[i] = (char) (lo | (hi << 8));
			}
			return new string(chars);
		}

		internal static string ReadAsciiString(Stream s)
		{
			int len = ReadInt32(s);
			char[] chars = new char[len];
			for (int i = 0; i < len; i++)
			{
				int c = s.ReadByte();
				if (c == -1)
					throw new IOException("Unexpected EOF encountered");
				chars[i] = (char) (c & 0x7F);
			}
			return new string(chars);
		}
	
		private const string photoUrl = "http://farm{0}.static.Rtm.com/{1}/{2}_{3}{4}.{5}";

		private static readonly Hashtable _serializers = new Hashtable();

		private static XmlSerializer GetSerializer(Type type)
		{
			if( _serializers.ContainsKey(type.Name) )
				return (XmlSerializer)_serializers[type.Name];
			else
			{
				XmlSerializer s = new XmlSerializer(type);
				_serializers.Add(type.Name, s);
				return s;
			}
		}
		/// <summary>
		/// Converts the response string (in XML) into the <see cref="Response"/> object.
		/// </summary>
		/// <param name="responseString">The response from Rtm.</param>
		/// <returns>A <see cref="Response"/> object containing the details of the </returns>
		internal static Response Deserialize(string responseString)
		{
			XmlSerializer serializer = GetSerializer(typeof(RtmNet.Response));
			try
			{
				// Deserialise the web response into the Rtm response object
				StringReader responseReader = new StringReader(responseString);
				RtmNet.Response response = (RtmNet.Response)serializer.Deserialize(responseReader);
				responseReader.Close();

				return response;
			}
			catch(InvalidOperationException ex)
			{
				// Serialization error occurred!
				throw new ResponseXmlException("Invalid response received from Rtm.", ex);
			}
		}

		internal static object Deserialize(System.Xml.XmlNode node, Type type)
		{
			XmlSerializer serializer = GetSerializer(type);
			try
			{
				// Deserialise the web response into the Rtm response object
				System.Xml.XmlNodeReader reader = new System.Xml.XmlNodeReader(node);
				object o = serializer.Deserialize(reader);
				reader.Close();

				return o;
			}
			catch(InvalidOperationException ex)
			{
				// Serialization error occurred!
				throw new ResponseXmlException("Invalid response received from Rtm.", ex);
			}
		}



	}

}

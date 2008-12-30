//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;

namespace SqueezeCenter
{	
	public static class Util
	{		
		public static IEnumerable<Ttarget> Cast<Tsource, Ttarget> (IEnumerable<Tsource> items)
		{						
			foreach (object i in items)
				yield return (Ttarget)i;
		}
		
		public static string UriDecode (string s)
		{
			int i = 0;
			List<byte> buff = new List<byte> (s.Length);
			
			while (i < s.Length)
			{
				if (s[i] == '%')
				{
					if (i + 2 < s.Length)
					{
						buff.Add (Byte.Parse(s.Substring (i+1, 2), System.Globalization.NumberStyles.HexNumber));
						i+=3;
					}
					else
					{
						break;
					}
				}
				else
				{
					buff.Add ((byte)s[i++]);
				}
			}
			return System.Text.Encoding.UTF8.GetString (buff.ToArray ());
		}
		
	}
	
}

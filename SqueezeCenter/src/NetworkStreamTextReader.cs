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
using System.Net.Sockets;
using System.Text;

namespace SqueezeCenter
{	
	public class NetworkStreamTextReader
	{
		NetworkStream stream;
		StringBuilder data = new StringBuilder ();
		byte[] readBuffer = new byte[1024];
		bool disconnected = false;
		
		public NetworkStreamTextReader (System.Net.Sockets.NetworkStream stream)
		{
			this.stream = stream;
			stream.BeginRead (readBuffer, 0, readBuffer.Length, new System.AsyncCallback (CB), null);
		}
		
		private void CB (IAsyncResult ar)
		{
			int numberOfBytesRead = stream.EndRead (ar);
			if (numberOfBytesRead == 0) {
				// disconnected
				disconnected = true;
				return;
			}

			lock (data)
				data.Append (Encoding.ASCII.GetString (readBuffer, 0, numberOfBytesRead));

			// read again
			stream.BeginRead (readBuffer, 0, readBuffer.Length, new System.AsyncCallback (CB), null);
		}
		
		public string ReadLine ()
		{						
			int i = 0;
			
			if (disconnected)
				throw new System.IO.IOException ("Connection closed");
			
			// return first line of data or null if no data is available
			lock (data) {
				if (data.Length == 0)
					return null;
				
				i = data.ToString().IndexOf ('\n');
				if (i >= 0) {
					string result = data.ToString (0, i);
					data = data.Remove (0, i + 1);
					return result;
				} else {
					return null;
				}		
			}
		}
	}
}

/* YoutubeConfig.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using Gtk;
using Do.Platform;
using Do.Platform.Linux;

namespace Do.Universe
{	
	
	public class YouTubeConfig : AbstractLoginWidget
	{
		const string Uri = "http://www.youtube.com/signup?next_url=/index&";
        
		public YouTubeConfig() : base ("YouTube", Uri)
		{
			Username = Youtube.Preferences.Username;
			Password = Youtube.Preferences.Password;
		}
		
		
		protected override void SaveAccountData(string username, string password)
		{
			Youtube.Preferences.Username = username;
			Youtube.Preferences.Password = password;
		}
		
		protected override bool Validate (string username, string password)
		{
			if (username.Length > 0 && password.Length > 0)
				return Youtube.TryConnect (username, password);
			return false;
		}
	}
}


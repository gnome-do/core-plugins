/* Bubash.cs 
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
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Quote
{

	public class Bubash : IQuoteProvider
	{
		const string urlRoot = "http://bubash.org/";

		List<string> tags;
		
		public Bubash()
		{
			Parameters = new NameValueCollection ();
			Parameters ["tags"] = "";
			Parameters ["quote"] = "";

			tags = LoadSavedTags ();
		}

		public Bubash (string quote) : this ()
		{
			Parameters ["quote"] = quote;
		}

		public Bubash (string quote, IEnumerable<string> tags) : this ()
		{
			Parameters ["quote"] = quote;
			Parameters ["tags"] = string.Join (" ", tags.ToArray ());
		}

		public bool ShouldAllowAutoRedirect {
			get { return false; }
		}

		public string Name {
			get { return "BuBash.org"; }
		}

		public string BaseUrl {
			get { return urlRoot + "submit"; }
		}

		public NameValueCollection Parameters { get; private set; }

		public IEnumerable<string> SavedTags {
			get { return tags; }
		}

		public string GetPasteUrlFromResponse (HttpWebResponse response)
		{
			return urlRoot + "/queue";
		}

		List<string> LoadSavedTags ()
		{
			return new List<string> ();
		}
	}
}

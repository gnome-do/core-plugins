//  OpenSearchItemTests.cs
//
//  GNOME Do is the legal property of its developers, whose names are too numerous
//  to list here.  Please refer to the COPYRIGHT file distributed with this
//  source distribution.
//
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
using NUnit.Framework;

using OpenSearch;

namespace OpenSearch.Tests
{
	[TestFixture()]
	public class OpenSearchItemTest
	{
		
		[Test()]
		public void BuildSearchUrl_should_encode_single_quotes ()
		{
			OpenSearchItem item = new OpenSearchItem ("test","test","http://www.google.com?q={searchTerms}");
			string searchTerm = "it's";
			string searchUrl = item.BuildSearchUrl (searchTerm);
			bool containsUnencodedSingleQuotes = searchUrl.Contains (searchTerm);	
			
			Assert.IsFalse (containsUnencodedSingleQuotes);
		}
		
		[Test()]
		public void BuildSearchUrl_should_not_encode_unicode_characters ()
		{
			OpenSearchItem item = new OpenSearchItem ("test","test","http://www.google.com?q={searchTerms}");
			string searchTerm = "日本語";
			string searchUrl = item.BuildSearchUrl (searchTerm);
			bool containsUnencodedUnicodeCharacters = searchUrl.Contains (searchTerm);	
			
			Assert.IsTrue (containsUnencodedUnicodeCharacters);
		}
	}
}







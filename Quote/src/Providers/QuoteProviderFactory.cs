/* QuoteProviderFactory.cs 
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

namespace Quote
{
	
	public class QuoteProviderFactory
	{
	
		public static IQuoteProvider GetProviderFromPreferences()
		{
			object [] args = new object [] {};
			return GetProviderFromPreferences (args);		
		}
		
		public static IQuoteProvider GetProviderFromPreferences (string text)
		{
			object [] args = new object [] {text};
				return GetProviderFromPreferences (args);		
		}
		
		public static IQuoteProvider GetProviderFromPreferences (string text, string tags)
		{
			object [] args = new object [] {text, tags};
			return GetProviderFromPreferences (args);
		}
		
		public static IQuoteProvider GetProviderFromPreferences (object [] args)
		{	
			Type providerType = typeof (Bubash);
			IQuoteProvider provider = (IQuoteProvider) System.Activator.CreateInstance (providerType, args);
			return provider;
		}
	}
}

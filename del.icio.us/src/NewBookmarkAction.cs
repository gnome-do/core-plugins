/* NewBookmarkAction.cs
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
using System.Threading;
using System.Text.RegularExpressions;

using Mono.Unix;
using Do.Universe;


//This code is ready, but I'm waiting for a bug to be released at delicious to
//actually include it as part of the plugin. Once that bug is fixed, this code will
//be working. for more info see http://support.delicious.com/forum/comments.php?DiscussionID=454
/*
namespace Delicious
{
	public class NewBookmarkAction : IAction
	{
		// URL regex taken from http://www.osix.net/modules/article/?id=586
		const string UrlPattern = "^(https?://)"
        + "?(([0-9a-zA-Z_!~*'().&=+$%-]+: )?[0-9a-zA-Z_!~*'().&=+$%-]+@)?" //user@
        + @"(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP- 199.194.52.184
        + "|" // allows either IP or domain
        + @"([0-9a-zA-Z_!~*'()-]+\.)*" // tertiary domain(s)- www.
        + @"([0-9a-zA-Z][0-9a-zA-Z-]{0,61})?[0-9a-zA-Z]\." // second level domain
        + "[a-zA-Z]{2,6})" // first level domain- .com or .museum
        + "(:[0-9]{1,4})?" // port number- :80
        + "((/?)|" // a slash isn't required if there is no file name
        + "(/[0-9a-zA-Z_!~*'().;?:@&=+$,%#-]+)+/?)$";
        
        Regex UrlRegex;
		
		public NewBookmarkAction ()
		{
			UrlRegex = new Regex (UrlPattern, RegexOptions.Compiled);
		}
        
		public string Name {
			get { return Catalog.GetString ("New del.icio.us bookmark"); }
		}
		
		public string Description {
			get { return Catalog.GetString ("Create a new bookmark at del.icio.us"); }
		}
		
		public string Icon {
			get { return "delicious.png@" + GetType ().Assembly.FullName; }
		}
		
		public IEnumerable<Type> SupportedItemTypes {
			get {
				return new Type [] {
					typeof (IURLItem),
					typeof (ITextItem),
				};
			}
		}
		
		public IEnumerable<Type> SupportedModifierItemTypes {
			get { return new Type [] { typeof (ITextItem), }; }
		}
		
		public bool SupportsItem (IItem item)
		{
			if (item is ITextItem)
				return UrlRegex.IsMatch ((item as ITextItem).Text);
			else if (item is IURLItem)
				return true;
				
			return false;
		}
		
		public bool SupportsModifierItemForItems (IEnumerable<IItem> items, IItem modItem)
		{
			return true;
		}
		
		public IEnumerable<IItem> DynamicModifierItemsForItem (IItem item)
		{
			return null;
		}
		
		public bool ModifierItemsOptional {
			get { return true; }
		}
		
		public IEnumerable<IItem> Perform (IEnumerable<IItem> items, IEnumerable<IItem> modItems)
		{
			string url;
			foreach (IItem item in items) {
				if (item is ITextItem)
					url = (item as ITextItem).Text;
				else
					url = (item as IURLItem).URL;
				
				//these are stupid workarounds for an upstream bug
				//http://support.delicious.com/forum/comments.php?DiscussionID=454
				if (!url.StartsWith ("http://"))
					url = "http://" + url;
				Thread newBookmark = new Thread (new ParameterizedThreadStart (
					Delicious.NewBookmark));
				newBookmark.IsBackground = true;
				newBookmark.Start ((object) url);
			}
			return null;
		}
	}
}
*/
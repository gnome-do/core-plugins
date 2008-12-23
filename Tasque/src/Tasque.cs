// Tasque.cs
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, see <http://www.gnu.org/licenses/> or 
// write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, 
// Boston, MA 02111-1307 USA
//

using System;
using System.Linq;
using System.Collections.Generic;

using Do.Universe;
using Do.Platform;

using Tasque.DBus;

namespace Tasque
{
	public static class Tasque
	{		
		public static IEnumerable<TasqueCategoryItem> GetCategoryItems ()
		{
			IEnumerable<string> categories;
			
			try {
				TasqueDBus tasque = new TasqueDBus ();
				categories = tasque.GetCategoryNames ();
			} catch (Exception e) {
				Log.Error ("Could not read Tasque's category: {0}", e.Message);
				Log.Debug (e.StackTrace);
			}
			return categories.Select (category => new TasqueCategoryItem (category));
		}
	}
}

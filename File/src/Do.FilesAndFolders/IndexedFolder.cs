/* IndexedFolder.cs
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
using System.Collections;
using System.Collections.Generic;

using Do.Platform;

namespace Do.FilesAndFolders
{

	[Serializable]
	struct IndexedFolder : IEquatable<IndexedFolder>
	{

		public string Path { get; private set; }
		public int Level { get; private set; }
		
		public IndexedFolder (string path, int level)
		{
			if (path == null) throw new ArgumentNullException ("path");
			
			Path = path.Replace ("~", Paths.UserHome);
			Level = level;
		}

		public override bool Equals (object other)
		{
			return other is IndexedFolder && Equals ((IndexedFolder) other);
		}

		public override int GetHashCode ()
		{
			return Path.GetHashCode () ^ Level.GetHashCode ();
		}

		public bool Equals (IndexedFolder other)
		{
			return other.Path == Path && other.Level == Level;
		}

		public static bool operator== (IndexedFolder left, IndexedFolder right)
		{
			return left.Equals (right);
		}

		public static bool operator!= (IndexedFolder left, IndexedFolder right)
		{
			return !left.Equals (right);
		}
	}
}

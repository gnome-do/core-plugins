/* FileTransformation.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this source distribution.
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
using System.IO;
using System.Linq;

using Do.Platform;

namespace Do.FilesAndFolders
{

	public delegate bool IncludeFile (string source);
	public delegate void TransformFile (string source, string destination);

	/// <summary>
	/// A class representing a recursive file transformation.
	/// </summary>
	public class FileTransformation
	{

		static bool DefaultIncludeFile (string source)
		{
			return !". ..".Contains (Path.GetFileName (source));
		}
		
		public TransformFile Transformation { get; private set; }
		public IncludeFile Include { get; private set; }
		
		public FileTransformation (TransformFile transform) :
			this (transform, DefaultIncludeFile)
		{
		}

		public FileTransformation (TransformFile transform, IncludeFile include)
		{
			Transformation = transform;
			Include = include;
		}

		/// <summary>
		/// Recursively perform transformation an all files at or below source
		/// to destination.
		/// </summary>
		/// <param name="source">
		/// A <see cref="System.String"/> source file or directory to transform.
		/// </param>
		/// <param name="destination">
		/// A <see cref="System.String"/> destination file or directory to transform to.
		/// </param>
		public void Transform (string source, string destination)
		{
			string target = Path.Combine (destination, Path.GetFileName (source));
			Transformation (source, target);
			if (Directory.Exists (source)) {
				Directory.GetFileSystemEntries (source)
					.Where (f => Include (f))
					.ForEach (f => Transform (f, target));
			}
		}
	}
}

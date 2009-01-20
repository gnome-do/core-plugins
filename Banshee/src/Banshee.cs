/* Banshee.cs 
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
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;

using Do.Platform;

namespace Banshee
{
	public enum PlaybackShuffleMode
    {
        Linear,
        Song,
        Artist,
        Album
    }
	
	public class Banshee
	{
		static BansheeDbus bus;
		
		static Banshee()
		{
			bus = new BansheeDbus ();
		}

		public static bool Playing {
			get { return bus.Playing; }
		}
		
		public static void Play ()
		{
			Thread thread = new Thread ((ThreadStart) (() => bus.Play ()));
			thread.IsBackground = true;
			thread.Start ();
		}

		public static void Pause ()
		{
			Thread thread = new Thread ((ThreadStart) (() => bus.Pause ()));
			thread.IsBackground = true;
			thread.Start ();
		}

		public static void Next ()
		{
			Thread thread = new Thread ((ThreadStart) (() => bus.Next ()));
			thread.IsBackground = true;
			thread.Start ();
		}

		public static void Previous ()
		{
			Thread thread = new Thread ((ThreadStart) (() => bus.Previous ()));
			thread.IsBackground = true;
			thread.Start ();
		}
	}
}

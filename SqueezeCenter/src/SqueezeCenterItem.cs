// ISqueezeCenterItem.cs created with MonoDevelop
// User: anders at 13:06 11-12-2008
//
////  This program is free software: you can redistribute it and/or modify
////  it under the terms of the GNU General Public License as published by
////  the Free Software Foundation, either version 3 of the License, or
////  (at your option) any later version.
////
////  This program is distributed in the hope that it will be useful,
////  but WITHOUT ANY WARRANTY; without even the implied warranty of
////  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
////  GNU General Public License for more details.
////
////  You should have received a copy of the GNU General Public License
////  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using Do.Universe;

namespace SqueezeCenter
{
	
	public abstract class SqueezeCenterItem : Do.Universe.Item
	{
		public SqueezeCenterItem()
		{			
		}
		
		bool available = true;
		public bool Available
		{
			get { return this.available; }
			set { this.available = value; }
		}
		
		public override abstract string Name { get; }
		public override abstract string Icon { get; }
		public override abstract string Description { get; }
		
	}
}

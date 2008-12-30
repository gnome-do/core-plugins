/* RTMPriorityItem.cs
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



using Do.Universe;
		
namespace Do.Addins.RTM
{
	public class RTMPriorityItem : Item
	{
		private string name;
		private string desc;
		
		public RTMPriorityItem (string name, string desc)
		{
			this.name = name;
			this.desc = desc;
		}
		
		public override string Name { get { return name; } }
		public override string Description { get { return desc; } }
		public override string Icon {
			get {
				if (name == "Low")
					return "task-low.png@" + GetType ().Assembly.FullName;
				else if (name == "Medium")
					return "task-medium.png@" + GetType ().Assembly.FullName;
				else if (name == "High")
					return "task-high.png@" + GetType ().Assembly.FullName;
				else if (name == "Up")
					return "task-priority-up.png@" + GetType ().Assembly.FullName;
				else if (name == "Down")
					return "task-priority-down.png@" + GetType ().Assembly.FullName;
				else
					return "task.png@" + GetType ().Assembly.FullName;
			}
		}
		
		public string Priority {
			get {
				if (name == "Low")
					return "3";
				else if (name == "Medium")
					return "2";
				else if (name == "High")
					return "1";
				else if (name == "Up")
					return "up";
				else if (name == "Down")
					return "down";
				else
					return "N";
			}
		}
		
	}
}

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
using System.Collections.Generic;


using Do.Universe;

namespace SqueezeCenter
{

	public abstract class RadioItem : Item
	{
		protected RadioSubItem[] children;
				
		public virtual RadioSubItem[] Children
		{
			get { return this.children; }
			set 
			{
				System.Threading.Interlocked.Exchange<RadioSubItem[]> (ref this.children, value);				
			}
		}
		
		public abstract RadioItem Parent {get;}
		
		public override string Icon
		{
			get { return "radio.png@" + this.GetType ().Assembly.FullName; }			
		}		
	}
		
	public class RadioSuperItem : RadioItem
	{		
		readonly string cmd, name;
		bool childrenSet = false;
		
		public RadioSuperItem (string cmd, string name)
		{
			this.cmd = cmd;
			this.name = name;
			this.children = new RadioSubItem[0];
		}
		
		public string Command {
			get {				
				return this.cmd;
			}
		}
						
		public override string Name {
			get {
				return this.name;
			}
		}
			
		public override string Description {
			get {
				return "Radio" ;
			}
		}
		
		public override RadioSubItem[] Children {
			get { return base.Children; }
			set 
			{ 
				base.Children = value;
				this.childrenSet = true;
			}
		}

		public bool IsLoadedRecursive
		{
			get
			{
				if (!this.childrenSet)
					return false;
				foreach (RadioSubItem rmi in this.children) {
					if (!rmi.IsLoadedRecursive)
						return false;
				}
				return true;
			}
		}

		public override RadioItem Parent
		{
			get { return null; }
		}

		public RadioSubItem[] GetChildrenRecursive () 
		{
			List<RadioSubItem> result = new List<RadioSubItem> ();
			foreach (RadioSubItem rmi in children) {
				result.Add (rmi);
				rmi.CopyChildrenRecursive (result);
			}
			return result.ToArray ();
		}
	}
	
	public class RadioSubItem : RadioItem
	{
		readonly int id;
		readonly RadioItem parent;
		readonly string name;
		readonly bool hasItems;
				
		public RadioSubItem (RadioItem parent, int id, string name, bool hasItems)
		{
			this.parent = parent;
			this.id = id;
			this.name = name;
			this.hasItems = hasItems;
			this.children = new RadioSubItem[0];
		}
						
		public override string Name {
			get {
				return this.name;
			}
		}
			
		public override string Description {
			get {
				RadioItem parent = this.parent;
				string result = string.Empty;
				while (parent != null) {
					result = parent.Name + (result.Length == 0 ? string.Empty : " → " + result);
					parent = parent.Parent;
				}
				return result;
			}
		}

		public override RadioItem Parent
		{
			get { return this.parent; }
		}
		
		public int Id
		{
			get { return this.id; }
		}

		public bool HasItems
		{
			get { return this.hasItems; }
		}
		
		public RadioSuperItem GetSuper ()
		{
			RadioItem r = this;
			while (r != null) {
				if (r is RadioSuperItem)
					return r as RadioSuperItem;
				r = r.Parent;
			}
			return null;
		}

		public string IdPath
		{
			get {
				if (! (this.parent is RadioSubItem)) {					
					return this.Id.ToString ();
				}
				return string.Format ("{0}.{1}", (this.parent as RadioSubItem).IdPath, this.Id); 				
			}
		}
		
		public void CopyChildrenRecursive (List<RadioSubItem> target)
		{
			foreach (RadioSubItem rmi in children) {
				target.Add (rmi);
				rmi.CopyChildrenRecursive (target);
			}
		}
		
		public bool IsLoadedRecursive
		{
			get
			{
				if (this.hasItems) {					
					foreach (RadioSubItem rmi in this.children) {
						if (!rmi.IsLoadedRecursive) {
							return false;
						}
					}
				}
				return true;
			}
		}
	}
}

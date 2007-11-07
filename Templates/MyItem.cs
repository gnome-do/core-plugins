//  MyItem.cs

using System;
using Do.Universe;

namespace MyCommandNamespace
{

	public class MyItem : IItem
	{
		
		public string Name {
			get {
				return "The name of the item.";
			}
		}
		
		public string Description {
			get {
				return "A description of the item.";
			}
		}
		
		public string Icon {
			get {
				return "my-item-icon";
			}
		}
		
	}
}
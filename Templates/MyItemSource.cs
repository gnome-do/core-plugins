//  MyItemSource.cs
//
//  This is a simple item source called MyItemSource. If you're going to
//  create an ItemSource addin, you can start by copying this code
//  to begin your own item source class.

using System;
using System.Collections.Generic;

using Do.Universe;

namespace MyAddinNamespace
{

	public class MyItemSource : IItemSource
	{
		
		public string Name {
			get {
				return "The name of the item source.";
			}
		}
		
		public string Description {
			get {
				return "A description of the item source.";
			}
		}
		
		public string Icon {
			get {
				return "my-item-source-icon";
			}
		}
		
		public Type[] SupportedItemTypes {
			get {
			return new Type[] {
					/* Supported item types go here. */
				};
			}
		}

		public ICollection<IItem> Items {
			get {
				return null;
			}
		}
		
		public ICollection<IItem> ChildrenOfItem (IItem item)
		{
			return null;
		}
		
		public void UpdateItems ()
		{
		}
		
	}
}
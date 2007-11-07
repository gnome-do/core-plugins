//  MyCommand.cs
//
//  This is a simple command called MyCommand. If you're going to
//  create a Command addin, you can start by copying this code
//  to begin your own command class.

using System;
using Do.Universe;

namespace MyAddinNamespace
{

	public class MyCommand : ICommand
	{
		
		public string Name {
			get {
				return "The name of the command.";
			}
		}
		
		public string Description {
			get {
				return "A description of the command.";
			}
		}
		
		public string Icon {
			get {
				return "my-command-icon";
			}
		}
		
		public Type[] SupportedItemTypes {
			get {
			return new Type[] {
					/* Supported item types go here. */
				};
			}
		}

		public Type[] SupportedItemTypes {
			get {
			return new Type[] {
					/* Supported modifier item types go here. */
				};
			}
		}

		public bool SupportsItem (IItem item)
		{
			return true;
		}
		
		public bool SupportsModifierItemForItems (IItem[] items, IItem modItem)
		{
			return true;
		}
		
		public void Perform (IItem[] items, IItem[] modItems)
		{
			// This is where the magic happens.
		}
		
	}
}
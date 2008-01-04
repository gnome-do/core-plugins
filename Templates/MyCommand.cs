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
				// Look in /usr/share/icons/Tango/* for some great icons
				// to use. Just return the name of the icon, without the
				// path or extension.
				return "my-command-icon";
			}
		}
		
		public Type[] SupportedItemTypes {
			get {
			return new Type[] {
					// Supported item types go here.
				};
			}
		}
		
		public bool SupportsItem (IItem item)
		{
			// Here, you can filter the items you support further. The argument
			// is non-null is a subtype of one of your SupportedItemTypes.
			return true;
		}
		
		public IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			// This is where the magic happens.
			
			// If you return a non-null, non-empty array of IItems, they will
			// be presented to the user in the main window for a new search.
			// You can return null if you don't want to provide any items to
			// the user as a result of performing your command.
			return null;
		}
		
		//////////////////////////////////////////////////////////////////////
		/// If you don't plan on supporting modifier items (third pane),   ///
		/// you can safely leave all of the following methods as they are. ///
		//////////////////////////////////////////////////////////////////////

		public Type[] SupportedModifierItemTypes {
			get {
			return new Type[] {
					// Supported modifier item types go here.
				};
			}
		}

		public bool ModifierItemsOptional {
			get { return true; }
		}
				
		public bool SupportsModifierItemForItems (IItem[] items, IItem modItem)
		{
			return true;
		}
		
		public IItem[] DynamicModifierItemsForItem (IItem item)
		{
			return null;
		}
		
	}
}
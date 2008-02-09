using System;
using System.Diagnostics;

using Do.Universe;

namespace EyeOfGNOME
{
	public class PlaySlideshowAction : AbstractAction
	{
		public PlaySlideshowAction ()
		{
		}

		public override string Name
		{
			get { return "Play Slideshow"; }
		}

		public override string Description
		{
			get { return "Plays a slideshow of images in a folder."; }
		}

		public override string Icon
		{
			get { return "eog"; }
		}

		public override Type[] SupportedItemTypes
		{
			get {
				return new Type[] {
					typeof (IFileItem),
				};
			}
		}

		public override bool SupportsItem (IItem item)
		{
			return System.IO.Directory.Exists ((item as IFileItem).Path);
		}

		public override IItem[] Perform (IItem[] items, IItem[] modifierItems)
		{
			string path;

			path = (items[0] as IFileItem).Path;
			try {
				Process.Start ("eog", "--slide-show \"" + path + "\"");
			} catch (Exception e) {
				Console.Error.WriteLine
					("Could not play slideshow for {0}: {1}", path, e.Message);
			}
			return null;
		}
	}
}

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
 
using Do.Universe;
using Do.Platform;
using Do.Universe.Common;
 
namespace Do.Plugins {
 
    public class QalculateAction : Act {
 
        public override string Name {
            get { return "Qalculate";  }
        }
 
        public override string Description {
            get { return "Perform a calculation using Qalculate"; }
        }
 
        public override string Icon {
            get { return "accessories-calculator"; }
        }
 
		public override IEnumerable<Type> SupportedItemTypes {
			get { yield return typeof (ITextItem); }
		}
 
        public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
        {
            string expression = (items.First () as ITextItem).Text;
			string result;
 			
			ProcessStartInfo ps = new ProcessStartInfo ("qalc", expression);
			ps.UseShellExecute = false;
			ps.RedirectStandardOutput = true;
			Process p = Process.Start (ps);
 
			result = p.StandardOutput.ReadToEnd ();
			p.WaitForExit ();
			
			yield return new TextItem (result);
        }
    }
}
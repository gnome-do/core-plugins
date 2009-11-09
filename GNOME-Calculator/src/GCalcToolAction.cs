/**
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
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;

using Mono.Addins;

using Do.Universe;
using Do.Universe.Common;

namespace GNOME.GCalcTool {

    public class GCalcToolAction : Act {

        public GCalcToolAction() {
        }

        public override string Name {
            get { return "GNOME Calculator";  }
        }

        public override string Description {
            get { return AddinManager.CurrentLocalizer.GetString ("Use GNOME Calculator to make simple calculations."); }
        }

        public override string Icon {
            get { return "accessories-calculator"; }
        }

        public override IEnumerable<Type> SupportedItemTypes {
            get { yield return typeof (ITextItem); }
        }

        public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems) {
            string expression = (items.First () as ITextItem).Text;
            string result = "";
            string error = AddinManager.CurrentLocalizer.GetString("Sorry I couldn't understand your expression, try another way");

            //TODO found something that works
            //string pattern = @"([\(]*([\(]*[\-][-]*[0-9]*[\.\,]*[0-9]([\+\-/\*\^][\-][-]*[0-9]*[\.\,]*[0-9])+[\)]*)+([\+\-/\*\^]([\(]*[\-][-]*[0-9]*[\.\,]*[0-9]([\+\-/\*\^][\-][-]*[0-9]*[\.\,]*[0-9])+[\)]*)+)+[\)]*)+";
            //if (!Regex.IsMatch(expression, pattern)) {
            //	yield return new TextItem (error);
            //}

            ProcessStartInfo ps = new ProcessStartInfo ("gcalctool", "-s " + expression);
            ps.UseShellExecute = false;
            ps.RedirectStandardOutput = true;
            Process p = Process.Start (ps);

            result = p.StandardOutput.ReadToEnd ();
            p.WaitForExit ();
            if (p.ExitCode != 0) {
                result = error;
            }

            yield return new TextItem (result);
        }
    }
}

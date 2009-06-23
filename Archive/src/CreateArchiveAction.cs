// CreateArchiveAction.cs
//
//  Copyright (C) 2008 Guillaume Béland
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//


using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Mono.Addins;
using Do.Universe;

namespace Archive {
        
        public enum ArchiveType { GZIP, BZIP2, TAR, ZIP };
        
        public class ArchiveAction : Act {
                
                public ArchiveAction()
                {
                }
                
                public override string Name {
                        get { return AddinManager.CurrentLocalizer.GetString ("Create archive"); }
                }
                
                public override string Description {
                        get { return AddinManager.CurrentLocalizer.GetString ("Create an archive with the selected item"); }
                }
        
                public override string Icon {
                        get { return "file-roller"; }
                }
                
                public override IEnumerable<Type> SupportedItemTypes {
                        get {
                                return new Type[] {                             
                                        typeof (IFileItem),
                                };
                        }
                }
                
                public override bool SupportsItem (Item item) 
                {
                        return true;
                }
                                
                public override IEnumerable<Type> SupportedModifierItemTypes {
                        get {
                                return new Type[] {
                                        typeof(ArchiveTypeItem),
                                };
                        }
                }
                
                public override bool ModifierItemsOptional {
                        get { return true; }
                }
                
                public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
                {
                        List<Item> items = new List<Item> ();
        
                        try {
                                items.Add(new ArchiveTypeItem (ArchiveType.GZIP));
                                items.Add(new ArchiveTypeItem (ArchiveType.BZIP2));
                                items.Add(new ArchiveTypeItem (ArchiveType.TAR));
                                //items.Add(new ArchiveTypeItem (ArchiveType.ZIP));
                                return items.ToArray();
                        } catch {
                                return null;
                        }
                        
                }
                
                public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
                {
                        if ( modItems.Any ())
                                Archive ((items.First () as IFileItem), (modItems.First () as ArchiveTypeItem).ArchiveType);
                        else
                                Archive ((items.First () as IFileItem), 0);
                        return null;
                }
                
                private void Archive ( IFileItem item, int archiveType)
                {
                        string path = null;
                        string file = null;
                               
                        if ( Directory.Exists(item.Path) ) {
                                path = item.Path.Replace(item.Name,"");
                                file = item.Name;
                        }
                        else {
                                path = item.Path.Replace(item.Name,"");
                                file = String.Concat (System.IO.Path.GetFileName (path.Substring (0, path.Length -1)),
                                                        "/",
                                                        item.Name);
                               
                                path = item.Path.Replace(file, "");
                        }                       
                
                        path = EscapeString (path);
                        file = EscapeString (file);

                        switch (archiveType) {
                                case (int)ArchiveType.GZIP:
                                        Process.Start (string.Format ("tar -czf {0} -C {1} {2}", String.Concat (item.Name ,".tar.gz"), path, file));  
                                        break;
                                case (int)ArchiveType.BZIP2:
                                        Process.Start (string.Format ("tar -cjf {0} -C {1} {2}", String.Concat (item.Name ,".tar.bz2"), path, file));
                                        break;
                                case (int)ArchiveType.TAR:
                                        Process.Start (string.Format ("tar -cf {0} -C {1} {2}", String.Concat (item.Name ,".tar"), path, file));
                                        break;
                                case (int)ArchiveType.ZIP:
                                        Process.Start (string.Format ("zip {0} {1} ", file , path));
                                        break;
                                default:
                                        Process.Start (string.Format ("tar -czf {0} {1} ", String.Concat (file,".tar.gz"), file));
                                        break;
                        }
                }

                private string EscapeString (string str)
                {
                        return str.Replace (" ", "\\ ")
                                  .Replace ("'", "\\'");
                }
        }
}

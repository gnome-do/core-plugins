// ExtractArchiveAction.cs
//
//  Copyright (C) 2008 [name of author]
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

using Do.Universe;

namespace Archive {
        
        
        public class ExtractAction : AbstractAction {
                
                public ExtractAction()
                {
                }
                
                public override string Name {
                        get { return "Extract archive"; }
                }
                
                public override string Description {
                        get { return "Extract an archive to a given folder"; }
                }
        
                public override string Icon {
                        get { return "file-roller"; }
                }
                
                public override Type[] SupportedItemTypes {
                        get {
                                return new Type[] {                             
                                        typeof (IFileItem),
                                };
                        }
                }
                
                public override bool SupportsItem (IItem item) 
                {
                        return IsArchive (item as IFileItem);
                }
                        
                public override bool SupportsModifierItemForItems (IItem [] items, IItem modItem)
                {
                        return FileItem.IsDirectory (modItem as IFileItem);
                }

                public override Type[] SupportedModifierItemTypes {
                        get {
                                return new Type[] {
                                        typeof(IFileItem),
                                };
                        }
                }
                
                public override bool ModifierItemsOptional {
                        get { return false; }
                }

                public override IItem[] DynamicModifierItemsForItem (IItem item)
                {
                        return null;       
                }
                
                public override IItem[] Perform (IItem[] items, IItem[] modItems)
                {
                        ExtractArchive ( (items[0] as IFileItem), (modItems[0] as IFileItem));
                        return null;
                }

                private bool IsArchive (IFileItem item)
                {
                        return item.Path.EndsWith(".tar.gz") ||
                               item.Path.EndsWith (".tar.bz2") ||
                               item.Path.EndsWith (".tar") ;
                
                }

                private void ExtractArchive ( IFileItem archive, IFileItem where)
                {
                        if ( archive.Name.EndsWith ("tar.gz"))
                                Process.Start (String.Format ("tar -xzf {0} -C {1}", EscapeString(archive.Path), EscapeString(where.Path)));  
                        else if ( archive.Name.EndsWith ("tar.bz2"))
                                Process.Start (String.Format ("tar -xjf {0} -C {1}", EscapeString(archive.Path), EscapeString(where.Path)));
                        else if (archive.Name.EndsWith ("tar"))
                                Process.Start (String.Format ("tar -xf {0} -C {1}", EscapeString (archive.Path), EscapeString(where.Path)));
                                               
                }

                private string EscapeString (string str)
                {
                        return str.Replace (" ", "\\ ")
                                  .Replace ("'", "\\'");
                }
        }
}

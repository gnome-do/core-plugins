// ArchiveTypeItem.cs
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

using Do.Universe;
using Archive;

namespace Archive {
   
        public class ArchiveTypeItem : IItem
        {
                private string archiveName;
                private string archiveDescription;
                private int archiveId;
                private string archiveIcon;     
        
                public ArchiveTypeItem (Archive.ArchiveType archiveType)
                {
                        this.archiveId = (int)archiveType;
                        
                        switch (archiveId) {
                                case (int)Archive.ArchiveType.GZIP:
                                        this.archiveName = "Tar.gz";
                                        this.archiveDescription = "Tar compressed with GZIP (.tar.gz)";    
                                        this.archiveIcon = "tgz";
                                        break;
                                case (int)Archive.ArchiveType.BZIP2:
                                        this.archiveName = "Tar.bz2";
                                        this.archiveDescription = "Tar compressed with BZIP2 (.tar.bz2)"; 
                                        this.archiveIcon = "tar";
                                        break;
                                case (int)Archive.ArchiveType.TAR:
                                        this.archiveName = "Tar";
                                        this.archiveDescription = "Tar uncompressed (.tar)";
                                        this.archiveIcon = "tar";
                                        break;
                                case (int)Archive.ArchiveType.ZIP:
                                        this.archiveName = ".zip";
                                        this.archiveDescription = "Zip (.zip)";
                                        this.archiveIcon = "zip";
                                        break;   
                                default:
                                        this.archiveName = ".tar.gz";
                                        this.archiveDescription = "Tar compressed with GZIP (.tar.gz)";    
                                        this.archiveIcon = "tgz";
                                        break;
                        }
                }
                
                public int ArchiveType {
                        get { return archiveId; }
                }
                
                public string Name {
                        get { return archiveName; }
                }
                
                public string Description {
                        get { return archiveDescription; }
                }
                
                public string Icon {
                        get { return archiveIcon; }
                }
                
        }
}

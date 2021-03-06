//  FolderItem.cs
//  
//  GNOME Do is the legal property of its developers, whose names are too numerous
//  to list here.  Please refer to the COPYRIGHT file distributed with this
//  source distribution.
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;

using Mono.Addins;

using Do.Universe;

namespace Firefox
{
	public class FolderItem : Item 
	{
		string folder_name;
		
		public FolderItem (string folderName, int id)
		{
			Id = id;
			folder_name = folderName;
		}
		
		public FolderItem (string folderName, int id, int parentId)
		{
			Id = id;
			ParentId = parentId;
			folder_name = folderName;
		}
		
		public override string Name {
			get { return folder_name; } 
		}
		
		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("A Firefox Bookmarks Directory"); }
		}
		
		public override string Icon {
			get { return "folder"; }
		}
		
		public int Id { get; private set; }
		public int ParentId { get; private set; }
	}
}

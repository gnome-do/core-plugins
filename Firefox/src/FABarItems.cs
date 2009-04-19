//  FABarItems.cs
//  GNOME Do is the legal property of its developers, whose names are too
//  numerous to list here.  Please refer to the COPYRIGHT file distributed with
//  this source distribution.
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

using System;
using Do.Universe;
using Do.Universe.Common;


namespace Firefox
{
	public class BrowseBookmarkItem : Item 
	{
		public override string Name { get { return "Bookmarks"; } }
		public override string Description { get { return "Browse Firefox Bookmarks"; } }
		public override string Icon { get { return "firefox-3.0"; } }
	}
	
	public class BrowseHistoryItem : Item 
	{
		public override string Name { get { return "History"; } }
		public override string Description { get { return "Browse Firefox History."; } }
		public override string Icon { get { return "firefox-3.0"; } }
	}
	
	public class FolderItem : Item 
	{
		string title;
		int id;
		int parentID;
		
		public FolderItem (string title, int id)
		{
			this.title = title;
			this.id = id;
		}
		
		public FolderItem (string title, int id, int parentID)
		{
			this.title = title;
			this.id = id;
			this.parentID = parentID;
		}
		
		public override string Name { get { return title; } }
		public override string Description { get { return "Mozilla Firefox Bookmark Directory"; } }
		public override string Icon { get { return "folder"; } }
		public int Id { get { return id; } }
		public int ParentId { get { return parentID; } }
	}
	
	public class PlaceItem : Item, IBookmarkItem
	{
		string title, url;
		int? parentID;
		
		public PlaceItem (string title, string url)
		{
			this.title = title;
			this.url = url;
		}
		
		public PlaceItem (string title, string url, int parentID)
		{
			this.title = title;
			this.url = url;
			this.parentID = parentID;
		}

		public override string Name { get { return title; } }
		public override string Description { get { return url; } }
		public override string Icon { get { return "www"; } }
		public string Url { get { return url; } }
		public int? ParentId { get { return parentID; } }
	}
}

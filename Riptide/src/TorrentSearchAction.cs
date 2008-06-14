// TorrentSearchAction.cs
//
//GNOME Do is the legal property of its developers. Please refer to the
//COPYRIGHT file distributed with this
//source distribution.
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
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

using Do.Universe;

namespace Do.Riptide
{
	
	
	public class TorrentSearchAction : AbstractAction
	{
		
		public TorrentSearchAction()
		{

		}
		
		public override string Name {
			get { return "Search For Torrents"; }
		}

		public override string Description {
			get { return "Search the internet for torrents"; }
		}
		
		public override string Icon {
			get { return "gnome-searchtool"; }
		}

		public override Type[] SupportedItemTypes {
			get { return new Type[] { typeof (ITextItem) }; }
		}

		
		public override IItem[] Perform (IItem[] items, IItem[] modItems)
		{
			List<IItem> outItems;
			outItems = new List<IItem> ();
			string search;
			search = HttpUtility.UrlEncode ((items[0] as ITextItem).Text);
			
			search = "http://isohunt.com/js/rss/" + search;
			
			WebClient client = new WebClient ();
			Stream stream = client.OpenRead (search);
			
			XmlDocument xdoc = new System.Xml.XmlDocument ();
			xdoc.Load (stream);
			
			XmlNodeList nodes;
			nodes = xdoc.SelectNodes ("/rss/channel/item");
			
			TorrentResultItem result;
			Regex regx;
			MatchCollection mc;
			string description, seeds, leeches, size;
			foreach (XmlNode n in nodes) {
				description = n.SelectSingleNode("description").InnerText;
				
				mc = Regex.Matches (description, "Seeds: [0-9]*");
				seeds = mc[0].Value;
				
				mc = Regex.Matches (description, "Leechers: [0-9]*");
				leeches = mc[0].Value;
				
				mc = Regex.Matches (description, "Size: [0-9]*.[0-9]* MB");
				size = mc[0].Value;
				
				result = new TorrentResultItem (n.SelectSingleNode("title").InnerText);
				
				result.URL = n.SelectSingleNode("enclosure").Attributes[0].InnerText;
				result.Seeds    = Convert.ToInt32 (seeds.Substring (7));
				result.Leechers = Convert.ToInt32 (leeches.Substring (10));
				result.Size     = size.Substring (6);
				
				outItems.Add (result);
			}
			
			outItems.Sort ();
			
			if (outItems.Count == 0) {
				outItems.Add (new TextItem ("No Torrent Results Found For " + 
				                            (items[0] as ITextItem).Text));
			}
			
			return outItems.ToArray ();
		}

	}
}












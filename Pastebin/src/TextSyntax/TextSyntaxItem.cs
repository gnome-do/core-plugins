//  TextSyntaxItem.cs
//
//  GNOME Do is the legal property of its developers, whose names are too
//  numerous to list here.  Please refer to the COPYRIGHT file distributed with
//  this source distribution.
//
//  This program is free software: you can redistribute it and/or modify it
//  under the terms of the GNU General Public License as published by the Free
//  Software Foundation, either version 3 of the License, or (at your option)
//  any later version.
//
//  This program is distributed in the hope that it will be useful, but WITHOUT
//  ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
//  FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
//  more details.
//
//  You should have received a copy of the GNU General Public License along with
//  this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Xml.Serialization;

using Do.Universe;

namespace Pastebin
{
	public class TextSyntaxItem : Item, ITextSyntaxItem
	{		
		public TextSyntaxItem ()
		{
		}
		
		[XmlIgnore]
		public override string Name { get { return SerializableName; } }
		
		[XmlIgnore]
		public override string Description { get { return SerializableDescription; } }
		
		[XmlIgnore]
		public override string Icon { get { return SerializableIcon; } }

		public string Syntax { get; set; }
		
		public string SerializableName { get; set; }
		
		public string SerializableDescription { get; set; }
		
		public string SerializableIcon { get; set; }
	}
}

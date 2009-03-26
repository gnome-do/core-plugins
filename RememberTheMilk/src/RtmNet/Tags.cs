using System;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace RtmNet
{
	/// <summary>
	/// (obsolete?) A simple tag class, containing a tag name and optional count (for <see cref="Rtm.TagsGetListUserPopular()"/>)
	/// </summary>
//	public class Tag
//	{
//		private string _tagName;
//		private int _count;
//
//		/// <summary>
//		/// The name of the tag.
//		/// </summary>
//		public string TagName
//		{
//			get { return _tagName; }
//		}
//
//		/// <summary>
//		/// The poularity of the tag. Will be 0 where the popularity is not retreaved.
//		/// </summary>
//		public int Count
//		{
//			get { return _count; }
//		}
//
//		internal Tag(XmlNode node)
//		{
//			if( node.Attributes["count"] != null ) _count = Convert.ToInt32(node.Attributes["count"].Value);
//			_tagName = node.InnerText;
//		}
//
//		internal Tag(string tagName, int count)
//		{
//			_tagName = tagName;
//			_count = count;
//		}
//	}
	
	/// <summary>
	/// Contains a list of <see cref="Tag"/> items for a given <see cref="TaskSeries"/>.
	/// </summary>
	[System.Serializable]
	public class Tags
	{
		/// <summary>
		/// An array of <see cref="Tag"/> items for the <see cref="TaskSeries"/>.
		/// </summary>
		[XmlElement("tag", Form=XmlSchemaForm.Unqualified)]
		public Tag[] TagCollection = new Tag[0];
	}
	
	[System.Serializable]
	public class Tag
	{
		private string text;	
		
		/// <summary>
		/// The text of the tag
		/// </summary>
		[XmlText()]
		public string Text { get { return text; } set { text = value; } }
	}
}

using System;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace RtmNet
{
	/// <summary>
	/// Contains a list of <see cref="Tag"/> items for a given <see cref="TaskSeries"/>.
	/// </summary>
	[System.Serializable]
	public class Tags
	{
		/// <summary>
		/// An array of <see cref="Tag"/> items for the <see cref="TaskSeries"/>.
		/// </summary>
		[XmlElement ("tag", Form = XmlSchemaForm.Unqualified)]
		public Tag[] TagCollection = new Tag[0];
	}
	
	[System.Serializable]
	public class Tag
	{
		private string text;
		
		/// <summary>
		/// The text of the tag
		/// </summary>
		[XmlText ()]
		public string Text { get { return text; } set { text = value; } }
	}
}

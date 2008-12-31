// Note.cs created with MonoDevelop
// User: calvin at 11:28 PM 2/12/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace RtmNet
{
	/// <summary>
	/// Contains a list of <see cref="Contact"/> items for a given user.
	/// </summary>
	[System.Serializable]
	public class Notes
	{
		/// <summary>
		/// An array of <see cref="Contact"/> items for the user.
		/// </summary>
		[XmlElement("note", Form=XmlSchemaForm.Unqualified)]
		public Note[] NoteCollection = new Note[0];
	}

	/// <remarks/>
	[System.Serializable]
	public class Note
	{
		private string id;
		private string rawCreated;
		private string rawModified;
		private string title;	
		private string text;		
		private DateTime created = DateTime.MinValue;
		private DateTime modified = DateTime.MinValue;

		/// <remarks/>
		[XmlAttribute("id", Form=XmlSchemaForm.Unqualified)]
		public string ID { get { return id; } set { id = value; } }
    
		/// <remarks/>
		[XmlAttribute("created", Form=XmlSchemaForm.Unqualified)]
		public string RawCreated
		{
			get { return rawCreated; }
			set {
				if(value.Length > 0) {
					rawCreated = value;
					created = Utils.DateStringToDateTime(rawCreated);
				}
			}
		}
    
		/// <summary>
		/// Converts the raw created field to a <see cref="DateTime"/>.
		/// </summary>	
		[XmlIgnore]
		public DateTime Created
		{
			get { return created; }
			set { created = value; }
		}

		/// <remarks/>
		[XmlAttribute("modified", Form=XmlSchemaForm.Unqualified)]
		public string RawModified
		{
			get { return rawModified; }
			set {
				if(value.Length > 0) {
					rawModified = value;
					modified = Utils.DateStringToDateTime(rawModified);
				}
			}
		}
		/// <summary>
		/// Converts the raw modified field to a <see cref="DateTime"/>.
		/// </summary>	
		[XmlIgnore]
		public DateTime Modified
		{
			get { return modified; }
			set { modified = value; }
		}


		/// <summary>
		/// Is this contact marked as a friend contact?
		/// </summary>
		[XmlAttribute("title", Form=XmlSchemaForm.Unqualified)]
		public string Title { get { return title; } set { title = value; } }		



		/// <summary>
		/// The text of the note
		/// </summary>
		[XmlText()]
		public string Text { get { return text; } set { text = value; } }		

	}
}

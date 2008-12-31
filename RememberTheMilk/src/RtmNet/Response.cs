using System;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace RtmNet
{
	/// <summary>
	/// The root object returned byRtm. Used with Xml Serialization to get the relevant object.
	/// It is internal to the RtmNet API Library and should not be used elsewhere.
	/// </summary>
	[XmlRoot("rsp", Namespace="", IsNullable=false)]
	[Serializable]
	public class Response 
	{

		/// <remarks/>
		[XmlElement("contacts", Form=XmlSchemaForm.Unqualified)]
		public Contacts Contacts;

		/// <remarks/>
		[XmlElement("lists", Form=XmlSchemaForm.Unqualified)]
		public Lists Lists;

		/// <remarks/>
		[XmlElement("tasks", Form=XmlSchemaForm.Unqualified)]
		public Tasks Tasks;

		/// <remarks/>
		[XmlAttribute("stat", Form=XmlSchemaForm.Unqualified)]
		public ResponseStatus Status;
		
		/// <remarks/>
		[XmlElement("list", Form=XmlSchemaForm.Unqualified)]
		public List List;
		
		/// <remarks/>
		[XmlElement("timeline", Form=XmlSchemaForm.Unqualified)]
		public string Timeline;

		/// <remarks/>
		[XmlElement("note", Form=XmlSchemaForm.Unqualified)]
		public Note Note;
		
		/// <summary>
		/// If an error occurs the Error property is populated with 
		/// a <see cref="ResponseError"/> instance.
		/// </summary>
		[XmlElement("err", Form=XmlSchemaForm.Unqualified)]
		public ResponseError Error;

		/// <summary>
		/// A <see cref="Method"/> instance.
		/// </summary>
		[XmlElement("method", Form=XmlSchemaForm.Unqualified)]
		public Method Method;

		/// <summary>
		/// If using Rtm.test.echo this contains all the other elements not covered above.
		/// </summary>
		/// <remarks>
		/// t is an array of <see cref="XmlElement"/> objects. Use the XmlElement Name and InnerXml properties
		/// to get the name and value of the returned property.
		/// </remarks>
		[XmlAnyElement(), NonSerialized()]
		public XmlElement[] AllElements;
	}

	/// <summary>
	/// If an error occurs then Rtm returns this object.
	/// </summary>
	[System.Serializable]
	public class ResponseError
	{
		/// <summary>
		/// The code or number of the error.
		/// </summary>
		/// <remarks>
		/// 100 - Invalid Api Key.
		/// 99  - User not logged in.
		/// Other codes are specific to a method.
		/// </remarks>
		[XmlAttribute("code", Form=XmlSchemaForm.Unqualified)]
		public int Code;

		/// <summary>
		/// The verbose message matching the error code.
		/// </summary>
		[XmlAttribute("msg", Form=XmlSchemaForm.Unqualified)]
		public string Message;
	}

	/// <summary>
	/// The status of the response, either ok or fail.
	/// </summary>
	public enum ResponseStatus
	{
		/// <summary>
		/// An unknown status, and the default value if not set.
		/// </summary>
		[XmlEnum("unknown")]
		Unknown,

		/// <summary>
		/// The response returns "ok" on a successful execution of the method.
		/// </summary>
		[XmlEnum("ok")]
		OK,
		/// <summary>
		/// The response returns "fail" if there is an error, such as invalid API key or login failure.
		/// </summary>
		[XmlEnum("fail")]
		Failed
	}
}
using System;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace RtmNet
{
	/// <summary>
	/// Contains a list of <see cref="Contact"/> items for a given user.
	/// </summary>
	[System.Serializable]
	public class Tasks
	{
		/// <summary>
		/// An array of <see cref="Contact"/> items for the user.
		/// </summary>
		[XmlElement ("list", Form = XmlSchemaForm.Unqualified)]
		public List[] ListCollection = new List[0];
	}


	/// <remarks/>
	[System.Serializable]
	public class TaskSeries
	{
		private string id = System.Guid.NewGuid ().ToString ();
		private string name;
		private string rawCreated;
		private string rawModified;
		private DateTime created = DateTime.MinValue;
		private DateTime modified = DateTime.MinValue;

		/// <remarks/>
		[XmlAttribute ("id", Form = XmlSchemaForm.Unqualified)]
		public string TaskSeriesID { get { return id; } set { id = value; } }


		/// <remarks/>
		[XmlAttribute ("created", Form = XmlSchemaForm.Unqualified)]
		public string RawCreated {
			get { return rawCreated; }
			set {
				if (value.Length > 0) {
					rawCreated = value;
					created = Utils.DateStringToDateTime (rawCreated);
				}
			}
		}

		/// <summary>
		/// Converts the raw created field to a <see cref="DateTime"/>.
		/// </summary>
		[XmlIgnore]
		public DateTime Created {
			get { return created; }
			set { created = value; }
		}
		
		/// <remarks/>
		[XmlAttribute ("modified", Form = XmlSchemaForm.Unqualified)]
		public string RawModified {
			get { return rawModified; }
			set {
				if (value.Length > 0) {
					rawModified = value;
					modified = Utils.DateStringToDateTime (rawModified);
				}
			}
		}
		
		/// <summary>
		/// Converts the raw modified field to a <see cref="DateTime"/>.
		/// </summary>
		[XmlIgnore]
		public DateTime Modified {
			get { return modified; }
			set { modified = value; }
		}
		
		/// <remarks/>
		[XmlAttribute ("name", Form = XmlSchemaForm.Unqualified)]
		public string Name { get { return name; } set { name = value; } }
		
		/// <remarks/>
		[XmlAttribute ("source", Form = XmlSchemaForm.Unqualified)]
		public string source;
		
		[XmlElement ("task", Form = XmlSchemaForm.Unqualified)]
		public Task[] TaskCollection = new Task[0];
		
		/// <remarks/>
		[XmlElement ("tags", Form = XmlSchemaForm.Unqualified)]
		public Tags Tags;
		
		/// <remarks/>
		[XmlElement ("notes", Form = XmlSchemaForm.Unqualified)]
		public Notes Notes;
		
		/// <remarks/>
		[XmlAttribute ("url", Form = XmlSchemaForm.Unqualified)]
		public string TaskURL;   
		
		/// <remarks/>
		[XmlAttribute ("location_id", Form = XmlSchemaForm.Unqualified)]
		public string LocationID; 
	}
	
	/// <remarks/>
	[System.Serializable]
	public class Task
	{
		private string id;
		private string rawDue;
		private string rawAdded;
		private string rawCompleted;
		private string rawDeleted;
		private DateTime due = DateTime.MinValue;
		private DateTime added = DateTime.MinValue;
		private DateTime completed = DateTime.MinValue;
		private DateTime deleted = DateTime.MinValue;
		
		/// <remarks/>
		[XmlAttribute ("id", Form = XmlSchemaForm.Unqualified)]
		public string TaskID { get { return id; } set { id = value; } }
		
		/// <remarks/>
		[XmlAttribute ("due", Form = XmlSchemaForm.Unqualified)]
		public string RawDue {
			get { return rawDue; }
			set {
				if (value.Length > 0) {
					rawDue = value;
					due = Utils.DateStringToDateTime (rawDue);
				}
			}
		}
		
		/// <summary>
		/// Converts the raw created field to a <see cref="DateTime"/>.
		/// </summary>	
		[XmlIgnore]
		public DateTime Due {
			get { return due; }
			set { due = value; }
		}
		
		/// <summary>
		/// Is this contact marked as a friend contact?
		/// </summary>
		[XmlAttribute ("has_due_time", Form = XmlSchemaForm.Unqualified)]
		public int HasDueTime;
		
		/// <remarks/>
		[XmlAttribute ("added", Form = XmlSchemaForm.Unqualified)]
		public string RawAdded {
			get { return rawAdded; }
			set {
				if (value.Length > 0) {
					rawAdded = value;
					added = Utils.DateStringToDateTime (rawAdded);
				}
			}
		}
		
		/// <value>
		/// Holds the date time for when the task was added
		/// </value>
		[XmlIgnore]
		public DateTime Added {
			get { return added; }
			set { added = value; }
		}
		
		/// <remarks/>
		[XmlAttribute("completed", Form = XmlSchemaForm.Unqualified)]
		public string RawCompleted {
			get { return rawCompleted; }
			set {
				if (value.Length > 0) {
					rawCompleted = value;
					completed = Utils.DateStringToDateTime (rawCompleted);
				}
			}
		}
		
		/// <summary>
		/// Converts the raw created field to a <see cref="DateTime"/>.
		/// </summary>
		[XmlIgnore]
		public DateTime Completed {
			get { return completed; }
			set { completed = value; }
		}
		
		/// <remarks/>
		[XmlAttribute("deleted", Form = XmlSchemaForm.Unqualified)]
		public string RawDeleted {
			get { return rawDeleted; }
			set {
				if (value.Length > 0) {
					rawDeleted = value;
					deleted = Utils.DateStringToDateTime (rawDeleted);
				}
			}
		}
		
		/// <summary>
		/// Converts the raw created field to a <see cref="DateTime"/>.
		/// </summary>
		[XmlIgnore]
		public DateTime Deleted {
			get { return deleted; }
			set { deleted = value; }
		}
		
		/// <remarks/>
		[XmlAttribute ("priority", Form = XmlSchemaForm.Unqualified)]
		public string Priority;
		
		/// <remarks/>
		[XmlAttribute ("postponed", Form = XmlSchemaForm.Unqualified)]
		public string Postponed;
		
		/// <remarks/>
		[XmlAttribute ("estimate", Form = XmlSchemaForm.Unqualified)]
		public string Estimate;
	}
}

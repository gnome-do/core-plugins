using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Collections;

namespace RtmNet
{
	/// <summary>
	/// Contains a list of <see cref="Location"/> items for a given user.
	/// </summary>
	[System.Serializable]
	public class Locations 
	{
		/// <summary>
		/// An array of <see cref="Location"/> items for the user.
		/// </summary>
		[XmlElement("location", Form=XmlSchemaForm.Unqualified)]
		public Location[] locationCollection = new Location[0];
	}
	
	/// <summary>
	/// Contains details of a location for a particular user.
	/// </summary>
	[System.Serializable]
	public class Location
	{
		/// <summary>
		/// The id of the location.
		/// </summary>
		[XmlAttribute("id", Form=XmlSchemaForm.Unqualified)]
		public string ID;
    
		/// <summary>
		/// The name of the location
		/// </summary>
		[XmlAttribute("name", Form=XmlSchemaForm.Unqualified)]
		public string Name;
    
		/// <summary>
		/// The longitude of the location
		/// </summary>
		[XmlAttribute("longitude", Form=XmlSchemaForm.Unqualified)]
		public string Longitude;
    
		/// <summary>
		/// The latitute of the location
		/// </summary>
		[XmlAttribute("latitude", Form=XmlSchemaForm.Unqualified)]
		public string Latitude;
    
		/// <summary>
		/// The zoom level of the location
		/// </summary>
		[XmlAttribute("zoom", Form=XmlSchemaForm.Unqualified)]
		public int Zoom;
		
		/// <summary>
		/// The address string of the location
		/// </summary>
		[XmlAttribute("address", Form=XmlSchemaForm.Unqualified)]
		public string Address;
		
		/// <summary>
		/// The viewable attribute of the location
		/// </summary>
		[XmlAttribute("viewable", Form=XmlSchemaForm.Unqualified)]
		public int Viewable;
		
	}

}

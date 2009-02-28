
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Mono.Unix;

using Do.Platform;
using Do.Universe;

namespace Microblogging
{	
	public class FriendItem : Item
	{
		string name, photo, status;
		List<MicroblogStatus> statuses;
		
		public FriendItem (int id, string name) : 
			this (id, name, new MicroblogStatus (-1, Catalog.GetString ("No Status"), name, DateTime.Now))
		{
		}
		
		public FriendItem (int id, string name, MicroblogStatus status)
		{
			statuses = new List<MicroblogStatus> ();
			
			Id = id;
			this.name = name;
			this.status = status.Status;
			this.photo = Path.Combine (MicroblogClient.PhotoDirectory,  "" + id);
			statuses.Add (status);
		}
		
		public override string Name {
			get { return name; }
		}
		
		public override string Description {
			get { return status; }
		}
			
		public override string Icon {
			get {
				if (!string.IsNullOrEmpty (photo) && File.Exists (photo))
					return photo;
				return "stock_person";
			}
		}
		
		public int Id { get; private set; }
		
		public IEnumerable<MicroblogStatus> Statuses {
			get { return statuses; }
		}
		
		public void AddStatus (MicroblogStatus status)
		{
			this.status = status.Status;
			statuses.Add (status);
		}
	}
}

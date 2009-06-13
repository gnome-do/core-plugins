
using System;

using Do.Universe;

namespace Microblogging
{
	
	public class MicroblogStatus : Item
	{				
		public MicroblogStatus (long id, string status, string owner, DateTime time)
		{
			Id = id;
			Owner = owner;
			Status = status;
			Created = time;
		}
		
		public override string Name {
			get { return Status;}
		}
		
		public override string Description {
			get { return string.Format ("Posted at {0}", Created); }
		}
		
		public override string Icon {
			get { return "microblogging.svg@" + GetType ().Assembly.FullName; }
		}
		
		public long Id { get; private set; }
		public string Owner { get; private set; }
		public string Status { get; private set; }
		public DateTime Created { get; private set; }
	}
	
	public class MicroblogStatusReply
	{
		public MicroblogStatusReply (Nullable<long> inReplyToID, string status)
		{
			Status = status;
			InReplyToId = inReplyToID;			
		}
	
		public Nullable<long> InReplyToId { get; private set; }
		public string Status { get; private set; }
	}
}

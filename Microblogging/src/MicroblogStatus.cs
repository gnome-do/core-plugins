
using System;

using Do.Universe;

namespace Microblogging
{
	
	public class MicroblogStatus : Item
	{		
		DateTime time;
		
		public MicroblogStatus (int id, string status, string owner, DateTime time)
		{
			Id = id;
			Owner = owner;
			Status = status;
			this.time = time;
		}
		
		public override string Name {
			get { return Status;}
		}
		
		public override string Description {
			get { return string.Format ("Posted at {0}", time); }
		}
		
		public override string Icon {
			get { return "microblogging.svg@" + GetType ().Assembly.FullName; }
		}
		
		public virtual int Id { get; private set; }
		public virtual string Owner { get; private set; }
		public virtual string Status { get; private set; }
	}
	
	public class MicroblogStatusReply
	{
		public MicroblogStatusReply (Nullable<int> inReplyToID, string status)
		{
			Status = status;
			InReplyToId = inReplyToID;			
		}
	
		public Nullable<int> InReplyToId { get; private set; }
		public string Status { get; private set; }
	}
}

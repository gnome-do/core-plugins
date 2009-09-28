
using System;
using System.Linq;

using Mono.Addins;

using Do.Universe;

namespace Skype
{
	
	
	public class SkypeContactDetailItem : Item
	{
		StatusItem status;
		
		public SkypeContactDetailItem (ContactItem owner, string handle)
		{
			this.Handle = handle;
			this.status = Skype.ContactStatus (this.Handle);
			//if offline was returned, query to see if the user has a phone number set, maybe the user's status is really "SKYPEOUT"
			if (this.status == Skype.Statuses [OnlineStatus.Offline]) {
				foreach (string detail in owner.Details.Where (d => d.Contains ("phone"))) {
					Console.WriteLine ("Checking {0} :: {1}", detail, owner [detail]);
					if (Skype.ContactStatus (Skype.StripPhoneChars (owner [detail])) == Skype.Statuses [OnlineStatus.SkypeOut])
						this.status = Skype.Statuses [OnlineStatus.SkypeOut];
				}
			}
		}

		public override string Name {
			get {
				string fullName = Skype.ContactFullName (this.Handle);
				if (string.IsNullOrEmpty (fullName))
				    return string.Format ("{0} (Skype)", this.Handle);
				return string.Format ("{0} (Skype)", fullName);
			}
		}

		public override string Description {
			get {
				string mood = Skype.ContactMood (this.Handle);
				if (string.IsNullOrEmpty (mood))
				    return status.Description;
				return string.Format ("\"{0}\" ({1})", Skype.ContactMood (this.Handle), status.Description);
			}
		}

		public override string Icon {
			get { return status.Icon; }
		}
		
		public string Handle { get; private set; }
	}
}

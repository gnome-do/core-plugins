using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using Do.Universe;
using Do.Platform;

namespace Emesene
{
	public class EmeseneContactItemSource : ItemSource
	{
		private List<Item> contacts;
		private Dictionary<string, int> contactsRelation;
		private int lastContact;
		
		public EmeseneContactItemSource ()
		{		
			this.contacts = new List<Item> ();
			this.contactsRelation = new Dictionary<string, int>();
			this.lastContact = -1;
		}
		
		public override string Name { get { return "Emesene Buddies"; } }
		public override string Description { get { return "Buddies on your emesene buddy list."; }}
		public override string Icon {get { return "emesene"; } }
		
		public override IEnumerable<Type> SupportedItemTypes 
		{
			get {
				return new Type[] {
					typeof (ContactItem)
				};
			}
		}
		
		public override IEnumerable<Item> Items 
		{
			get { return this.contacts; }
		}
			
		public void getDisplayFromDB(ContactItem contact)
		{
			string photo;
			Log.Debug ("Emesene > Tryng to get display from DB for: {0}...", contact["email"]);
			photo = Emesene.get_last_display_picture(contact["email"], false);
			if (photo != "noImage") 
			{
                Log.Debug ("Emesene > Display found! in: {0}", photo);
				contact["photo"] = photo;
			}
			else
			{   
			    Log.Debug ("Emesene > No display picture in DB for: {0}", contact["email"]);
				if(this.lastContact == -1)
				{
					this.lastContact = this.contactsRelation[contact["email"]];
				}
				do
				{
					ContactItem newContact = (ContactItem)this.contacts[lastContact];
					string p = newContact["photo"];
					if(p != "")
					{
						this.lastContact--;
						continue;
					}
					Log.Debug ("Emesene > Now tryng to get display from DB for: {0}", newContact["email"]);
					photo = Emesene.get_last_display_picture(newContact["email"], false);
					if (photo != "noImage")
					{
                        Log.Debug ("Emesene > Display found! in: {0}", photo);
						newContact["photo"] = photo;
						Log.Debug ("Emesene > Display picture of user: {0} : {1}", newContact["email"], newContact["photo"]);
					}
					else
					{
					    Log.Debug ("Emesene > No display picture in DB for user: {0}", newContact["email"]);
					}
					this.lastContact--;
					Log.Debug ("Emesene > lastContact: {0}", this.lastContact);
				}while(photo == "noImage" && this.lastContact > 0);
			}
		}
		
		public override void UpdateItems ()
		{   
            Log.Debug ("Emesene > Updating contacts");
            Log.Debug ("Emesene > Checking for emesene...");
			if (Emesene.checkForEmesene())
			{
                Log.Debug ("Emesene > emesene Dbus is ON");
			} 
			else 
			{
				Log.Debug ("Emesene > emesene Dbus is OFF");
			}			
			string contactsFile;			
			if (Emesene.checkForEmesene())
			{
				if (contacts.Count < 1)
				{
					contactsFile = Emesene.getCurrentEmeseneUser();
					contactsFile = contactsFile.Replace(".","_");
					contactsFile = Environment.GetFolderPath
							(Environment.SpecialFolder.Personal)+
							"/.config/emesene1.0/"+contactsFile.Replace("@","_")+
								"/cache/"+Emesene.getCurrentEmeseneUser()+"_di.xml";
					Log.Debug ("Emesene > ------------------EmeseneContactItemSource------------------");
					Log.Debug ("Emesene > XML file with contacts: {0}", contactsFile);					
					XmlDocument blist;
					Dictionary<ContactItem, bool> buddies_seen;
					buddies_seen = new Dictionary<ContactItem, bool> ();
					blist = new XmlDocument();				
					string mail;				
					try 
					{
						blist.Load (contactsFile);
						int i = 0 ;
						int withDP = 0;
						string photo;
						foreach (XmlNode buddy_node in blist.GetElementsByTagName ("passportName"))
						{
							i++;
							mail = buddy_node.InnerText;
							Log.Debug ("Emesene > ===============================================");
							Log.Debug ("Emesene > Node#: {0} - PassportName: {1}", i,mail);
							ContactItem buddy;
							buddy = ContactItem.CreateWithEmail(mail);
							photo = Emesene.get_last_display_picture(mail, true);
							if (photo != "noImage")
							{
								buddy["photo"] = photo;
								Log.Debug ("Emesene > User: {0} - Display: {1}", buddy["email"], photo);
								withDP++;
							}
							else
							{
							    Log.Debug ("Emesene > User: {0} - Display: None", buddy["email"]);
							}
							if (buddy == null) continue;
							buddies_seen[buddy] = true;
						}
						Log.Debug ("Emesene > Total # of node contacts: {0}", --i);
						Log.Debug ("Emesene > Total # of buddies seen: {0}", buddies_seen.Keys.Count);
						Log.Debug ("Emesene > # of buddies with display: {0}", withDP);
					} 
					catch (Exception e) 
					{
					    Log.Error ("Emesene > Error reading contact list file: {0}", e.Message);
				        Log.Debug (e.StackTrace);
					}
					foreach (ContactItem buddy in buddies_seen.Keys) 
					{
						contacts.Add (buddy);
						contactsRelation[buddy["email"]]= contacts.Count - 1;
					}
				}
				//contacts > 0
				else
				{
				    Log.Debug ("Emesene > Length of the contact list was larger than 0...");
					string photo;
					ContactItem newContact = null;
					foreach (ContactItem contact in contacts)
					{
						if(contact["photo"] == null)
						{
						    Log.Debug ("Emesene > Getting display picture from emesene cache for: {0}...", contact["email"]);
							photo = Emesene.get_last_display_picture(contact["email"], true);
							if (photo != "noImage")
							{
								contact["photo"] = photo;
								Log.Debug ("Emesene > Display picture found! : {0}", contact["photo"]);
							}
							else
							{
								newContact = contact;
							}
						}
					}
					if(newContact != null)
					{
						this.getDisplayFromDB(newContact);
					}
				}
				Log.Debug ("Emesene > End, waiting to update again...");
			}
		}
	}
}

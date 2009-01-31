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
			Log<EmeseneContactItemSource>.Debug ("Tryng to get display from DB for: {0}...", contact["email"]);
			photo = Emesene.get_last_display_picture(contact["email"], false);
			if (photo != "noImage") 
			{
                Log<EmeseneContactItemSource>.Debug ("Display found! in: {0}", photo);
				contact["photo"] = photo;
			}
			else
			{   
			    Log<EmeseneContactItemSource>.Debug ("No display picture in DB for: {0}", contact["email"]);
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
					Log<EmeseneContactItemSource>.Debug ("Now tryng to get display from DB for: {0}", newContact["email"]);
					photo = Emesene.get_last_display_picture(newContact["email"], false);
					if (photo != "noImage")
					{
                        Log<EmeseneContactItemSource>.Debug ("Display found! in: {0}", photo);
						newContact["photo"] = photo;
						Log<EmeseneContactItemSource>.Debug ("Display picture of user: {0} : {1}", newContact["email"], newContact["photo"]);
					}
					else
					{
					    Log<EmeseneContactItemSource>.Debug ("No display picture in DB for user: {0}", newContact["email"]);
					}
					this.lastContact--;
					Log<EmeseneContactItemSource>.Debug ("lastContact: {0}", this.lastContact);
				}while(photo == "noImage" && this.lastContact > 0);
			}
		}
		
		public override void UpdateItems ()
		{   
            Log<EmeseneContactItemSource>.Debug ("Updating contacts");
            Log<EmeseneContactItemSource>.Debug ("Checking for emesene...");
			if (Emesene.checkForEmesene())
			{
                Log<EmeseneContactItemSource>.Debug ("emesene Dbus is ON");
			} 
			else 
			{
				Log<EmeseneContactItemSource>.Debug ("emesene Dbus is OFF");
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
					//Log<EmeseneContactItemSource>.Debug (" ------------------EmeseneContactItemSource------------------");
					Log<EmeseneContactItemSource>.Debug ("XML file with contacts: {0}", contactsFile);					
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
							//Log<EmeseneContactItemSource>.Debug ("===============================================");
							//Log<EmeseneContactItemSource>.Debug ("Emesene > Node#: {0} - PassportName: {1}", i,mail);
							ContactItem buddy;
							buddy = ContactItem.CreateWithEmail(mail);
							photo = Emesene.get_last_display_picture(mail, true);
							if (photo != "noImage")
							{
								buddy["photo"] = photo;
								//Log<EmeseneContactItemSource>.Debug ("User: {0} - Display: {1}", buddy["email"], photo);
								withDP++;
							}
							else
							{
							    //Log<EmeseneContactItemSource>.Debug ("User: {0} - Display: None", buddy["email"]);
							}
							if (buddy == null) continue;
							buddies_seen[buddy] = true;
						}
						Log<EmeseneContactItemSource>.Debug ("Total # of node contacts: {0}", --i);
						//Log<EmeseneContactItemSource>.Debug ("Total # of buddies seen: {0}", buddies_seen.Keys.Count);
						//Log<EmeseneContactItemSource>.Debug ("# of buddies with display: {0}", withDP);
					} 
					catch (Exception e) 
					{
					    Log<EmeseneContactItemSource>.Error ("Error reading contact list file: {0}", e.Message);
				        Log<EmeseneContactItemSource>.Debug (e.StackTrace);
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
				    Log<EmeseneContactItemSource>.Debug ("Length of the contact list was larger than 0...");
					string photo;
					ContactItem newContact = null;
					foreach (ContactItem contact in contacts)
					{
						if(contact["photo"] == null)
						{
						    //Log<EmeseneContactItemSource>.Debug ("Getting display picture from emesene cache for: {0}...", contact["email"]);
							photo = Emesene.get_last_display_picture(contact["email"], true);
							if (photo != "noImage")
							{
								contact["photo"] = photo;
								//Log<EmeseneContactItemSource>.Debug ("Display picture found! : {0}", contact["photo"]);
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
				Log<EmeseneContactItemSource>.Debug ("End, waiting to update again...");
			}
		}
	}
}

// EmeseneContactItemSource.cs created with MonoDevelop
// User: luis at 04:22 p 04/07/2008

using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

using Do.Universe;

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
			System.Console.WriteLine("Tryng to get display from DB for: " +
				                         contact["email"] );
			photo = Emesene.get_last_display_picture(contact["email"], false);
			if (photo != "noImage") 
			{
				System.Console.WriteLine ("Display found! in: "+photo);
				contact["photo"] = photo;
			}
			else
			{
				System.Console.WriteLine ("No display picture in DB for: "+
				                          contact["email"]);
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
					System.Console.WriteLine ("Now tryng to get display from DB for: " +
					                          newContact["email"] );
					photo = Emesene.get_last_display_picture(newContact["email"], false);
					if (photo != "noImage")
					{
						System.Console.WriteLine ("Display found! in: "+photo);
						newContact["photo"] = photo;
						System.Console.WriteLine("Photo of user: "+newContact["email"]+" : "+
						                         newContact["photo"]);
					}
					else
					{
						System.Console.WriteLine ("No display picture in DB for: "+
						                          newContact["email"]);
					}
					this.lastContact--;
					System.Console.WriteLine ("lastContact: "+this.lastContact);
				}while(photo == "noImage" && this.lastContact > 0);
			}
		}
		
		public override void UpdateItems ()
		{   
			System.Console.WriteLine("---Updating contacts---");
			System.Console.WriteLine("---Checking for emesene---");
			if (Emesene.checkForEmesene())
			{
				System.Console.WriteLine("---emesene Dbus is ON---");
			} 
			else 
			{
				System.Console.WriteLine("---emesene Dbus is OFF---");
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
					System.Console.WriteLine
							("------------------EmeseneContactItemSource------------------");	
					System.Console.WriteLine("XML file with contacts: " + contactsFile);
					
					XmlDocument blist;
					// Add buddies as they are encountered to this hash
					// so we don't create duplicates.
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
							System.Console.WriteLine
									("===============================================");
							System.Console.WriteLine("Nodo #: "+i+" - PassportName: "+mail);
							ContactItem buddy;
							buddy = ContactItem.CreateWithEmail(mail);
							photo = Emesene.get_last_display_picture(mail, true);
							if (photo != "noImage")
							{
								buddy["photo"] = photo;
								System.Console.WriteLine("USER: "+buddy["email"]+
									                         " - DISPLAY: "+photo);
								withDP++;
							}
							else
							{
								System.Console.WriteLine ("USER: " + buddy["email"]+
									                          " - DISPLAY: None");
							}
							if (buddy == null) continue;
							buddies_seen[buddy] = true;
						}
						System.Console.WriteLine ("Total # of node contacts: " + (i-1));
						System.Console.WriteLine ("Total # of Buddies seen: " + 
							                          buddies_seen.Keys.Count);
						System.Console.WriteLine ("Buddies with display: " + withDP);
					} 
					catch (Exception e) 
					{
						System.Console.WriteLine 
								("Could not read emesene buddy list file: " + e.Message);
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
					System.Console.WriteLine ("Length of the contact list was larger than 0");
					string photo;
					ContactItem newContact = null;
					foreach (ContactItem contact in contacts)
					{
						if(contact["photo"] == null)
						{
							System.Console.Write 
									("Getting display from emesene cache for: " +
									 contact["email"]);
							photo = Emesene.get_last_display_picture(contact["email"], true);
							if (photo != "noImage")
							{
								contact["photo"] = photo;
								System.Console.Write(" --FOUND!\r\n");
							}
							else
							{
								newContact = contact;
								System.Console.Write(" --NOT FOUND!\r\n");
							}
						}
					}
					if(newContact != null)
					{
						this.getDisplayFromDB(newContact);
					}
				}
				System.Console.WriteLine("End, waiting to update again");
			}
		}
	}
}
// GContactsItemSource.cs created with MonoDevelop
// User: alex at 9:33 AM 4/5/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Xml;
using System.Net;
using System.Web;
using System.IO;
using System.Collections.Generic;

using GConf;

using Do.Addins;
using Do.Universe;

using Google.GData.Client;

namespace Do.GContacts
{
	public class GContactsItemSource : IItemSource
	{
		private List<IItem> items;
		
		public GContactsItemSource()
		{
			items = new List<IItem> ();
			UpdateItems ();
		}
		
		public string Name { get { return "GMail Contacts"; } }
		public string Description { get { return "Indexes your GMail Contacts"; } }
		public string Icon { get { return "email"; } }
		
		public Type[] SupportedItemTypes {
			get {
				return new Type[] {
					typeof (ContactItem),
					typeof (BookmarkItem),
				};
			}
		}
		
		public ICollection<IItem> Items {
			get {
				return items;
			}
		}
		
		public ICollection<IItem> ChildrenOfItem (IItem item) {
			return null;
		}
		
		
		public void UpdateItems () {
			ContactItem buddy;
			string password, username, auth_token;
			GConf.Client gconf = new GConf.Client ();
			username = password = "";
			try {
				username = gconf.Get ("/apps/gnome-do/plugins/gcontacts/username") as string;
				password = gconf.Get ("/apps/gnome-do/plugins/gcontacts/password") as string;
			} catch (GConf.NoSuchKeyException) {
				gconf.Set ("/apps/gnome-do/plugins/gcontacts/username","");
				gconf.Set ("/apps/gnome-do/plugins/gcontacts/password","");
			}
			auth_token = Authorize (username, password);
			username = HttpUtility.UrlEncode (username);
			string url = string.Format ("http://www.google.com/m8/feeds/contacts/{0}/base?max-results=5000",
			                            username);
			if (auth_token !=  null) {
				items.Clear ();
				HttpWebRequest req = (HttpWebRequest) WebRequest.Create (url);
				WebHeaderCollection auth_header = new WebHeaderCollection ();
				auth_header.Add ("Authorization: GoogleLogin auth="+auth_token);
				req.Headers = auth_header;
				req.Method = "GET";
				string name, email, phone, address, key;
				int i;
				name = email = phone = address = key = "";
				XmlDocument contacts = new XmlDocument ();
				Dictionary<string, string> emails = new Dictionary<string,string> ();
				contacts.Load (req.GetResponse ().GetResponseStream ());
				foreach (XmlNode contact in contacts.GetElementsByTagName ("entry")) {
					i = 0;
					foreach (XmlNode cont in contact.ChildNodes) {
						switch (cont.Name) {
						case ("title"): name = cont.InnerText; break;
						case ("gd:email"):
							i++;
							email = cont.Attributes.GetNamedItem ("address").Value;
							key = "email." + i;
							try {
								if (cont.Attributes.GetNamedItem ("primary").Value == "true" )
									key = "email";
							} catch {
							}
							if(!emails.ContainsKey (key))
								emails.Add (key, email);
							break;
						case ("gd:phoneNumber") : phone = cont.InnerText; break;
						case ("gd:postalAddress"): address = cont.InnerText; break;
						}
					}
					if (!name.Equals ("") && !email.Equals ("")) {
						buddy = ContactItem.Create (name);
						foreach (KeyValuePair<string, string> kvp in emails) 
							buddy[kvp.Key] = kvp.Value;
						buddy["phone"] = phone;
						buddy["address"] = address;
						//For some reason this is not working automatically, so
						//let's force it to do what we want.
						buddy["description"] = buddy["email"];
						items.Add (buddy);
						emails.Clear ();
					}
				}
			}
		}
		
		private string Authorize (string username, string password) {
			string postdata,auth_token;
			string appid = "alexLauni-doGmailContacts-1";
			items.Clear ();
			
			//Begin HTTP POST request to google
			//Console.Error.WriteLine("THIS IS THE NEW VERSION");
			try {
				HttpWebRequest req = (HttpWebRequest) WebRequest.Create
					("https://www.google.com/accounts/ClientLogin");
				req.Method = "POST";
				req.ContentType = "application/x-www-form-urlencoded";
				postdata=string.Format ("accountType=GOOGLE_OR_HOSTED&Email={0}" +
				         "&Passwd={1}&service=cp&source={2}",username,password,appid);
				req.ContentLength = postdata.Length;
				StreamWriter sw = new StreamWriter (req.GetRequestStream (),
				                                    System.Text.Encoding.ASCII);
				sw.Write (postdata);
				sw.Close ();
				
				StreamReader sr = new StreamReader (req.GetResponse ().GetResponseStream ());
				sr.ReadLine ();
				sr.ReadLine ();
				auth_token = sr.ReadLine ();
				auth_token = auth_token.Substring (5, auth_token.Length - 5);
			} catch (WebException we) {
				items.Clear ();
				Console.Error.WriteLine (we.Response);
				Console.Error.WriteLine (we.Message);
				items.Add (new BookmarkItem ("GMail Contacts Authentication!"
				                             ,"https://www.google.com/accounts/DisplayUnlockCaptcha"));
				auth_token = null;
			}
			return auth_token;
		}
	}
}

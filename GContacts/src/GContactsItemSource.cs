// GContactsItemSource.cs created with MonoDevelop
// User: alex at 9:33 AM 4/5/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.IO;
using System.Net;
using System.Threading;
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
			GConf.Client gconf = new GConf.Client ();
			items = new List<IItem> ();
			try {
				GContact.Username = gconf.Get ("/apps/gnome-do/plugins/gcontacts/username") as string;
				GContact.Password = gconf.Get ("/apps/gnome-do/plugins/gcontacts/password") as string;
			} catch (GConf.NoSuchKeyException) {
				gconf.Set ("/apps/gnome-do/plugins/gcontacts/username","");
				gconf.Set ("/apps/gnome-do/plugins/gcontacts/password","");
				Console.Error.WriteLine ("Please set GConf keys");
			}
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
			if (GContact.Auth_Token == null) {
				GContact.Auth_Token = Authorize (GContact.Username,GContact.Password);
				if (GContact.Auth_Token == null)
					return;
			}
			Thread updateRunner = new Thread (new ThreadStart (GContact.UpdateContacts));
			updateRunner.Start ();
			items = GContact.Contacts;
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
				items.Add (new BookmarkItem ("GMail Contacts Authentication"
				                             ,"https://www.google.com/accounts/DisplayUnlockCaptcha"));
				auth_token = null;
			}
			return auth_token;
		}
	}
}

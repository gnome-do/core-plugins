/* GDocs.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */


using System;
using System.Net;
using System.Xml;
using System.Threading;
using System.Collections.Generic;
using Mono.Unix;

using Google.GData.Client;
using Google.GData.Documents;
using Google.GData.Extensions;

using Do.Universe;
using Do.Platform;

namespace GDocs {
	public static class GDocs {
		private static DocumentsService service;
		private static List<Item> docs;
		private static object docs_lock;
		
		const string FeedUri = "http://docs.google.com/feeds/documents/private/full";
		private static string gAppName = "pengDeng-gnomeDoGDocsPlugin-1.0";
		
		static GDocs ()
		{
			string username, password;
			//Google works over SSL, we need accept the cert.
			System.Net.ServicePointManager.CertificatePolicy = new CertHandler ();
			
			docs = new List<Item> ();
			docs_lock = new object ();
			
			Configuration.GetAccountData (out username, out password,
			                              typeof (Configuration));
			Connect (username, password);
		}
		
		public static string GAppName {
			get { return gAppName; }
		}
		
		public static bool Connect (string username, string password)
		{
			try {
				service = new DocumentsService (gAppName);
				service.setUserCredentials (username, password);
			} catch (Exception e) {
				Console.Error.WriteLine (e.Message);
				return false;
			}
			return true;
		}
		
		public static List<Item> Docs {
			get { return docs; }
		}
		
		public static void UpdateDocs ()
		{
			DocumentsFeed docsFeed;
			DocumentsListQuery query = new DocumentsListQuery ();
			query.Uri = new Uri (FeedUri);
			
			try {
				docsFeed = service.Query (query);
			} catch (Exception e) {
				docsFeed = null;
				Console.Error.WriteLine (e.Message);
				return;
			}
			
			lock (docs_lock) {				
				docs.Clear ();				
				foreach (DocumentEntry doc in docsFeed.Entries) {					
					string doc_url = doc.AlternateUri.Content;
					string doc_title = doc.Title.Text;
					
					if (doc.IsDocument) 
						docs.Add (new GDocsDocumentItem (doc_title, doc_url));
					else if (doc.IsSpreadsheet) 
						docs.Add (new GDocsSpreadsheetItem (doc_title, doc_url));
					else if (doc.IsPresentation) 
						docs.Add (new GDocsPresentationItem (doc_title, doc_url));
					else if (doc.IsPDF)
						docs.Add (new GDocsPDFItem (doc_title, doc_url));					
				}
			}
		}
		
		public static Item UploadDocument (string fileName, string documentName)
		{
			DocumentEntry newDoc;
			GDocsItem newDocItem;
			
			try {
				newDoc = service.UploadDocument (fileName, documentName);
			} catch (Exception e) {
				newDoc = null;
				Console.Error.WriteLine (e.Message);
				Do.Platform.Services.Notifications.Notify (new Do.Platform.Notification 
				                                           (Catalog.GetString ("Uploading failed."), 
				                                            Catalog.GetString ("An error occurred when uploading file to Google Docs."), 
				                                            "gDocsIcon.png@" + typeof (GDocsItemSource).Assembly.FullName));
				return null; 
			}		
			
			string doc_url = newDoc.AlternateUri.Content;
			string doc_title = newDoc.Title.Text;
		
			if (newDoc.IsDocument)
				newDocItem = new GDocsDocumentItem (doc_title, doc_url);
			else if (newDoc.IsSpreadsheet)
				newDocItem = new GDocsSpreadsheetItem (doc_title, doc_url);
			else if (newDoc.IsPresentation)
				newDocItem = new GDocsPresentationItem (doc_title, doc_url);
			else if (newDoc.IsPDF)
				newDocItem = new GDocsPDFItem (doc_title, doc_url);
			else
				return null;
			
			lock (docs_lock) {
				docs.Add (newDocItem);
			}
			return newDocItem;
		}
		
		public static void TrashDocument (GDocsItem docItem)
		{
			string doc_title = docItem.Name;
			string doc_url   = docItem.URL;
			
			// Search for document(s) having exactly the title,
			// Delete the one with matching AlternateUri
			DocumentsListQuery query = new DocumentsListQuery ();
			query.Title = doc_title;
			query.TitleExact = true;
			DocumentsFeed docFeed = service.Query (query);	

			if (docFeed.Entries.Count >= 1) {
				foreach (DocumentEntry docEntry in docFeed.Entries) {	
					if (docEntry.AlternateUri == doc_url) {
						try {
							docEntry.Delete ();
						} catch (Exception e) {
							Console.Error.WriteLine (e.Message);
							Do.Platform.Services.Notifications.Notify (new Do.Platform.Notification 
							                                           (Catalog.GetString ("Deleting failed."), 
							                                            Catalog.GetString ("An error occurred when deleting the document at Google Docs."), 
							                                            "gDocsIcon.png@" + typeof (GDocsItemSource).Assembly.FullName));
							return;
						}
						
						Do.Platform.Services.Notifications.Notify (new Do.Platform.Notification 
						                                           (Catalog.GetString ("Document deleted."), 
						                                            Catalog.GetString (String.Format ("The document '{0}' has been successfully moved into Trash at Google Docs.",
						                                                                              doc_title)), 
						                                            "gDocsIcon.png@" + typeof (GDocsItemSource).Assembly.FullName));						
						lock (docs_lock) {						
							docs.Remove (docItem);
						}
					}
				}
			}
		}
	}
}
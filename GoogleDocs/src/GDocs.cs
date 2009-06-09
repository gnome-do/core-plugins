// GDocs.cs
// 
// Copyright (C) 2009 GNOME Do
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Net;
using System.Xml;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using Mono.Unix;

using Google.GData.Client;
using Google.GData.Documents;
using Google.GData.Extensions;

using Do.Universe;
using Do.Platform;

namespace GDocs
{
	
	public class GDocs
	{
		
		const string FeedUri = "http://docs.google.com/feeds/documents/private/full";
		public const string GAppName = "pengDeng-gnomeDoGDocsPlugin-1.0";
		
		static List<Item> docs;
		static object docs_lock;
		static GDocsPreferences prefs;
		static DocumentsService service;
		
		static GDocs ()
		{
			docs = new List<Item> ();
			docs_lock = new object ();
			prefs = new GDocsPreferences ();
			
			// Google works over SSL, we need accept the cert.
			System.Net.ServicePointManager.CertificatePolicy = new CertHandler ();
			Connect (Preferences.Username, Preferences.Password);
		}
		
		public static bool Connect (string username, string password)
		{
			try {
				service = new DocumentsService (GAppName);
				service.setUserCredentials (username, password);
			} catch (Exception e) {
				Log<GDocs>.Error (e.Message);
				return false;
			}
			return true;
		}
		
		internal static GDocsPreferences Preferences {
			get { return prefs; }
		}
		
		public static List<Item> Docs {
			get { 
				lock (docs_lock)
					return docs; 
			}
		}
		
		public static void UpdateDocs ()
		{
			lock (docs_lock) {
				DocumentsFeed docsFeed;
				DocumentsListQuery query = new DocumentsListQuery ();
				query.Uri = new Uri (FeedUri);
				
				try {
					docsFeed = service.Query (query);
				} catch (Exception e) {
					docsFeed = null;
					Log<GDocs>.Error (e.Message);
					return;
				}
				
				docs.Clear ();
				foreach (DocumentEntry doc in docsFeed.Entries) {
					GDocsAbstractItem item = MaybeItemFromEntry (doc);
					if (item != null)
						docs.Add (item);
				}
			}
		}
		
		static GDocsAbstractItem MaybeItemFromEntry (DocumentEntry doc)
		{
				string url = doc.AlternateUri.Content;
				string title = doc.Title.Text;
			
				if (doc.IsDocument) 
					return new GDocsDocumentItem (title, url);
				else if (doc.IsSpreadsheet) 
					return new GDocsSpreadsheetItem (title, url);
				else if (doc.IsPresentation) 
					return new GDocsPresentationItem (title, url);
				else if (doc.IsPDF)
					return new GDocsPDFItem (title, url);
				else
					return null;
		}
		
		public static Item UploadDocument (string fileName, string documentName)
		{
			DocumentEntry newDoc;
			
			try {
				newDoc = service.UploadDocument (fileName, documentName);
			} catch (Exception e) {
				newDoc = null;
				Log<GDocs>.Error (e.Message);
				Services.Notifications.Notify (GetUploadFailedNotification ());
				return null; 
			}
			
			return  MaybeItemFromEntry (newDoc);
		}
		
		static Notification GetUploadFailedNotification ()
		{
			return new Notification (
				Catalog.GetString ("Uploading failed."), 
				Catalog.GetString ("An error occurred when uploading files to Google Docs."), 
				"gDocsIcon.png@" + typeof (GDocsItemSource).Assembly.FullName);
		}
		
		static Notification GetDeleteDocumentFailedNotification ()
		{
			return new Notification (
				Catalog.GetString ("Deleting failed."), 
				Catalog.GetString ("An error occurred when deleting the document at Google Docs."), 
				"gDocsIcon.png@" + typeof (GDocsItemSource).Assembly.FullName);
		}
		
		static Notification GetDocumentDeletedNotification (string documentTitle)
		{
			return new Notification (
				Catalog.GetString ("Document deleted."), 
				string.Format (Catalog.GetString ("The document '{0}' has been successfully moved into Trash at Google Docs."), documentTitle), 
				"gDocsIcon.png@" + typeof (GDocsItemSource).Assembly.FullName);
		}
		
		public static void TrashDocument (GDocsAbstractItem item)
		{
			// Search for document(s) having exactly the title,
			// Delete the one with matching AlternateUri
			DocumentsListQuery query = new DocumentsListQuery ();
			query.Title = item.Name;
			query.TitleExact = true;
			DocumentsFeed docFeed = service.Query (query);	
			DocumentEntry document =
				docFeed.Entries.FirstOrDefault (e => e.AlternateUri == item.URL) as DocumentEntry;
			
			if (document == null) return;
			
			try {
				document.Delete ();
			} catch (Exception e) {
				Log.Error (e.Message);
				Services.Notifications.Notify (GetDeleteDocumentFailedNotification ());
				return;
			}
			
			Services.Notifications.Notify (GetDocumentDeletedNotification (item.Name));
		}
	}
}

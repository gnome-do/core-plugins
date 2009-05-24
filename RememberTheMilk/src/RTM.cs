// RTM.cs
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
// 

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Unix;
using RtmNet;

using Do.Platform;
using Do.Universe;

namespace RememberTheMilk
{
	public static class RTM
	{
		#region [ Private Variable, Constant ]
		
		static Rtm rtm;
		static List<Item> tasks;
		static List<Item> lists;
		static List<Item> tags;
		static List<Item> locations;
		static List<Item> priorities;
		static List<Item> notes;
		static object list_lock;
		static object task_lock;
		static object location_lock;
		static object note_lock;
		static string timeline;
		static DateTime last_sync;
		static string username;
		static string filter;
		static  string RTMIconPath = "rtm.png@" + typeof (RTMListItemSource).Assembly.FullName;
		
		const string ApiKey = "ee32c06f2d45baf935a2c046323457d8";
		const string SharedSecret = "1b835b123a903938";

		#endregion [ Private Properties, Constant ]
		
		static RTM ()
		{
			rtm = new Rtm (ApiKey, SharedSecret);
			tasks = new List<Item> ();
			lists = new List<Item> ();
			tags = new List<Item> ();
			locations = new List<Item> ();
			priorities = new List<Item> ();
			notes = new List<Item> ();
			list_lock = new object ();
			task_lock = new object ();
			location_lock = new object ();
			note_lock = new object ();
			last_sync = DateTime.MinValue;
			Preferences = new RTMPreferences ();
			filter = Preferences.Filter;
			
			UpdatePriorities ();
			TryConnect ();
		}
		
		#region [ Authentication ]
		
		public static bool IsAuthenticated {
			get { return  (rtm.IsAuthenticated && !String.IsNullOrEmpty (rtm.AuthToken)); }
		}
		
		public static string AuthInit ()
		{
			string frob;
			try {
				frob = rtm.AuthGetFrob ();
			} catch (RtmException e) {
				Log.Error (Catalog.GetString ("Failed to initialize authentication."), e.Message);
				return "";
			}
			Do.Platform.Services.Environment.OpenUrl(rtm.AuthCalcUrl (frob, AuthLevel.Delete));
			return frob;
		}
		
		public static Auth AuthComplete (string frob)
		{
			Auth auth;
			try {
				auth = rtm.AuthGetToken (frob);
			} catch (RtmException e) {
				Log.Error (Catalog.GetString("Failed to complete authentication."), e.Message);
				return null;
			}
			rtm.AuthToken = auth.Token;
			timeline = rtm.TimelineCreate ();
			return auth;
		}
		
		#endregion [ Authentication ]
		
		#region [ Public Properties ]
		
		public static RTMPreferences Preferences { get; set; }
		
		public static List<Item> Lists {
			get {
				if (lists.FindIndex (i => (i as RTMListItem).Id == "All Tasks") == -1)
					lists.Add (new RTMListItem ("All Tasks", "All Tasks", 1, 0));
				return lists;
			}
		}
		
		public static List<Item> Tasks {
			get {
				return tasks;
			}
		}
		
		public static List<Item> Locations {
			get {
				return locations;
			}
		}
		
		public static List<Item> Tags {
			get { return tags; }
		}
		
		public static List<Item> Priorities
		{
			get {
				return priorities;
			}
		}
		
		#endregion [ Public Properties ]
		
		#region [ Relational Search ]
		
		public static List<Item> TasksForList (string listId)
		{
			if (listId == "All Tasks")
				return tasks;
			else
				return tasks.FindAll (i => (i as RTMTaskItem).ListId == listId);
		}
		
		public static List<Item> TasksForTag (string tag)
		{
			return tasks.FindAll (i => (i as RTMTaskItem).Tags.Contains (tag));
		}
		
		public static List<Item> TasksForLocation (string locationId)
		{
			return tasks.FindAll (i => (i as RTMTaskItem).LocationId == locationId);
		}
		
		public static List<Item> AttributesForTask (RTMTaskItem task)
		{
			List<Item> attribute_list = new List<Item> ();
			
			if (task.Due != DateTime.MinValue)
				attribute_list.Add (new RTMTaskAttributeItem (task.Due.ToString ((task.HasDueTime != 0) ? "g" : "d"),
				                                              "Due Date/Time",
				                                              task.Url, "stock_calendar", task));
			if (!String.IsNullOrEmpty (task.TaskUrl))
				attribute_list.Add (new RTMTaskAttributeItem (task.TaskUrl, "URL",
				                                              task.TaskUrl, "text-html", task));
			if (!String.IsNullOrEmpty (task.Estimate))
				attribute_list.Add (new RTMTaskAttributeItem (task.Estimate, "Time Estimate",
				                                              task.Url, "stock_appointment-reminder", task));
			if (!String.IsNullOrEmpty (task.LocationId)) {
				attribute_list.Add (locations.Find (i => (i as RTMLocationItem).Id == task.LocationId));
			}
			
			if (!String.IsNullOrEmpty (task.Tags)) {
				attribute_list.Add (new RTMTaskAttributeItem (task.Tags, "Tags", task.Url,
				                                              "task-tag.png@" + typeof (RTMListItemSource).Assembly.FullName,
				                                              task));
			}
			
			List<Item> note_list = notes.FindAll (i => (i as RTMNoteItem).TaskId == task.Id);
			if (note_list.Any ())
				lock (note_lock)
					foreach (Item item in note_list)
						attribute_list.Add (item);
			
			return attribute_list;
		}
		
		#endregion [ Relational Search ]
		
		#region [ Methods for Data Update ]
		
		public static void UpdateLists ()
		{
			if (!IsAuthenticated)
				if (!TryConnect ())
					return;
			
			Lists rtmLists;
			try {
				rtmLists = rtm.ListsGetList ();
			} catch (RtmException e) {
				Log.Debug (Catalog.GetString ("An error happend when updating RTM lists."), e.Message);
				rtmLists = null;
				return;
			}

			lock (list_lock) {
				lists.Clear ();
				foreach (List rtmList in rtmLists.listCollection)
					if (rtmList.Deleted == 0 && rtmList.Smart == 0)
						lists.Add (new RTMListItem (rtmList.ID, rtmList.Name, rtmList.Locked, rtmList.Smart));
			}
		}
		
		public static void UpdateLocations ()
		{
			if (!IsAuthenticated)
				if (!TryConnect ())
					return;
			
			Locations rtmLocations;
			try {
				rtmLocations = rtm.LocationsGetList ();
			} catch (RtmException e) {
				Log.Debug (Catalog.GetString ("An error happend when updating RTM locations."), e.Message);
				rtmLocations = null;
				return;
			}

			lock (location_lock) {
				locations.Clear ();
				if (rtmLocations.locationCollection.Length > 0) {
					foreach (Location rtmLocation in rtmLocations.locationCollection) {
						locations.Add (new RTMLocationItem (rtmLocation.ID, 
						                                    rtmLocation.Name, 
						                                    rtmLocation.Address,
						                                    rtmLocation.Longitude, 
						                                    rtmLocation.Latitude));
					}
				}
			}
		}
		
		public static void UpdateTasks ()
		{
			if (!IsAuthenticated)
				if (!TryConnect ())
					return;
			
			Tasks rtmTasks;
			
			// if settings have changed, reset the synchronization state;
			if (filter != Preferences.Filter || username != Preferences.Username)
				last_sync = DateTime.MinValue;
			
			if (last_sync == DateTime.MinValue) {
				tasks.Clear ();
			}
			
			filter = Preferences.Filter;
			if (String.IsNullOrEmpty (filter))
				filter = "status:incomplete";
			else if (!filter.Contains ("status:"))
				filter = "status:incomplete OR (" + filter + ")";
			
			try {
				// If first time sync, get full list of incompleted tasks
				// otherwise, only do incremental sync.
				if (last_sync == DateTime.MinValue)
					rtmTasks = rtm.TasksGetList (null, null, filter);
				else
					rtmTasks = rtm.TasksGetList (null, last_sync.ToUniversalTime ().ToString ("u"), filter);
			} catch (RtmException e) {
				rtmTasks = null;
				last_sync = DateTime.MinValue;
				Log.Debug (Catalog.GetString ("An error happend when updating RTM tasks."), e.Message);
				return;
			}
			
			foreach (List rtmList in rtmTasks.ListCollection) {
				
				if (rtmList.DeletedTaskSeries != null)
					foreach (TaskSeries rtmTaskSeries in rtmList.DeletedTaskSeries.TaskSeriesCollection)
						foreach (Task rtmTask in rtmTaskSeries.TaskCollection)
							TryRemoveTask (rtmTask.TaskID);
				
				if (rtmList.TaskSeriesCollection != null) {
					lock (task_lock) {
						foreach (TaskSeries rtmTaskSeries in rtmList.TaskSeriesCollection) {
							foreach (Task rtmTask in rtmTaskSeries.TaskCollection) {
								// delete one recurrent task will cause other deleted instances
								// appear in the taskseries tag, so here we need to check again.
								if (rtmTask.Deleted == DateTime.MinValue) {
									// handle tags
									string temp_tags = "";
									if (rtmTaskSeries.Tags.TagCollection.Length > 0) {
										foreach (Tag rtmTag in rtmTaskSeries.Tags.TagCollection) {
											if (tags.FindIndex (i => (i as RTMTagItem).Name == rtmTag.Text) == -1)
												tags.Add (new RTMTagItem (rtmTag.Text));
											temp_tags += rtmTag.Text + ", ";
										}
										temp_tags = temp_tags.Remove (temp_tags.Length-2);
									}
									
									// handle notes
									if (rtmTaskSeries.Notes.NoteCollection.Length > 0) {
										foreach (Note rtmNote in rtmTaskSeries.Notes.NoteCollection)
											notes.Add (new RTMNoteItem (rtmNote.Title, rtmNote.Text, rtmNote.ID, 
											                            "http://www.rememberthemil.com/print/"
											                            + username + "/" 
											                            + rtmList.ID + "/" 
											                            + rtmTask.TaskID 
											                            + "/notes/",
											                            rtmTask.TaskID));
									}
									
									// add new task
									RTMTaskItem new_task = new RTMTaskItem (rtmList.ID,
									                                        rtmTaskSeries.TaskSeriesID,
									                                        rtmTask.TaskID,
									                                        rtmTaskSeries.Name,
									                                        rtmTask.Due,
									                                        rtmTask.Completed,
									                                        rtmTaskSeries.TaskURL,
									                                        rtmTask.Priority,
									                                        rtmTask.HasDueTime,
									                                        rtmTask.Estimate,
									                                        rtmTaskSeries.LocationID,
									                                        temp_tags);
									if (last_sync != DateTime.MinValue)
										TryRemoveTask (rtmTask.TaskID);
									tasks.Add (new_task);
								}
							}
						}
					}
				}
			}
			
			last_sync = DateTime.Now;
			
			if (Preferences.OverdueNotification)
				NotifyOverDueItems ();
        }
		
		static void UpdatePriorities ()
		{
			priorities.Add (new RTMPriorityItem (Catalog.GetString ("High"),
			                                     Catalog.GetString ("High Priority")));
			priorities.Add (new RTMPriorityItem (Catalog.GetString ("Medium"),
			                                     Catalog.GetString ("Medium Priority")));
			priorities.Add (new RTMPriorityItem (Catalog.GetString ("Low"),
			                                     Catalog.GetString ("Low Priority")));
			priorities.Add (new RTMPriorityItem (Catalog.GetString ("None"),
			                                     Catalog.GetString ("No Priority")));
			priorities.Add (new RTMPriorityItem (Catalog.GetString ("Up"),
			                                     Catalog.GetString ("Increase the priority")));
			priorities.Add (new RTMPriorityItem (Catalog.GetString ("Down"),
			                                     Catalog.GetString ("Decrease the priority")));
		}
		
		#endregion [ Methods for Data Update ]
		
		#region [ Task Actions ]
		
		public static RTMTaskItem NewTask (string listId, string taskData)
		{
			List rtmList;
			bool parse = true;
			string priority = "N";
			
			// Task string starting with "@" won't be parsed for date/time information
			if (taskData.StartsWith ("@")) {
				taskData = taskData.Remove (0, 1).Trim ();
				parse = false;
			}
			
			// Task string starting with "![123]" contains priority information
			if (Regex.IsMatch (taskData, @"^![123]\s")) {
				priority = taskData.Substring (1,1);
				taskData = taskData.Remove (0, 3);
			}
			
			try {
				rtmList = rtm.TasksAdd (timeline, taskData, listId, parse);
			} catch (RtmException e) {
				Log.Debug (e.Message);
				return null;
			}
			
			
			if (priority != "N") {
				try {
					rtm.TasksSetPriority (timeline, rtmList.ID,
					                      rtmList.TaskSeriesCollection[0].TaskSeriesID,
					                      rtmList.TaskSeriesCollection[0].TaskCollection[0].TaskID,
					                      priority);
				} catch (RtmException e) {
					Log.Debug (e.Message);
				}
			}
			
			TryNotify (Catalog.GetString ("New Task Created"),
			           Catalog.GetString ("The task has been successully added to your"
			                              + "Remember The milk task list."));
			
			return new RTMTaskItem (rtmList.ID, rtmList.TaskSeriesCollection[0].TaskSeriesID,
			                        rtmList.TaskSeriesCollection[0].TaskCollection[0].TaskID,
			                        rtmList.TaskSeriesCollection[0].Name,
			                        rtmList.TaskSeriesCollection[0].TaskCollection[0].Due,
			                        rtmList.TaskSeriesCollection[0].TaskCollection[0].Completed,
			                        rtmList.TaskSeriesCollection[0].TaskURL,
			                        priority,
			                        rtmList.TaskSeriesCollection[0].TaskCollection[0].HasDueTime,
			                        rtmList.TaskSeriesCollection[0].TaskCollection[0].Estimate,
			                        rtmList.TaskSeriesCollection[0].LocationID, "");
		}
		
		public static void DeleteTask (string listId, string taskSeriesId, string taskId)
		{
			try {
				rtm.TasksDelete (timeline, listId, taskSeriesId, taskId);
			} catch (RtmException e) {
				Log.Debug (e.Message);
				return;
			}
			
			TryNotify (Catalog.GetString ("Task Deleted"),
			           Catalog.GetString ("The selected task has been successfully deleted"
			                              +" from your Remember The Milk task list"));
		}

		public static void CompleteTask (string listId, string taskSeriesId, string taskId)
		{
			try {
				rtm.TasksComplete (timeline, listId, taskSeriesId, taskId);
			} catch (RtmException e) {
				Log.Debug (e.Message);
				return;
			}
			
			TryNotify (Catalog.GetString ("Task Completed"),
			           Catalog.GetString ("The selected task in your Remember The Milk"
			                              +" task list has been marked as completed."));
		}
		
		public static void SetTaskPriority (string listId, string taskSeriesId, string taskId, string priority)
		{
			try {
				if (priority == "up" || priority == "down")
					rtm.TasksMovePriority (timeline, listId, taskSeriesId, taskId, priority);
				else
					rtm.TasksSetPriority (timeline, listId, taskSeriesId, taskId, priority);
			} catch (RtmException e) {
				Log.Debug (e.Message);
				return;
			}
			
			TryNotify (Catalog.GetString ("Priority Changed"),
			           Catalog.GetString ("The priority of the selected task in your"
			                              +" Remember The Milk task list has been changed."));
		}
		
		public static void SetDueDateTime (string listId, string taskSeriesId, string taskId, string due)
		{
			try {
				if (String.IsNullOrEmpty (due))
					rtm.TasksSetDueDate (timeline, listId, taskSeriesId, taskId);
				else
					rtm.TasksSetDueDateParse (timeline, listId, taskSeriesId, taskId, due);
			} catch (RtmException e) {
				Log.Debug (e.Message);
				return;
			}
			
			if (String.IsNullOrEmpty (due)) 
				TryNotify (Catalog.GetString ("Due Date/Time Unset"),
				           Catalog.GetString ("The due date/time of the selected task in your "
				                              +"Remember The Milk task list has been unset."));
			else 
				TryNotify (Catalog.GetString ("Due Date/Time Changed"),
				           Catalog.GetString ("The due date/time of the selected task in your "
				                              +"Remember The Milk task list has been changed."));
		}
		
		public static void MoveTask (string fromListId, string toListId, string taskSeriesId, string taskId)
		{
			try {
				rtm.TasksMoveTo (timeline, fromListId, toListId, taskSeriesId, taskId);
			} catch (RtmException e) {
				Log.Debug (e.Message);
				return;
			}
			
			TryNotify (Catalog.GetString ("Task Moved"),
			           Catalog.GetString (String.Format ("The selected task has been moved from"
			                                             + " Remember The Milk list \"{0}\" to list \"{1}\".",
			                                             lists.Find (i => (i as RTMListItem).Id == fromListId).Name,
			                                             lists.Find (i => (i as RTMListItem).Id == toListId).Name)));
		}
		
		public static void RenameTask (string listId, string taskSeriesId, string taskId, string newName)
		{
			try {
				rtm.TasksSetName (timeline, listId, taskSeriesId, taskId, newName);
			} catch (RtmException e) {
				Log.Debug (e.Message);
				return;
			}
			
			TryNotify (Catalog.GetString ("Task Renamed"),
			           Catalog.GetString (String.Format ("The selected task has"
			                                             + " been renamed to \"{0}\".", newName)));
		}
		
		public static void PostponeTask (string listId, string taskSeriesId, string taskId)
		{
			try {
				rtm.TasksPostpone (timeline, listId, taskSeriesId, taskId);
			} catch (RtmException e) {
				Log.Debug (e.Message);
				return;
			}
			
			TryNotify (Catalog.GetString ("Task Postponed"),
			           Catalog.GetString ("The selected task in your Remember The Milk task"
			                              + " list has been postponed"));
		}
		
		public static void SetRecurrence (string listId, string taskSeriesId, string taskId, string repeat)
		{
			try {
				rtm.TasksSetRecurrence (timeline, listId, taskSeriesId, taskId, repeat);
			} catch (RtmException e) {
				Log.Debug (e.Message);
				return;
			}
			
			TryNotify (Catalog.GetString ("Recurrence Pattern Changed"),
			           Catalog.GetString ("The recurrence pattern of the selected task in your"
			                              + " Remember The Milk task list has been changed."));
		}
		
		public static void SetURL(string listId, string taskSeriesId, string taskId, string url)
		{
			try {
				rtm.TasksSetUrl(timeline, listId, taskSeriesId, taskId, url);
			} catch (RtmException e) {
				Log.Debug (e.Message);
				return;
			}
			
			if (!string.IsNullOrEmpty(url)) {
				TryNotify (Catalog.GetString ("Task URL Set"),
				           Catalog.GetString ("The selected task has been assigned a URL."));
			} else {
				TryNotify (Catalog.GetString ("Task URL Reset"),
				           Catalog.GetString ("The URL for the selected task has been reset."));
			}
		}
		
		public static void SetEstimateTime (string listId, string taskSeriesId, string taskId, string estimateTime)
		{
			try {
				rtm.TasksSetEstimateTime(timeline, listId, taskSeriesId,
				                         taskId, estimateTime);
			} catch (RtmException e) {
				Log.Debug (e.Message);
				return;
			}
			
			if (String.IsNullOrEmpty(estimateTime))
				TryNotify (Catalog.GetString ("Task Estimated Time Unset"),
				           Catalog.GetString ("The estimated time for the selected task has been unset."));
			else
				TryNotify (Catalog.GetString ("Task Estimated Time Set"),
				           Catalog.GetString ("The selected task has been assigned an estimated time."));
		}
		
		public static void SetLocation (string listId, string taskSeriesId, string taskId, string locationId)
		{
			try {
				rtm.TasksSetLocation (timeline, listId, taskSeriesId, taskId, locationId);
			} catch (RtmException e) {
				Log.Debug (e.Message);
				return;
			}
			
			if (string.IsNullOrEmpty (locationId)) {
				TryNotify (Catalog.GetString ("Location Reset"),
				           Catalog.GetString ("The location of the selected task has been cleared."));
			} else {
				TryNotify (Catalog.GetString ("Location changed"),
				           Catalog.GetString ("The location of the selected task has been successfully changed."));
			}
			
		}
		
		public static void UncompleteTask (string listId, string taskSeriesId, string taskId)
		{
			try {
				rtm.TasksUncomplete (timeline, listId, taskSeriesId, taskId);
			} catch (RtmException e) {
				Log.Debug (e.Message);
				return;
			}
			
			TryNotify (Catalog.GetString ("Task Uncompleted"),
			           Catalog.GetString ("The selected task has been marked as \"incomplete\"."));
		}
		
		#endregion [ Task Actions ]
		
		#region [ List Actions ]
		
		public static void NewList(string newListName)
		{
			if (IsProtectedList (newListName)) {
				Services.Notifications.Notify ("Invalid List Name", "The provided new list name is reserved.", 
				                               RTMIconPath);
				return;
			}
			
			try {
				rtm.ListsNew(timeline, newListName);
			} catch (RtmException e) {
				Log.Debug (e.Message);
				return;
			}
			
			TryNotify (Catalog.GetString ("New List Created"),
			           Catalog.GetString (String.Format ("A new task list named \"{0}\" has been created.", newListName)));
		}
		
		public static void DeleteList(string listId)
		{
			try {
				rtm.ListsDelete(timeline, listId);
			} catch (RtmException e) {
				Log.Debug (e.Message);
				return;
			}
			
			TryNotify (Catalog.GetString("List Deleted"),
			           Catalog.GetString("The selected task list has been deleted."));
		}      
		
		public static void RenameList(string listId, string newListName)
		{
			if (IsProtectedList (newListName)) {
				Services.Notifications.Notify ("Invalid List Name", "The provided new list name is reserved.", 
				                               RTMIconPath);
				return;
			}
			
			try {
				rtm.ListsRename(timeline, listId, newListName);
			} catch (RtmException e) {
				Log.Debug (e.Message);
				return;
			}
			TryNotify (Catalog.GetString("Task List Renamed"),
			           Catalog.GetString(String.Format ("The selected task"
			                                            + " list has been renamed to \"{0}\".", newListName)));
		}
		
		#endregion [ List Actions ]
		
		#region [ Note Actions ]
		
		public static void NewNote (string listId, string taskSeriesId, string taskId, string note)
		{
			string[] parts = null;
			string note_title;
			string note_body;
			bool has_separator = (0 < note.IndexOf ("|") && note.IndexOf ("|") < note.Length);
			bool has_newline = (0 < note.IndexOf ("\n") && note.IndexOf ("\n") < note.Length);
			bool newline_first = note.IndexOf ("\n") < note.IndexOf ("|");

			
			if ((has_newline && has_separator && newline_first) || (has_newline && !has_separator)) {
				parts = note.Split(new char[] {'\n'}, 2);
			} else if (has_separator) {
				parts = note.Split(new char[] {'|'}, 2);
			} 
			
			if (string.IsNullOrEmpty (note) || ((has_separator || has_newline) && parts != null && parts.Length < 2)) {
				Log.Debug ("Entered text cannot be used as a note.");
				return;
			} else {
				note_title = (has_separator || has_newline) ? parts[0].Trim () : "Untitled Note";
				note_body = (has_separator || has_newline) ? parts[1].Trim () : note;
			}
			
			try {
				rtm.NotesAdd (timeline, listId, taskSeriesId, taskId, note_title, note_body);
			} catch (RtmException e) {
				Log.Debug (e.Message);
				return;
			}
			
			TryNotify (Catalog.GetString ("Note Added"),
			           Catalog.GetString ("A note has been added to the selected task"));
		}
		
		public static void DeleteNote (string noteId)
		{
			try {
				rtm.NotesDelete (timeline, noteId);
			} catch (RtmException e) {
				Log.Debug (e.Message);
				return;
			}

			lock (note_lock)
				notes.Remove (notes.Find (i => (i as RTMNoteItem).Id == noteId));
			
			TryNotify (Catalog.GetString ("Note Deleted"),
			           Catalog.GetString ("The selected note has been deleted from the selected task"));
		}
		
		#endregion [ Note Actions ]
		
		#region [ Tag Actions ]
		
		public static void AddTags (string listId, string taskSeriesId, string taskId, string tags)
		{
			if (String.IsNullOrEmpty (tags)) {
				Log.Debug ("[RememberTheMilk] Tags to add is empty or null string.");
			} else {
				try {
					rtm.TasksAddTags (timeline, listId, taskSeriesId, taskId, tags);
				} catch (RtmException e) {
					Log.Debug (e.Message);
					return;
				}
				
				TryNotify (Catalog.GetString ("Tags Added"),
				           Catalog.GetString ("New tags have been successfully added to the selected task."));
			}
		}

		public static void DeleteTags (string listId, string taskSeriesId, string taskId, string tags)
		{
			if (String.IsNullOrEmpty (tags)) {
				Log.Debug ("[RememberTheMilk] Tags to delete is empty or null string.");
			} else {
				try {
					rtm.TasksRemoveTags (timeline, listId, taskSeriesId, taskId, tags);
				} catch (RtmException e) {
					Log.Debug (e.Message);
					return;
				}
				
				TryNotify (Catalog.GetString ("Tags Deleted"),
				           Catalog.GetString ("Selected tags have been successfully removed from the selected task."));
			}
		}
		
		#endregion [ Tag Actions ]
		
		#region [ Utilities ]

		static bool TryConnect ()
		{
			if (!String.IsNullOrEmpty (Preferences.Token)) {
				Auth auth;
				try {
					auth = rtm.AuthCheckToken (Preferences.Token);
				} catch (RtmException e) {
					Log.Error (Catalog.GetString ("Token verification failed."), e.Message);
					return false;
				}
				
				rtm.AuthToken = auth.Token;
				username = auth.User.Username;
				
				try {
					timeline = rtm.TimelineCreate ();
				} catch (RtmException e) {
					Log.Error (Catalog.GetString ("Remember The Milk timeline creation failed."), e.Message);
					return false;
				}
				
				return true;
			} else {
				Log.Error (Catalog.GetString ("Not authorized to use an Remember The Milk account."));
				return false;
			}
		}
		
		static void TryRemoveTask (string taskId)
		{
			Item task = tasks.Find (i => (i as RTMTaskItem).Id == taskId);
			if (task != null)
				lock (task_lock) 
					tasks.Remove (task);
		}
		
		/// <summary>
		/// Check if there is overdue task in All Tasks list,
		/// when user chooses to be notified, display the information.
		/// </summary>
		static void NotifyOverDueItems ()
		{
			List<Item> overdue_tasks;
			object list_lock = new object ();
			overdue_tasks = new List<Item> ();
			overdue_tasks = tasks.FindAll (i => IsOverdue (i as RTMTaskItem));      
			
			int len = overdue_tasks.ToArray ().Length;
			if (len > 0) {
				string title;
				title = String.Format (Catalog.GetPluralString ("{0} Task Overdue",
				                                                "{0} Tasks Overdue", len), len);
				
				string body = "";
				lock (list_lock) {
					foreach (Item item in overdue_tasks)
						body += ("- " + (item as RTMTaskItem).Name +"\n");
				}
				Do.Platform.Services.Notifications.Notify (new Do.Platform.Notification( title, body, "task-overdue.png@" + typeof(RTMTaskItem).Assembly.FullName));
			}
		}

		static bool IsOverdue (RTMTaskItem item)
		{
			return (item.Completed == DateTime.MinValue && item.Due > DateTime.MinValue &&
			        ((item.HasDueTime == 1 && item.Due < DateTime.Now) || item.Due.Date < DateTime.Today));
		}
		
		static bool IsProtectedList (string listName)
		{
			Item item = lists.Find (i => (i as RTMListItem).Name == listName);
			if (item != null)
				return (item as RTMListItem).Locked;
			
			return false;
		}
		
		static void TryNotify (string title, string body)
		{
			if (Preferences.ActionNotification) {
				Do.Platform.Services.Notifications.Notify (new Do.Platform.Notification (title, body, RTMIconPath));
			}
		}
		
		#endregion [ Utilities ]
		
	}
}

/* RTM.cs
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
		private static Rtm rtm;
		private static Dictionary<string,List<Item>> tasks;
		private static Dictionary<string,Item> lists;
		private static Dictionary<string,Item> notes;
		private static Dictionary<string,Item> locations;
		private static object lists_lock;
		private static object lists_lock_at;
		private static object dict_lock;
		private static object notes_lock;
		private static object loc_lock;
		private static string timeline;
		private static DateTime last_sync;
		private static string username;
		private static string filter;
		
		private const string ApiKey = "ee32c06f2d45baf935a2c046323457d8";
		private const string SharedSecret = "1b835b123a903938";
		
		static RTM ()
		{
			rtm = new Rtm (ApiKey, SharedSecret);
			tasks = new Dictionary<string,List<Item>> ();
			lists = new Dictionary<string,Item> ();
			notes = new Dictionary<string,Item> ();
			locations = new Dictionary<string,Item> ();
			lists_lock = new object ();
			lists_lock_at = new object ();
			dict_lock = new object ();
			notes_lock =  new object ();
			loc_lock =  new object ();
			last_sync = DateTime.MinValue;
			Preferences = new RTMPreferences ();
			filter = Preferences.Filter;
			
			TryConnect ();
		}
		
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
		
		public static RTMPreferences Preferences { get; private set; }
		
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
		
		public static List<Item> Lists {
			get {
				List<Item> lists2 = new List<Item> ();
				lists2.Clear ();
				lists2.Add (new RTMListItem ("All Tasks", "All Tasks"));
				
				lock (dict_lock)
					foreach (KeyValuePair<string,Item> kvp in lists)
						lists2.Add (kvp.Value);
				
				return lists2;
			}
		}
		
		public static List<Item> Tasks {
			get {
				return tasks ["All Tasks"];
			}
		}
		
		public static List<Item> Notes {
			get {
				List<Item> notes2 = new List<Item> ();
				notes2.Clear ();
				
				lock (notes_lock)
					foreach (KeyValuePair<string,Item> kvp in notes)
						notes2.Add (kvp.Value);
				return notes2;
			}
		}
		
		public static List<Item> Locations {
			get {
				List<Item> locations2 = new List<Item> ();
				locations2.Clear ();
				
				lock (loc_lock)
					foreach (KeyValuePair<string,Item> kvp in locations)
						locations2.Add (kvp.Value);
				
				return locations2;
			}
		}
		
		public static List<Item> TasksForList (string listId)
		{
			return tasks [listId];
		}
		
		public static List<Item> AttributesForTask (RTMTaskItem task)
		{
			List<Item> attribute_list = new List<Item> ();
			
			// attribute_list.Add (new RTMTaskAttributeItem ("Name", task.Name, task.Url, task.Icon));
			
			if (task.Due != DateTime.MinValue)
				attribute_list.Add (new RTMTaskAttributeItem (task.Due.ToString ((task.HasDueTime != 0) ? "g" : "d"),
				                                              "Due Date/Time",
				                                              task.Url, "stock_calendar"));
			if (!String.IsNullOrEmpty (task.TaskUrl))
				attribute_list.Add (new RTMTaskAttributeItem (task.TaskUrl, "URL",
				                                              task.TaskUrl, "text-html"));
			if (!String.IsNullOrEmpty (task.Estimate))
				attribute_list.Add (new RTMTaskAttributeItem (task.Estimate, "Time Estimate",
				                                              task.Url, "stock_appointment-reminder"));
			if (!String.IsNullOrEmpty (task.LocationId)) {
				attribute_list.Add (locations [task.LocationId] as RTMLocationItem);
//				RTMLocationItem loc = locations [task.LocationId] as RTMLocationItem;
//				attribute_list.Add (new RTMTaskAttributeItem (loc.Description, 
//				                                              "Location [" + loc.Name + "] ",
//				                                              "http://maps.google.com/maps?q=" + loc.Latitude + "," + loc.Longitude,
//				                                              loc.Icon));
			}
			
			if (notes.ContainsKey (task.Id))
				attribute_list.Add (notes [task.Id] as RTMTaskAttributeItem);
			
			return attribute_list;
		}
		
		public static string ListNameForList (string listId)
		{
			return lists [listId].Name;
		}
		
		public static void UpdateLists ()
		{
			if (!IsAuthenticated)
				if (!TryConnect ())
					return;
			
			Lists rtmLists;
			try {
				rtmLists = rtm.ListsGetList ();
			} catch (RtmException e) {
				Log.Error (Catalog.GetString ("An error happend when updating RTM lists."), e.Message);
				rtmLists = null;
				return;
			}
			
			lists.Clear ();
			foreach (List rtmList in rtmLists.listCollection)
				if (rtmList.Deleted == 0 && rtmList.Smart == 0)
					lists [rtmList.ID] = new RTMListItem (rtmList.ID, rtmList.Name);
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
				Log.Error (Catalog.GetString ("An error happend when updating RTM locations."), e.Message);
				rtmLocations = null;
				return;
			}
			
			locations.Clear ();
			if (rtmLocations.locationCollection.Length > 0) {
				foreach (Location rtmLocation in rtmLocations.locationCollection) {
					locations [rtmLocation.ID] = new RTMLocationItem (rtmLocation.ID, 
					                                                  rtmLocation.Name, 
					                                                  rtmLocation.Address,
					                                                  rtmLocation.Longitude, 
					                                                  rtmLocation.Latitude);
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
				tasks ["All Tasks"] = new List<Item> ();
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
				Log.Error (Catalog.GetString ("An error happend when updating RTM tasks."), e.Message);
				return;
			}
			
			foreach (List rtmList in rtmTasks.ListCollection) {
				if (!tasks.ContainsKey (rtmList.ID))
					tasks [rtmList.ID] = new List<Item> ();
				
				 if (rtmList.DeletedTaskSeries != null)
					foreach (TaskSeries rtmTaskSeries in rtmList.DeletedTaskSeries.TaskSeriesCollection)
						foreach (Task rtmTask in rtmTaskSeries.TaskCollection)
							UniverseRemoveTask (rtmTask.TaskID, rtmList.ID);
				
				if (rtmList.TaskSeriesCollection != null) {
					foreach (TaskSeries rtmTaskSeries in rtmList.TaskSeriesCollection) {
						foreach (Task rtmTask in rtmTaskSeries.TaskCollection) {
							// delete one recurrent task will cause other deleted instances
							// appear in the taskseries tag, so here we need to check again.
							if (rtmTask.Deleted == DateTime.MinValue) {
								if (rtmTaskSeries.Notes.NoteCollection.Length > 0) {
									List<RTMTaskNoteItem> note_list = new List<RTMTaskNoteItem> ();
									note_list.Clear ();
									foreach (Note rtmNote in rtmTaskSeries.Notes.NoteCollection)
										note_list.Add (new RTMTaskNoteItem (rtmNote.Title, rtmNote.Text, rtmNote.ID));
									
									string desc1 = String.Format (Catalog.GetPluralString
									                              ("{0} note is",
									                               "{0} notes are",
									                               rtmTaskSeries.Notes.NoteCollection.Length),
									                              rtmTaskSeries.Notes.NoteCollection.Length);
									string desc2 = Catalog.GetString (" associated with this task");
									string url =  "http://www.rememberthemilk.com/print/" +
										RTM.Preferences.Username + "/" + rtmList.ID + "/" + rtmTask.TaskID + "/notes/";
									
									notes [rtmTask.TaskID] =
										new RTMTaskAttributeNotes (Catalog.GetString("Notes"),
										                           desc1+desc2,
										                           url,
										                           note_list);
								}
								
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
								                                        rtmTaskSeries.LocationID);
								tasks [rtmList.ID].Add (new_task);
								tasks ["All Tasks"].Add (new_task);
							}
						}
					}
				}
			}
			
			last_sync = DateTime.Now;
			
			if (Preferences.OverdueNotification)
				NotifyOverDueItems ();
        }
		
		public static bool IsProtectedList (string listName)
		{
			const string ProtectedListPattern = @"^(All\ Tasks|Inbox|Sent)$";
			return new Regex (ProtectedListPattern, RegexOptions.Compiled).IsMatch (listName);
		}
		
		/// <summary>
		/// Remove task identified by taskId from list identified by listId
		/// and from 'All Tasks' list
		/// </summary>
		/// <param name="taskId">
		/// A <see cref="System.String"/>, to identify the task to remove
		/// </param>
		/// <param name="listId">
		/// A <see cref="System.String"/>, to identify the list from which to remove
		/// the task
		/// </param>
		private static void UniverseRemoveTask (string taskId, string listId)
		{
			lock (lists_lock) {
				foreach (RTMTaskItem task in tasks [listId].ToArray ())
					if (task.Id == taskId)
						tasks [listId].Remove (task);
			}
			
			lock (lists_lock_at) {
				foreach (RTMTaskItem task in tasks ["All Tasks"].ToArray ())
					if (task.Id == taskId)
						tasks ["All Tasks"].Remove (task);
			}
		}
		
		/// <summary>
		/// Check if there is overdue task in All Tasks list,
		/// when user chooses to be notified, display the information.
		/// </summary>
		private static void NotifyOverDueItems ()
		{
			List<string> overdue_tasks;
			overdue_tasks = new List<string> ();
			lock (lists_lock_at) {
				foreach (RTMTaskItem task in tasks ["All Tasks"].ToArray ())
					if ((task.Completed == DateTime.MinValue) && (task.Due > DateTime.MinValue) &&
					    ((task.Due < DateTime.Now && task.HasDueTime == 1) || task.Due.Date < DateTime.Today))
						overdue_tasks.Add (task.Name);
			}
			
			int len = overdue_tasks.ToArray ().Length;
			if (len > 0) {
				string title;
				title = String.Format (Catalog.GetPluralString ("{0} Task Overdue",
				                                                "{0} Tasks Overdue", len), len);
				
				string body = "";
				foreach (string name in overdue_tasks) // TODO: missing lock
					body += ("- " + name +"\n");
				
				Do.Platform.Services.Notifications.Notify (new Do.Platform.Notification( title, body, "task-overdue.png@" + typeof(RTMTaskItem).Assembly.FullName ) );
			}
		}
		
		/// <summary>
		/// A wrapper function to complete several common tasks for most action methods:
		/// 1. display notification if user choose to
		/// 2. clear modified task from its list and 'All Tasks' list
		/// 3. update tasks
		/// </summary>
		/// <param name="title">
		/// A <see cref="System.String"/> for the title of the notification message.
		/// </param>
		/// <param name="body">
		/// A <see cref="System.String"/> for the content of the notification message.
		/// </param>
		/// <param name="taskId">
		/// A <see cref="System.String"/>, if exist, will be passed to <see cref="UniverseRemoveTask"/>
		/// </param>
		/// <param name="listId">
		/// A <see cref="System.String"/>, if exist, will be passed to <see cref="UniverseRemoveTask"/>
		/// </param>
		private static void ActionRoutine (string title, string body, string taskId, string listId)
		{
			if (Preferences.ActionNotification) {
				Do.Platform.Services.Notifications.Notify(new Do.Platform.Notification (title, body,
				                                                                        "rtm.png@" + typeof (RTMListItemSource).Assembly.FullName));
			}
			if (taskId != null && listId != null)
				UniverseRemoveTask (taskId, listId);
			UpdateTasks ();
		}
		
		private static void ActionRoutine (string title, string body, string listId)
		{
			if (Preferences.ActionNotification) {
				Do.Platform.Services.Notifications.Notify(new Do.Platform.Notification (title, body,
				                                                                        "rtm.png@" + typeof (RTMListItemSource).Assembly.FullName));
			}
			UpdateLists ();
			UpdateTasks ();
		}
		
		private static void ActionRoutine (string title, string body)
		{
			if (Preferences.ActionNotification) {
				Do.Platform.Services.Notifications.Notify(new Do.Platform.Notification (title, body,
				                                                                        "rtm.png@" + typeof (RTMListItemSource).Assembly.FullName));
			}
			UpdateLists ();
		}
		
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
				Log.Error (e.Message);
				return null;
			}
			
			
			if (priority != "N") {
				try {
					rtm.TasksSetPriority (timeline, rtmList.ID,
					                      rtmList.TaskSeriesCollection[0].TaskSeriesID,
					                      rtmList.TaskSeriesCollection[0].TaskCollection[0].TaskID,
					                      priority);
				} catch (RtmException e) {
					Log.Error (e.Message);
				}
			}
			
			ActionRoutine (Catalog.GetString ("New Task Created"),
			               Catalog.GetString ("The task has been successully added to your"
			                                  + "Remember The milk task likst."),
			               null, null);
			
			return new RTMTaskItem (rtmList.ID, rtmList.TaskSeriesCollection[0].TaskSeriesID,
			                        rtmList.TaskSeriesCollection[0].TaskCollection[0].TaskID,
			                        rtmList.TaskSeriesCollection[0].Name,
			                        rtmList.TaskSeriesCollection[0].TaskCollection[0].Due,
			                        rtmList.TaskSeriesCollection[0].TaskCollection[0].Completed,
			                        rtmList.TaskSeriesCollection[0].TaskURL,
			                        priority,
			                        rtmList.TaskSeriesCollection[0].TaskCollection[0].HasDueTime,
			                        rtmList.TaskSeriesCollection[0].TaskCollection[0].Estimate,
			                        rtmList.TaskSeriesCollection[0].LocationID);
		}
		
		public static void DeleteTask (string listId, string taskSeriesId, string taskId)
		{
			try {
				rtm.TasksDelete (timeline, listId, taskSeriesId, taskId);
			} catch (RtmException e) {
				Log.Error (e.Message);
				return;
			}
			
			ActionRoutine (Catalog.GetString ("Task Deleted"),
			               Catalog.GetString ("The selected task has been successfully deleted"
			                                  +" from your Remember The Milk task list"),
			               null, null);
		}

		public static void CompleteTask (string listId, string taskSeriesId, string taskId)
		{
			try {
				rtm.TasksComplete (timeline, listId, taskSeriesId, taskId);
			} catch (RtmException e) {
				Log.Error (e.Message);
				return;
			}
			
			ActionRoutine (Catalog.GetString ("Task Completed"),
			               Catalog.GetString ("The selected task in your Remember The Milk"
			                                  +" task list has been marked as completed."),
			               taskId, listId);
		}
		
		public static List<Item> GeneratePriorities ()
		{
			List<Item> priorities = new List<Item> ();
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
			return priorities;
		}
		
		public static void SetTaskPriority (string listId, string taskSeriesId, string taskId, string priority)
		{
			try {
				if (priority == "up" || priority == "down")
					rtm.TasksMovePriority (timeline, listId, taskSeriesId, taskId, priority);
				else
					rtm.TasksSetPriority (timeline, listId, taskSeriesId, taskId, priority);
			} catch (RtmException e) {
				Log.Error (e.Message);
				return;
			}
			
			ActionRoutine (Catalog.GetString ("Priority Changed"),
			               Catalog.GetString ("The priority of the selected task in your"
			                                  +" Remember The Milk task list has been changed."),
			               taskId, listId);
		}
		
		public static void SetDueDateTime (string listId, string taskSeriesId, string taskId, string due)
		{
			try {
				if (String.IsNullOrEmpty (due))
					rtm.TasksSetDueDate (timeline, listId, taskSeriesId, taskId);
				else
					rtm.TasksSetDueDateParse (timeline, listId, taskSeriesId, taskId, due);
			} catch (RtmException e) {
				Log.Error (e.Message);
				return;
			}
			
			ActionRoutine (Catalog.GetString ("Due Date/Time Changed"),
			               Catalog.GetString ("The due date/time of the selected task in your "
			                                  +"Remember The Milk task list has been changed."),
			               taskId, listId);
		}
		
		public static void MoveTask (string fromListId, string toListId, string taskSeriesId, string taskId)
		{
			try {
				rtm.TasksMoveTo (timeline, fromListId, toListId, taskSeriesId, taskId);
			} catch (RtmException e) {
				Log.Error (e.Message);
				return;
			}
			
			ActionRoutine (Catalog.GetString ("Task Moved"),
			               Catalog.GetString (String.Format ("The selected task has been moved from"
			                                                 + " Remember The Milk list \"{0}\" to list \"{1}\".",
			                                                 lists [fromListId].Name, lists [toListId].Name)),
			               taskId, fromListId);
		}
		
		public static void RenameTask (string listId, string taskSeriesId, string taskId, string newName)
		{
			try {
				rtm.TasksSetName (timeline, listId, taskSeriesId, taskId, newName);
			} catch (RtmException e) {
				Log.Error (e.Message);
				return;
			}
			
			ActionRoutine (Catalog.GetString ("Task Renamed"),
			               Catalog.GetString (String.Format ("The selected task has"
			                                                 + " been renamed to \"{0}\".", newName)),
			               taskId, listId);
		}
		
		public static void PostponeTask (string listId, string taskSeriesId, string taskId)
		{
			try {
				rtm.TasksPostpone (timeline, listId, taskSeriesId, taskId);
			} catch (RtmException e) {
				Log.Error (e.Message);
				return;
			}
			
			ActionRoutine (Catalog.GetString ("Task Postponed"),
			               Catalog.GetString ("The selected task in your Remember The Milk task"
			                                  + " list has been postponed"),
			               taskId, listId);
		}
		
		public static void SetRecurrence (string listId, string taskSeriesId, string taskId, string repeat)
		{
			try {
				rtm.TasksSetRecurrence (timeline, listId, taskSeriesId, taskId, repeat);
			} catch (RtmException e) {
				Log.Error (e.Message);
				return;
			}
			
			ActionRoutine (Catalog.GetString ("Recurrence Pattern Changed"),
			               Catalog.GetString ("The recurrence pattern of the selected task in your"
			                                  + " Remember The Milk task list has been changed."),
			               taskId, listId);
		}
		
		public static void SetURL(string listId, string taskSeriesId, string taskId, string url)
		{
			try {
				rtm.TasksSetUrl(timeline, listId, taskSeriesId, taskId, url);
			} catch (RtmException e) {
				Log.Error (e.Message);
				return;
			}
			
			if (!string.IsNullOrEmpty(url)) {
				ActionRoutine (Catalog.GetString ("Task URL Set"),
				               Catalog.GetString ("The selected task has been assigned a URL."),
				               taskId, listId);
			} else {
				ActionRoutine (Catalog.GetString ("Task URL Reset"),
				               Catalog.GetString ("The URL for the selected task has been reset."),
				               taskId, listId);
			}
		}
		
		public static void SetEstimateTime (string listId, string taskSeriesId, string taskId, string estimateTime)
		{
			try {
				rtm.TasksSetEstimateTime(timeline, listId, taskSeriesId,
				                         taskId, estimateTime);
			} catch (RtmException e) {
				Log.Error (e.Message);
				return;
			}
			
			if (!string.IsNullOrEmpty(estimateTime)) {
				ActionRoutine (Catalog.GetString ("Task Estimated Time Set"),
				               Catalog.GetString ("The selected task has been assigned an estimated time."),
				               taskId, listId);
			} else {
				ActionRoutine (Catalog.GetString ("Task Estimated Time Reset"),
				               Catalog.GetString ("The estimated time for the selected task has been reset."),
				               taskId, listId);
			}
		}
		
		public static void SetLocation (string listId, string taskSeriesId, string taskId, string locationId)
		{
			try {
				rtm.TasksSetLocation (timeline, listId, taskSeriesId, taskId, locationId);
			} catch (RtmException e) {
				Log.Error (e.Message);
				return;
			}
			
			if (string.IsNullOrEmpty (locationId)) {
				ActionRoutine (Catalog.GetString ("Location Reset"),
				               Catalog.GetString ("The location of the selected task has been cleared."),
				               taskId, listId);
			} else {
				ActionRoutine (Catalog.GetString ("Location changed"),
				               Catalog.GetString ("The location of the selected task has been successfully changed."),
				               taskId, listId);
			}
			
		}
		
		public static void UncompleteTask (string listId, string taskSeriesId, string taskId)
		{
			try {
				rtm.TasksUncomplete (timeline, listId, taskSeriesId, taskId);
			} catch (RtmException e) {
				Log.Error (e.Message);
				return;
			}
			
			ActionRoutine (Catalog.GetString ("Task Uncompleted"),
			               Catalog.GetString ("The selected task has been marked as \"incomplete\"."),
			               taskId, listId);
		}

		public static void NewList(string newListName)
		{
			try {
				rtm.ListsNew(timeline, newListName);
			} catch (RtmException e) {
				Log.Error (e.Message);
				return;
			}
			
			ActionRoutine (Catalog.GetString("New List Created"),
			               Catalog.GetString(String.Format ("A new task"
			                                                + " list named \"{0}\" has been created.", newListName)));
		}
		
		public static void DeleteList(string listId)
		{
			try {
				rtm.ListsDelete(timeline, listId);
			} catch (RtmException e) {
				Log.Error (e.Message);
				return;
			}
			
			ActionRoutine (Catalog.GetString("List Deleted"),
			               Catalog.GetString("The selected task list has been deleted."));
		}      
		
		public static void RenameList(string listId, string newListName)
		{
			try {
				rtm.ListsRename(timeline, listId, newListName);
			} catch (RtmException e) {
				Log.Error (e.Message);
				return;
			}
			ActionRoutine (Catalog.GetString("Task List Renamed"),
			               Catalog.GetString(String.Format ("The selected task"
			                                                + " list has been renamed to \"{0}\".", newListName)),
			               listId);
		}
		
		public static void NewNote (string listId, string taskSeriesId, string taskId, string note)
		{
			string[] parts = null;
			string note_title;
			string note_body;
			bool has_separator = (0 < note.IndexOf ("|") && note.IndexOf ("|") < note.Length);
			bool has_newline = (0 < note.IndexOf ("\n") && note.IndexOf ("\n") < note.Length) ;
			
			if (has_separator) {      // when separator is used, don't care about newlines
				parts = note.Split(new char[] {'|'}, 2);
			} else if (has_newline) {
				parts = note.Split(new char[] {'\n'}, 2);
			}
			
			if (string.IsNullOrEmpty (note) || ((has_separator || has_newline) && parts != null && parts.Length < 2)) {
				Log.Error ("Entered text cannot be used as a note.");
				return;
			} else {
				note_title = (has_separator || has_newline) ? parts[0].Trim () : "Untitled Note";
				note_body = (has_separator || has_newline) ? parts[1].Trim () : note;
			}
			
			try {
				rtm.NotesAdd (timeline, listId, taskSeriesId, taskId, note_title, note_body);
			} catch (RtmException e) {
				Log.Error (e.Message);
				return;
			}
			
			ActionRoutine (Catalog.GetString ("Note Added"),
			               Catalog.GetString ("A note has been added to the selected Remember The Milk task"),
			               taskId, listId);
		}

	}
}

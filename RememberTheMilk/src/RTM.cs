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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Mono.Addins;
using RtmNet;


using Do.Universe;


namespace RememberTheMilk
{
    public static class RTM
    {
        private static Rtm rtm;
        private static Dictionary<string,List<Item>> tasks;
        private static Dictionary<string,Item> lists;
        private static object lists_lock;
        private static object lists_lock_at;
        private static object dict_lock;
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
            lists_lock = new object ();
            lists_lock_at = new object ();
            dict_lock = new object ();
            last_sync = DateTime.MinValue;
			Preferences = new RTMPreferences ();
			filter = Preferences.Filter;
			
            if (!String.IsNullOrEmpty (Preferences.Token)) {
                Auth auth;
                try {
                    auth = rtm.AuthCheckToken (Preferences.Token);
                } catch (RtmException e) {
                    Console.Error.WriteLine ("Token verification failed: " + e.Message);
                    return;
                }
				
                rtm.AuthToken = auth.Token;
                timeline = rtm.TimelineCreate ();
                username = auth.User.Username;
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
                Console.Error.WriteLine ("Fail to initialize authentication: " + e.Message);
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
                Console.Error.WriteLine ("Fails to complete authentication: " + e.Message);
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

        public static void UpdateLists ()
        {
			if (!IsAuthenticated)
				return;
			
            Lists rtmLists;
            try {
                rtmLists = rtm.ListsGetList ();
            } catch (RtmException e) {
                Console.Error.WriteLine (e.Message);
                rtmLists = null;
                return;
            }

            lists.Clear ();
            foreach (List rtmList in rtmLists.listCollection)
                if (rtmList.Deleted == 0 && rtmList.Smart == 0)
                    lists [rtmList.ID] = new RTMListItem (rtmList.ID, rtmList.Name);
        }

        public static List<Item> TasksForList (string listId)
        {
            return tasks [listId];
        }

        public static string ListNameForList (string listId)
        {
            return lists [listId].Name;
        }

        public static void UpdateTasks ()
        {

			if (!IsAuthenticated)
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
                Console.Error.WriteLine (e.Message);
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
								tasks [rtmList.ID].Add (new RTMTaskItem (rtmList.ID,
									rtmTaskSeries.TaskSeriesID,
								        rtmTask.TaskID,
								        rtmTaskSeries.Name,
								        rtmTask.Due,
								        rtmTask.Completed,
								        rtmTask.TaskURL,
								        rtmTask.Priority,
								        rtmTask.HasDueTime));
								tasks ["All Tasks"].Add (new RTMTaskItem (rtmList.ID,
								        rtmTaskSeries.TaskSeriesID,
								        rtmTask.TaskID,
								        rtmTaskSeries.Name,
								        rtmTask.Due,
								        rtmTask.Completed,
								        rtmTask.TaskURL,
								        rtmTask.Priority,
								        rtmTask.HasDueTime));
							}
                        }
                    }
                }
            }

            last_sync = DateTime.Now;
			
			if (Preferences.OverdueNotification)
				NotifyOverDueItems ();
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
				title = String.Format (AddinManager.CurrentLocalizer.GetPluralString ("{0} Task Overdue", 
				                                                "{0} Tasks Overdue", len), len);
//				if (len > 1)
//					title = AddinManager.CurrentLocalizer.GetString (String.Format ("{0} Tasks overdue", len));
//				else
//					title = AddinManager.CurrentLocalizer.GetString ("1 Task Overdue");
				
				string body = "";
				foreach (string name in overdue_tasks) // TODO: missing lock
					body += ("- " + name +"\n");
				
				Do.Platform.Services.Notifications.Notify(new Do.Platform.Notification( title, body, "task-overdue.png@" + typeof(RTMTaskItem).Assembly.FullName ) );
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
				Do.Platform.Services.Notifications.Notify( new Do.Platform.Notification( title, body, 
				                                                                        "rtm.png@" + typeof(RTMTaskItem).Assembly.FullName ) );
			}
			if (taskId != null && listId != null)
				UniverseRemoveTask (taskId, listId);
			UpdateTasks ();
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
                Console.Error.WriteLine (e.Message);
                return null;
            }


            if (priority != "N") {
                try {
                    rtm.TasksSetPriority (timeline, rtmList.ID, 
					                      rtmList.TaskSeriesCollection[0].TaskSeriesID,
                                          rtmList.TaskSeriesCollection[0].TaskCollection[0].TaskID, 
					                      priority);
                } catch (RtmException e) {
                    Console.Error.WriteLine (e.Message);
                }
            }

            UpdateTasks ();
            return new RTMTaskItem (rtmList.ID, rtmList.TaskSeriesCollection[0].TaskSeriesID,
                                    rtmList.TaskSeriesCollection[0].TaskCollection[0].TaskID,
                                    rtmList.TaskSeriesCollection[0].Name,
                                    rtmList.TaskSeriesCollection[0].TaskCollection[0].Due, 
			            rtmList.TaskSeriesCollection[0].TaskCollection[0].Completed, 
			            rtmList.TaskSeriesCollection[0].TaskCollection[0].TaskURL,
			            priority,
                                    rtmList.TaskSeriesCollection[0].TaskCollection[0].HasDueTime);
        }

        public static void DeleteTask (string listId, string taskSeriesId, string taskId)
        {
            try {
                rtm.TasksDelete (timeline, listId, taskSeriesId, taskId);
            } catch (RtmException e) {
                Console.Error.WriteLine (e.Message);
                return;
            }

            ActionRoutine (AddinManager.CurrentLocalizer.GetString ("Task Deleted"),
			               AddinManager.CurrentLocalizer.GetString ("The selected task has been successfully deleted"
			                                  +" from your Remember The Milk task list"),
			               null, null);
        }

        public static void CompleteTask (string listId, string taskSeriesId, string taskId)
        {
            try {
                rtm.TasksComplete (timeline, listId, taskSeriesId, taskId);
            } catch (RtmException e) {
                Console.Error.WriteLine (e.Message);
                return;
            }

            ActionRoutine (AddinManager.CurrentLocalizer.GetString ("Task Completed"),
			               AddinManager.CurrentLocalizer.GetString ("The selected task in your Remember The Milk"
			                                  +" task list has been marked as completed."),
			               taskId, listId);
        }
		
        public static List<Item> GeneratePriorities ()
        {
            List<Item> priorities = new List<Item> ();
            priorities.Add (new RTMPriorityItem (AddinManager.CurrentLocalizer.GetString ("High"), 
			                                     AddinManager.CurrentLocalizer.GetString ("High Priority")));
            priorities.Add (new RTMPriorityItem (AddinManager.CurrentLocalizer.GetString ("Medium"), 
			                                     AddinManager.CurrentLocalizer.GetString ("Medium Priority")));
            priorities.Add (new RTMPriorityItem (AddinManager.CurrentLocalizer.GetString ("Low"), 
			                                     AddinManager.CurrentLocalizer.GetString ("Low Priority")));
            priorities.Add (new RTMPriorityItem (AddinManager.CurrentLocalizer.GetString ("None"), 
			                                     AddinManager.CurrentLocalizer.GetString ("No Priority")));
            priorities.Add (new RTMPriorityItem (AddinManager.CurrentLocalizer.GetString ("Up"), 
			                                     AddinManager.CurrentLocalizer.GetString ("Increase the priority")));
            priorities.Add (new RTMPriorityItem (AddinManager.CurrentLocalizer.GetString ("Down"), 
			                                     AddinManager.CurrentLocalizer.GetString ("Decrease the priority")));
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
                Console.Error.WriteLine (e.Message);
                return;
            }

            ActionRoutine (AddinManager.CurrentLocalizer.GetString ("Priority Changed"),
			               AddinManager.CurrentLocalizer.GetString ("The priority of the selected task in your"
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
                Console.Error.WriteLine (e.Message);
                return;
            }

            ActionRoutine (AddinManager.CurrentLocalizer.GetString ("Due Date/Time Changed"),
			               AddinManager.CurrentLocalizer.GetString ("The due date/time of the selected task in your "
			                                  +"Remember The Milk task list has been changed."),
			               taskId, listId);
        }

        public static void MoveTask (string fromListId, string toListId, string taskSeriesId, string taskId)
        {
            try {
                rtm.TasksMoveTo (timeline, fromListId, toListId, taskSeriesId, taskId);
            } catch (RtmException e) {
                Console.Error.WriteLine (e.Message);
                return;
            }

            ActionRoutine (AddinManager.CurrentLocalizer.GetString ("Task Moved"),
			               AddinManager.CurrentLocalizer.GetString (String.Format ("The selected task has been moved from"
			                                                 + " Remember The Milk list \"{0}\" to list \"{1}\".",
			                                                 lists [fromListId].Name, lists [toListId].Name)),
			               taskId, fromListId);
        }

        public static void RenameTask (string listId, string taskSeriesId, string taskId, string newName)
        {
            try {
                rtm.TasksSetName (timeline, listId, taskSeriesId, taskId, newName);
            } catch (RtmException e) {
                Console.Error.WriteLine (e.Message);
                return;
            }

            ActionRoutine (AddinManager.CurrentLocalizer.GetString ("Task Renamed"),
			               AddinManager.CurrentLocalizer.GetString (String.Format ("The selected task has"
			                                                + " been renamed to \"{0}\".", newName)),
			               taskId, listId);
        }

        public static void PostponeTask (string listId, string taskSeriesId, string taskId)
        {
            try {
                rtm.TasksPostpone (timeline, listId, taskSeriesId, taskId);
            } catch (RtmException e) {
                Console.Error.WriteLine (e.Message);
                return;
            }

            ActionRoutine (AddinManager.CurrentLocalizer.GetString ("Task Postponed"),
			               AddinManager.CurrentLocalizer.GetString ("The selected task in your Remember The Milk task"
			                                  + " list has been postponed"),
			               taskId, listId);
		}

        public static void SetRecurrence (string listId, string taskSeriesId, string taskId, string repeat)
        {
			try {
				rtm.TasksSetRecurrence (timeline, listId, taskSeriesId, taskId, repeat);
            } catch (RtmException e) {
                Console.Error.WriteLine (e.Message);
                return;
            }

            ActionRoutine (AddinManager.CurrentLocalizer.GetString ("Recurrence Pattern Changed"),
			               AddinManager.CurrentLocalizer.GetString ("The recurrence pattern of the selected task in your"
			                                  + " Remember The Milk task list has been changed."), 
			               taskId, listId);
        }
        
        public static void SetURL(string listId, string taskSeriesId, string taskId, string url)
        {
        	try {
        		rtm.TasksSetUrl(timeline, listId, taskSeriesId, taskId, url);
        	} catch (RtmException e) {
				Console.Error.WriteLine (e.Message);
				return;
			}
		
			if (!string.IsNullOrEmpty(url)) {
				ActionRoutine (AddinManager.CurrentLocalizer.GetString ("Task URL Set"),
					AddinManager.CurrentLocalizer.GetString ("The selected task has been assigned a URL."),
						taskId, listId);
			} else {
				ActionRoutine (AddinManager.CurrentLocalizer.GetString ("Task URL Reset"),
					AddinManager.CurrentLocalizer.GetString ("The URL for the selected task has been reset."),
						taskId, listId);
			}
        }
		
		public static void UncompleteTask (string listId, string taskSeriesId, string taskId)
		{
			try {
				rtm.TasksUncomplete (timeline, listId, taskSeriesId, taskId);
			} catch (RtmException e) {
				Console.Error.WriteLine (e.Message);
				return;
			}
			
			ActionRoutine (AddinManager.CurrentLocalizer.GetString ("Task Uncompleted"),
			               AddinManager.CurrentLocalizer.GetString ("The selected task has been marked as \"incomplete\"."),
			               taskId, listId);
		}
    }
}

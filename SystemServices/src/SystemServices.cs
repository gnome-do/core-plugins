// Services.cs 
// User: Karol Będkowski at 09:39 2008-10-24
//
//Copyright Karol Będkowski 2008
//
//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Collections.Generic;

using Mono.Posix;
using Mono.Unix;

using Do.Universe;
using Do.Platform;

namespace SystemServices {

	public static class SystemServices {

		// path to search for scripts
		static readonly IEnumerable<string> ServiceDirectories = new [] {
			"/etc/init.d", "/etc/rc/init.d",
		}
		;
		// default services controlled by user
		static readonly IEnumerable<string> DefaultUserServices = new [] {
			"bluetooth", "cups", "kvm", "lighttpd", "mysql",
			"postgresql-8.3", "privoxy", "samba", "tor", "vmware", "vboxdrv", "vboxnet",
		};
		
		// don't show this scripts (i.e. system scriptis)
		static readonly IEnumerable<string> MainBlackList = new [] {
			"acpid", "acpi-support", "alsa-utils", "apmd", "binfmt-support",
			"bootlogd", "bootmisc.sh", "checkfs.sh", "checkroot.sh",
			"console-screen.kbd.sh", "console-setup", "dbus", "dns-clean", "glibc.sh", "hal", "halt", "hostname.sh", 
			"hotkey-setup", "hwclockfirst.sh", "hwclock.sh", "keyboard-setup", "killprocs", "klogd", "laptop-mode",
			"linux-restricted-modules-common", "module-init-tools", "mountall-bootclean.sh", "mountall.sh", 
			"mountdevsubfs.sh", "mountkernfs.sh", "mountnfs-bootclean.sh", "mountnfs.sh", "mountoverflowtmp", "mtab.sh", 
			"policykit", "pppd-dns", "procps", "rc", "rc.local", "rcS", "reboot",	"readahead", "readahead-desktop", 
			"rmnologin", "screen-cleanup", "sendsigs", "single", "stop-bootlogd", "stop-bootlogd-single", "stop-readahead", 
			"sysklogd", "system-tools-backends", "udev", "udev-finish", "umountfs", "umountnfs.sh", "umountroot", "urandom", 
			"vbesave", "wpa-ifupdown", "x11-common",
		};

		// where we save settings in Preferences
		const string PrefsServicesKey = "services";
		const string PrefsSudoCommandKey = "command";
		
		// default program to run with root privs
		const string DefaultSudoCommand = "gksudo";
			
		static IPreferences prefs;
		// list of services selected by user to show
		static List<string>	userServices = null;
		// directory with initd scripts
		static string servicesDirectory = null;

		
		static SystemServices () {
			prefs = Services.Preferences.Get<SystemServicesConfig> ();

			// load user services
			string services = prefs.Get (PrefsServicesKey, "");
			if (string.IsNullOrEmpty (services)) { 
				// load default list
				userServices = new List<string> (DefaultUserServices);
			} else {
				userServices = new List<string> (services.Split ());
			}

			// find directory with scripts
			foreach (string dir in ServiceDirectories) {
				if (Directory.Exists (dir)) {
					servicesDirectory = dir;
					break;
				}
			}

			if (string.IsNullOrEmpty (servicesDirectory)) {
				Log.Error ("err: Sevices.FindServicesDirectory - not found dir with scripts");			
			}
		}

		//// <value>
		/// Program to run services (sudo, gksudo, etc...)
		/// </value>
		public static string SudoCommand {
			get { return prefs.Get (PrefsSudoCommandKey, DefaultSudoCommand); }
			set { prefs.Set (PrefsSudoCommandKey, value); }
		}

		
		/// <summary>
		/// Get list names of services selected by user.
		/// </summary>
		public static List<Item> LoadServices () {
			List<Item> items = new List<Item> ();
			
			if (string.IsNullOrEmpty (servicesDirectory)) {
				return items;
			}

			foreach (string service in userServices) {
				// is selected script exists?
				if (File.Exists (Path.Combine (servicesDirectory, service))) {
					items.Add (new Service (service));
				}
			}

			return items;
		}
		
		
		/// <summary>
		/// Get full path to do action on service.
		/// </summary>
		/// <param name="name">name of script (service)</param>
		/// <param name="action">action to do</param>
		/// <returns>execute arguments for (gk)sudo</returns>
		public static string GetArgsForService (string name, ServiceActionType action) {
			string actionArgument;
			
			switch (action) {
			case ServiceActionType.Start:
				actionArgument = "start";
				break;
			case ServiceActionType.Stop:
				actionArgument = "stop";
				break;
			case ServiceActionType.Restart:
				actionArgument = "restart";
				break;
			default:
				throw new ArgumentException ("Unsupported Action", "action");
			}
			return Path.Combine (servicesDirectory, name) + " " + actionArgument;
		}

		public static string GetIconForActionType (ServiceActionType action)
		{
			switch (action) {
			case ServiceActionType.Start: return "start";
			case ServiceActionType.Stop: return "stop";
			case ServiceActionType.Restart: return "restart";
			default: return "applications-system";
			}
		}

		
		/// <summary>
		/// Get dictionary of ("service name" -> "selected by user") items.
		/// Search for directory with scripts, load all scripts except scripts in MainBlackList.
		/// Used in configuration panel.
		/// </summary>
		public static IDictionary<string, bool> GetServicesNamesWithStatus () {
			IDictionary<string, bool> result = new SortedDictionary<string, bool> ();

			if (string.IsNullOrEmpty (servicesDirectory)) {
				return result;
			}

			// get all files
			string[] files = Directory.GetFiles (servicesDirectory, "*");			
			foreach (string fileName in files) {
				// without *~
				if (fileName.EndsWith ("~")) {
					continue;
				}

				// only executables
				UnixFileInfo info = new UnixFileInfo (fileName);
				if ((info.FileAccessPermissions & FileAccessPermissions.UserExecute) != FileAccessPermissions.UserExecute) {
					continue;
				}

				result.Add(info.Name, false);
			}

			// remove items defined in main blacklist
			foreach (string globalBlackListItem in MainBlackList) {
				if (result.ContainsKey (globalBlackListItem)) {
					result.Remove (globalBlackListItem);
				}
			}

			// set status services (is service selected by user?)
			foreach (string service in userServices) {
				if (result.ContainsKey (service)) {
					result[service] = true;
				}
			}

			return result;
		}

		
		/// <summary>
		/// Add service do user list and save list.
		/// </summary>
		/// <param name="service">Name of service to add</param>
		public static void AddItemToUserlist(string service) {
			// is already exists?
			foreach (string userService in userServices) {
				if (service == userService) {
					return;
				}
			}
			// add & save
			userServices.Add (service);
			SaveUserList ();
		}

		/// <summary>
		/// Remove given service from user list.
		/// Save list to preferences after remove.
		/// </summary>
		/// <param name="service">Service to remove</param>
		public static void RemoveItemFromUserklist(string service) {
			if (userServices.Remove (service)) {
				SaveUserList ();
			}
		}

		
		/// <summary>
		/// Save user services list to Preferences.
		/// </summary>
		static void SaveUserList() {
			prefs.Set (PrefsServicesKey, string.Join ("\n", userServices.ToArray ())); 
		}
	}
}

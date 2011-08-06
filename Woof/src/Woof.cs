// Woof.cs
//
// Parts of the code were copy/pasted from the Pidgin plugin (in Pidgin.cs).
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
#if USE_DBUS_SHARP
using DBus;
#else
using NDesk.DBus;
#endif
using org.freedesktop.DBus;

using Do.Universe;

namespace Woof
{

	public static class Pidgin {
		const string BUS_NAME = "im.pidgin.purple.PurpleService";
		const string OBJECT_PATH = "/im/pidgin/purple/PurpleObject";
		public const uint PURPLE_CONV_TYPE_IM = 1;
		public const uint PURPLE_CONV_TYPE_CHAT = 2;

		[Interface ("im.pidgin.purple.PurpleInterface")]
			public interface IPidgin {
				int[] PurpleAccountsGetAllActive ();
				int PurpleFindBuddy (int accound_id, string screenname);
				bool PurpleAccountIsConnected (int account_id);
				int PurpleAccountsFindConnected (string account, string proto);
				bool PurpleBuddyIsOnline (int buddy_id);
				int PurpleConversationNew (uint conv_type,
						int account_id,
						string screenname);
				void PurpleConversationPresent (int conversation_id);
				int PurpleConvIm (int conv_id);
				void PurpleConvImSend (int im_id, string message);
			}

		public static IPidgin FindInstance ()
		{
			if(!Bus.Session.NameHasOwner (BUS_NAME)) {
				Bus.Session.StartServiceByName (BUS_NAME);
				System.Threading.Thread.Sleep (5000);
				if(!Bus.Session.NameHasOwner (BUS_NAME))
					throw new Exception (String.Format ("Name {0} has no owner.", BUS_NAME));
			}
			return Bus.Session.GetObject<IPidgin> (BUS_NAME, new ObjectPath (OBJECT_PATH));
		}

		private static int[] ConnectedAccounts {
			get {
				List<int> connected;
				IPidgin prpl;

				prpl = FindInstance ();
				connected = new List<int> ();
				try {
					foreach (int account in prpl.PurpleAccountsGetAllActive ()) {
						if (prpl.PurpleAccountIsConnected (account))
							connected.Add (account);
					}
				} catch { }
				return connected.ToArray ();
			}
		}

		public static bool BuddyIsOnline (string name)
		{
			int account;
			return GetBuddyIsOnlineAndAccount (name, out account);   
		}

		public static bool GetBuddyIsOnlineAndAccount (string name, out int account_out)
		{
			IPidgin prpl;
			int buddy;

			prpl = FindInstance ();
			try {
				foreach (int account in ConnectedAccounts) {
					buddy = prpl.PurpleFindBuddy (account, name);
					if (prpl.PurpleBuddyIsOnline (buddy)) {
						account_out = account;
						return true;
					}
				}
			} catch { }
			account_out = -1;
			return false;
		}

		public static void OpenConversationWithBuddy (string name)
		{
			IPidgin prpl;
			int account, conversation;

			prpl = FindInstance ();
			try {
				GetBuddyIsOnlineAndAccount (name, out account);
				if (account == -1)
					account = prpl.PurpleAccountsFindConnected ("", "");
				conversation = prpl.PurpleConversationNew (PURPLE_CONV_TYPE_IM,
						account, name);
				prpl.PurpleConversationPresent (conversation);
			} catch (Exception e) {
				Console.Error.WriteLine ("Could not create new Pidgin conversation: {0}", e.Message);
			}
		}
	}

	public class WoofServer {
		private string server_url; 
		private string file_path;
		private string screen_name;
		private int timeout;

		public WoofServer (string screen_name)
		{
			this.screen_name = screen_name;
			this.timeout = 60 * 5;
		}

		public string ServerUrl {
			get { return this.server_url; }
		}

		public string ScreenName {
			get { return this.screen_name; }
		}

		public string FilePath {
			get { return this.file_path; }
		}

		public string FileName {
			get {
				// file_path points to a file
				if (Path.GetFileName (this.file_path).Length > 0)
					return Path.GetFileName (this.file_path);

				// file_path points to a directory
				string[] str_parts;
				str_parts = this.file_path.Split (Path.DirectorySeparatorChar);
				return str_parts[str_parts.Length - 1];
			}
		}

		public int Timeout {
			get { return this.timeout; }
		}

		public void SendMessageToBuddy (string message)
		{
			Pidgin.IPidgin prpl;
			int account, conversation, im;

			prpl = Pidgin.FindInstance ();
			try {
				Pidgin.GetBuddyIsOnlineAndAccount (this.screen_name, out account);
				if (account == -1)
					account = prpl.PurpleAccountsFindConnected ("", "");
				conversation = prpl.PurpleConversationNew (Pidgin.PURPLE_CONV_TYPE_IM,
						account, this.screen_name);
				prpl.PurpleConversationPresent (conversation);
				im = prpl.PurpleConvIm (conversation);
				prpl.PurpleConvImSend (im, message);
			} catch (Exception e) {
				Console.Error.WriteLine ("Could not create new Pidgin conversation: {0}", e.Message);
			}
		}

		public void ServeFile(string path)
		{
			this.file_path = path;
			// Read the content of the python file
			Assembly a = Assembly.GetExecutingAssembly ();
			string content = string.Empty;
			using (Stream f = a.GetManifestResourceStream ("Do.Addins.Woof.woof.py")) {
				using (StreamReader sr = new StreamReader (f))
					content = sr.ReadToEnd ();
			}

			// Start process parameters
			System.Diagnostics.Process p = new System.Diagnostics.Process ();
			StreamWriter stdin;
			ProcessStartInfo pinfo = new ProcessStartInfo ("python");
			pinfo.UseShellExecute = false;
			pinfo.RedirectStandardInput = true;
			pinfo.RedirectStandardOutput = true;
			pinfo.EnvironmentVariables.Add ("WOOF_FILE", path);
			p.StartInfo = pinfo;
			p.OutputDataReceived += new DataReceivedEventHandler (WoofOutputHandler);

			// Start the python process
			p.Start ();

			// Redirect stdin
			stdin = p.StandardInput;
			stdin.AutoFlush = true;

			// Watch stdout
			p.BeginOutputReadLine ();

			// Feed stdin
			stdin.Write (content);
			stdin.Close ();

			// Set the timeout
			new Thread ((ThreadStart) delegate {
					Thread.Sleep (this.timeout * 1000);
					if (! p.HasExited) {
					p.Kill ();
					this.SendMessageToBuddy ("Offer canceled.");
					}
					return;
					}).Start ();

			// Wait for the process to exit
			p.WaitForExit ();
		}

		private void WoofOutputHandler (object sendingProcess,
				DataReceivedEventArgs outLine)
		{
			// Print out the sort command output.
			if (!String.IsNullOrEmpty (outLine.Data)) {
				if (outLine.Data.StartsWith ("Now serving on")) {
					this.server_url = outLine.Data.Substring (14);
					this.SendMessageToBuddy (String.Format ("{0}: {1}",
								this.FileName,
								this.server_url));
				}
			}
		}
	}
}

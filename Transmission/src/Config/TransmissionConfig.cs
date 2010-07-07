using System;
using System.IO;
using System.Text;

using Mono.Addins;

using Gtk;

using Do.Platform;

namespace Transmission
{

	[System.ComponentModel.Category("File")]
	[System.ComponentModel.ToolboxItem(true)]
	public partial class TransmissionConfig : Gtk.Bin
	{
		public static string home_path = Environment.GetFolderPath (Environment.SpecialFolder.Personal); 
		public static string settings_path = System.IO.Path.Combine (home_path, ".config/transmission/settings.json");

		static IPreferences prefs;

		public TransmissionConfig()
		{
			Build();
			RefreshView();
		}

		private void RefreshView()
		{
			if (UseRemote)
				use_local_option.Activate();
			else
				use_remote_option.Activate();

			remote_address_entry.Text = RemoteAddress;
			user_name_entry.Text = RemoteUserName;
			password_entry.Text = RemotePassword;

			// Check whether remote control is enabled in Transmission,
			// if it is then hide warning message and disable button
			// used to enable remote control.
			bool control_allowed = IsRemoteControlTurnedOn();
			turn_control_on_button.Sensitive = !control_allowed;
			//remote_control_disabled_warning_label.Visible = !control_allowed;
			remote_control_disabled_warning_label.Sensitive = !control_allowed;
		}

		static TransmissionConfig()
		{
			prefs = Services.Preferences.Get<TransmissionConfig>();
		}

		public static bool UseRemote
		{
			get { return prefs.Get<bool>("UseRemote", false); }
			set { prefs.Set<bool> ("UseRemote", value); }
		}

		public static string RemoteAddress
		{
			get { return prefs.Get<string>("RemoteAddress", "127.0.0.1"); }
			set { prefs.Set<string> ("RemoteAddress", value); }
		}

		public static int RemotePort
		{
			get { return prefs.Get<int>("RemotePort", 9091); }
			set { prefs.Set<int> ("RemotePort", value); }
		}

		public static string RemoteUserName
		{
			get { return prefs.Get<string>("RemoteUserName", ""); }
			set { prefs.Set<string> ("RemoteUserName", value); }
		}

		public static string RemotePassword
		{
			get { return prefs.Get<string>("RemotePassword", ""); }
			set { prefs.Set<string> ("RemotePassword", value); }
		}

		public static Jayrock.Json.JsonObject ReadTransmissionConfig(string path) {
			using (TextReader reader = new StreamReader(path))
				return (Jayrock.Json.JsonObject)Jayrock.Json.Conversion.JsonConvert.Import(reader);
		}

		public static void WriteTransmissionConfig(string path, Jayrock.Json.JsonObject config) {
			using (TextWriter writer = new StreamWriter(path))
				Jayrock.Json.Conversion.JsonConvert.Export(config, writer);
		}

		protected string GenerateRandomString(int length) {
			StringBuilder builder = new StringBuilder();
			Random random = new Random();
			for (int i = 0; i < length; ++i)
			{
				char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
				builder.Append(ch);
			}
			return builder.ToString();
		}

		protected virtual bool IsRemoteControlTurnedOn()
		{
			Jayrock.Json.JsonObject config = ReadTransmissionConfig(settings_path);

			bool rpc_enabled = (bool)config["rpc-enabled"];
			bool rpc_whitelist_enabled = (bool)config["rpc-whitelist-enabled"];
			string comma_separated_ips = (string)config["rpc-whitelist"];
			string[] ips = comma_separated_ips.Split(',');

			return rpc_enabled && (!rpc_whitelist_enabled || Array.IndexOf(ips, "127.0.0.1") >= 0);
		}

		protected virtual void OnRemoteAddressEntryChanged (object sender, System.EventArgs e)
		{
			RemoteAddress = remote_address_entry.Text;
			TransmissionPlugin.ResetConnection();
		}

		protected virtual void OnUserNameEntryChanged (object sender, System.EventArgs e)
		{
			RemoteUserName = user_name_entry.Text;
			TransmissionPlugin.ResetConnection();
		}

		protected virtual void OnPasswordEntryChanged (object sender, System.EventArgs e)
		{
			RemotePassword = password_entry.Text;
			TransmissionPlugin.ResetConnection();
		}

		protected virtual void OnUseRemoteOptionToggled (object sender, System.EventArgs e)
		{
			UseRemote = use_remote_option.Active;
			TransmissionPlugin.ResetConnection();
		}

		protected virtual void OnTurnControlOnButtonClicked (object sender, System.EventArgs e)
		{
			Log<TransmissionConfig>.Info("Turning RPC interface on for local Transmission");

			// FIXME: check whether Transmission is running.
			Jayrock.Json.JsonObject config = ReadTransmissionConfig(settings_path);

			config["rpc-enabled"] = true;
			if ((bool)config["rpc-authentication-required"]) {
				if (! config.Contains("rpc-username") || (string)config["rpc-username"] == "") {
					string username = GenerateRandomString(10);
					Log<TransmissionConfig>.Debug("Generated RPC username: {0}", username);
					config["rpc-username"] = username;
				}
				if (! config.Contains("rpc-password") || (string)config["rpc-password"] == "") {
					string password = GenerateRandomString(10);
					Log<TransmissionConfig>.Debug("Generated RPC password: {0}", password);
					config["rpc-password"] = password;
				}
			}
			if ((bool)config["rpc-whitelist-enabled"]) {
				string comma_separated_ips = (string)config["rpc-whitelist"];
				string[] ips = comma_separated_ips.Split(',');
				if (Array.IndexOf(ips, "127.0.0.1") < 0)
					config["rpc-whitelist"] = comma_separated_ips + ",127.0.0.1";
			}

			WriteTransmissionConfig(settings_path, config);
		}
		
		
	}
}

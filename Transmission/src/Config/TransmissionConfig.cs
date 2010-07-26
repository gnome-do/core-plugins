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
			address_entry.Text = Address;
			port_entry.Text = Port.ToString();
			user_name_entry.Text = UserName;
			password_entry.Text = Password;
		}

		static TransmissionConfig()
		{
			prefs = Services.Preferences.Get<TransmissionConfig>();
		}

		public static string Address
		{
			get { return prefs.Get<string>("Address", "127.0.0.1"); }
			set { prefs.Set<string> ("Address", value); }
		}

		public static int Port
		{
			get { return prefs.Get<int>("Port", TransmissionAPI.DEFAULT_PORT); }
			set { prefs.Set<int> ("Port", value); }
		}

		public static string UserName
		{
			get { return prefs.Get<string>("UserName", ""); }
			set { prefs.Set<string> ("UserName", value); }
		}

		public static string Password
		{
			get { return prefs.Get<string>("Password", ""); }
			set { prefs.Set<string> ("Password", value); }
		}

		protected virtual void OnAddressEntryChanged (object sender, System.EventArgs e)
		{
			Address = address_entry.Text;
			TransmissionPlugin.ResetConnection();
		}

		protected virtual void OnUserNameEntryChanged (object sender, System.EventArgs e)
		{
			UserName = user_name_entry.Text;
			TransmissionPlugin.ResetConnection();
		}

		protected virtual void OnPasswordEntryChanged (object sender, System.EventArgs e)
		{
			Password = password_entry.Text;
			TransmissionPlugin.ResetConnection();
		}

		protected virtual void OnPortEntryChanged (object sender, System.EventArgs e)
		{
			Port = int.Parse(port_entry.Text);
			TransmissionPlugin.ResetConnection();
		}
		
	}
}

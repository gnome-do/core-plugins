/* Configuration.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this source distribution.
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
using System.Threading;
using System.Collections;

using Gnome.Keyring;
using Gtk;
using Gdk;

namespace PingFM
{
	
	
	public partial class Configuration : Gtk.Bin
	{
		
		public Configuration()
		{
			Build();
//			string appkey;
//			appkey = "";
//			GetAccountData (out appkey, GetType());			
			appkey_entry.Text = PingFM.Preferences.AppKey;
		}

		protected virtual void OnApplyBtnClicked (object sender, System.EventArgs e)
		{
			validate_lbl.Markup = "<i>Validating...</i>";
			validate_btn.Sensitive = false;
			
			string appkey = appkey_entry.Text;
			
			Thread thread = new Thread ((ThreadStart) delegate {
				bool valid = PingFM.TryConnect (appkey);
				Gtk.Application.Invoke (delegate {		
					if (valid) {
						validate_lbl.Markup = "<i>Account validation succeeded!</i>";
						SaveAccountData (appkey_entry.Text);
					} else {
						validate_lbl.Markup = "<i>Account validation failed!</i>";
					}
					validate_btn.Sensitive = true;
				});
			});
			thread.IsBackground = true; //don't hang on exit if fail
			thread.Start ();
		}
		
		public static void SaveAccountData (string appkey)
		{
			PingFM.Preferences.AppKey = appkey;
//			string keyName = type.FullName;
//			string keyring;
//			Hashtable ht;
//			
//			try {
//				keyring = Ring.GetDefaultKeyring ();
//				ht = new Hashtable ();
//				ht["name"] = keyName;
//				
//				Ring.CreateItem (keyring, ItemType.GenericSecret, keyName,
//					ht, appkey, true);
//				                 
//			} catch (Exception e) {
//				Console.Error.WriteLine (e.Message);
//			}
		}
//		
//		public static void GetAccountData (out string appkey, Type type)
//		{
//			string keyName = type.FullName;
//			appkey = "";
//			Hashtable ht = new Hashtable ();
//			ht ["name"] = keyName;
//			
//			try {
//				foreach (ItemData s in Ring.Find (ItemType.GenericSecret, ht)) {
//					if (s.Attributes.ContainsKey ("name") && (s.Attributes ["name"] as string).Equals (keyName)) {
//						appkey = s.Secret;
//						return;
//					}
//				}
//			} catch (Exception) {
//				Console.Error.WriteLine ("No account info stored for {0}", keyName);
//			}
//		}
		
		protected virtual void OnAppKeyEntryActivated (object sender, System.EventArgs e)
		{
			validate_btn.Click ();
		}
	
	}
}

//InlineGoogleSearchConfig.cs created with MonoDevelop
//Brian Lucas (bcl1713@gmail.com)
//sacul@irc.ubuntu.com/#gnome-do
// 
//GNOME Do is the legal property of its developers. Please refer to the
//COPYRIGHT file distributed with this
//source distribution.
//
//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.
//
//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.
//


using System;
using Do.Platform;


/// <summary>
/// Do plug-in that returns search results from google back to gnome-do for 
/// further processing
/// </summary>
namespace InlineGoogleSearch {	
	
	/// <summary>
	/// Config Dialog for InlineGoogleSearch
	/// </summary>
	public partial class InlineGoogleSearchConfig : Gtk.Bin {
		/// <summary>
		/// Do.Addins.Util Preferences
		/// </summary>
		static IPreferences prefs;
		
		private const string activess = "active";
		private const string moderatess = "moderate";
		private const string noss = "off"; 

		/// <summary>
		/// Initializes and calls InlineGoogleSearchConfig Widget
		/// </summary>
		public InlineGoogleSearchConfig () 
		{
			this.Build ();
			
			switch (SearchRestrictions) {
			case noss:
				nosafe_rbtn.Active = true; break;
			case moderatess:
				moderate_rbtn.Active = true; break;
			default:
				strict_rbtn.Active = true; break;
			}
			
			returnResults_rbtn.Active = ReturnResults;
			goToSearch_rbtn.Active = !ReturnResults;
			
			showFirstCheck.Active = ShowSearchFirst;
			applySSL.Active = InheritSSL;
			
			updateSensitivities();
		}
		
		static InlineGoogleSearchConfig () 
		{
			prefs = Do.Platform.Services.Preferences.Get<InlineGoogleSearchConfig>();
		}

		public static string SearchRestrictions {
			get { 
				return prefs.Get<string> ("SearchRestrictions", 
				                          moderatess); 
			}
		 	set { prefs.Set<string> ("SearchRestrictions", value); }
		}
		
		public static bool ShowSearchFirst {
			get {
				return prefs.Get<bool> ("ShowSearchFirst", false);
			}
			set {prefs.Set<bool> ("ShowSearchFirst", value); }
		}

		public static bool InheritSSL {
			get {
				return prefs.Get<bool> ("InheritSSL", false);
			}
			set {prefs.Set<bool> ("InheritSSL", value);}
		}
		
		public static bool ReturnResults {
			get {
				return prefs.Get<bool> ("ReturnResults", true);
			}
			set {prefs.Set<bool> ("ReturnResults", value);}
		}
		
		protected virtual void OnNosafeRbtnToggled (object sender, 
		                                            System.EventArgs e) 
		{
			prefs.Set("SearchRestrictions",noss);			
		}

		
		protected virtual void OnModerateRbtnToggled (object sender, 
		                                              System.EventArgs 
		                                              e) 
		{
			prefs.Set("SearchRestrictions",moderatess);
		}

		protected virtual void OnStrictRbtnToggled (object sender, 
		                                            System.EventArgs e) 
		{
			prefs.Set("SearchRestrictions",activess);
		}

		protected virtual void OnShowFirstCheckClicked (object sender, System.EventArgs e)
		{
			prefs.Set("ShowSearchFirst", showFirstCheck.Active);
			updateSensitivities();
		}

		protected virtual void OnApplySSLClicked (object sender, System.EventArgs e)
		{
			prefs.Set("InheritSSL", applySSL.Active);
			updateSensitivities();
		}
		
		protected virtual void OnReturnResultsRbtnToggled (object sender, System.EventArgs e)
		{
			prefs.Set("ReturnResults", true);
			updateSensitivities();
		}

		protected virtual void OnGoToSearchRbtnToggled (object sender, System.EventArgs e)
		{
			prefs.Set("ReturnResults", false);
			updateSensitivities();
		}

		private void updateSensitivities() {
			showFirstCheck.Sensitive = ReturnResults;
			safeSearchBox.Sensitive = ReturnResults || InheritSSL;
		}
		
	}
}
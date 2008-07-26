// Permission is hereby granted, free of charge, to any person obtaining 
// a copy of this software and associated documentation files (the 
// "Software"), to deal in the Software without restriction, including 
// without limitation the rights to use, copy, modify, merge, publish, 
// distribute, sublicense, and/or sell copies of the Software, and to 
// permit persons to whom the Software is furnished to do so, subject to 
// the following conditions: 
//  
// The above copyright notice and this permission notice shall be 
// included in all copies or substantial portions of the Software. 
//  
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// 
// Copyright (c) 2008 Novell, Inc. (http://www.novell.com) 
// 
// Authors: 
//      Sandy Armstrong <sanfordarmstrong@gmail.com>
// 


using System;

using Do.Addins;

namespace Tomboy
{
	
	
	public partial class TomboyConfiguration : Gtk.Bin
	{
		private const string DeriveTitlePrefKey = "deriveTitle";
		private const string TitleFirstPrefKey = "titleFirst";
		
		private static IPreferences prefs;
		
		static TomboyConfiguration ()
		{
			prefs = Do.Addins.Util.GetPreferences ("Tomboy");
		}
		
		public static bool DeriveTitle
		{
			get {
				return prefs.Get (DeriveTitlePrefKey, false);
			}
		}
		
		public static bool TitleFirst
		{
			get {
				return prefs.Get (TitleFirstPrefKey, false);
			}
		}
		
		public TomboyConfiguration()
		{
			this.Build();
			
			// Initialize fields with values from prefs.
			deriveTitleCheckButton.Active = DeriveTitle;
			contentFirstRadioButton.Active = !TitleFirst;
			titleFirstRadioButton.Active = TitleFirst;
			
			// Set up events.  I had trouble doing this from stetic.
			deriveTitleCheckButton.Toggled += OnDeriveTitleCheckButtonToggled;
			contentFirstRadioButton.Toggled += OnContentFirstRadioButtonToggled;
			titleFirstRadioButton.Toggled += OnContentFirstRadioButtonToggled;
		}

		protected virtual void OnDeriveTitleCheckButtonToggled (object sender, System.EventArgs e)
		{
			prefs.Set (DeriveTitlePrefKey, deriveTitleCheckButton.Active);
		}

		protected virtual void OnContentFirstRadioButtonToggled (object sender, System.EventArgs e)
		{
			prefs.Set (TitleFirstPrefKey, titleFirstRadioButton.Active);
		}
	}
}

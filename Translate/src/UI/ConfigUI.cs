// ConfigUI.cs
// 
// Copyright (C) 2008 Chris Szikszoy
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
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using Do.Platform;

using Mono.Unix;

namespace Translate
{
	
	public partial class ConfigUI : Gtk.Bin
	{
		static IPreferences TranslatePluginPrefs;
		
		public ConfigUI ()
		{
			this.Build ();
			SetupColumns ();
			FillProviderCmb ();	
		}
		static ConfigUI ()
		{
			TranslatePluginPrefs = Services.Preferences.Get<Translate.ConfigUI> ();
		}
		
		public void SetupColumns ()
		{		
			//setup columns
			Gtk.TreeViewColumn FlagCol = new Gtk.TreeViewColumn ();
			Gtk.CellRendererPixbuf pixbuf = new Gtk.CellRendererPixbuf ();	
			FlagCol.PackStart (pixbuf, false);
			treeEnableLang.AppendColumn (FlagCol);
			FlagCol.AddAttribute (pixbuf, "pixbuf", 0);
			
			Gtk.TreeViewColumn LanguageCol = new Gtk.TreeViewColumn ();
			Gtk.CellRendererText text = new Gtk.CellRendererText ();
			LanguageCol.PackStart (text, true);
			treeEnableLang.AppendColumn (LanguageCol);
			LanguageCol.AddAttribute (text, "text", 1);
			
			/*
			Gtk.TreeViewColumn IsEnabled = new Gtk.TreeViewColumn ();
			Gtk.CellRendererToggle EnabledToggle = new Gtk.CellRendererToggle ();
			EnabledToggle.Activatable = true;
			EnabledToggle.Toggled += CheckEnable;
			IsEnabled.PackStart (EnabledToggle, false);
			treeEnableLang.AppendColumn (IsEnabled);
			IsEnabled.AddAttribute (EnabledToggle, "active", 2);
			
			//add a dummy column to fill the rest
			Gtk.TreeViewColumn DummyCol = new Gtk.TreeViewColumn ();
			Gtk.CellRendererText DummyText = new Gtk.CellRendererText ();
			DummyCol.PackStart (DummyText, true);
			treeEnableLang.AppendColumn (DummyCol);
			DummyCol.AddAttribute (DummyText, "text", 3);
			*/
		}
		public void FillProviderCmb ()
		{		
			cmbProvider.Clear ();
			
	        Gtk.CellRendererText cell = new Gtk.CellRendererText ();
			Gtk.CellRendererPixbuf pixbuf = new Gtk.CellRendererPixbuf ();
			cmbProvider.PackStart (pixbuf, true);
			cmbProvider.PackStart (cell, true);
			cmbProvider.AddAttribute (pixbuf, "pixbuf" , 0);
	        cmbProvider.AddAttribute (cell, "text", 1);	
			
			Gtk.ListStore ProvidersList = new Gtk.ListStore (typeof (Gdk.Pixbuf), typeof (string));
			
			// Get an instance of each pastebin provider in this assembly.
			var providers = from type in Assembly.GetExecutingAssembly ().GetTypes ()
					where type.GetInterface ("Translate.ITranslateProvider") != null
					select Activator.CreateInstance (type);
			
			Gdk.Pixbuf pb = null;
			string[] Icon = null;
			foreach (ITranslateProvider provider in providers) {
				Icon = provider.Icon.Split (new char[] {'@'});
				using (Gdk.Pixbuf temp = Gdk.Pixbuf.LoadFromResource(Icon[0])) {
					pb = temp.ScaleSimple ((20 * temp.Width)/temp.Height, 20, Gdk.InterpType.Bilinear);
				}		    
		        ProvidersList.AppendValues (pb, provider.Name);
			}
			cmbProvider.Model = ProvidersList;
			//set selection to what's in GConf, if we can
			Gtk.TreeIter ti;
			if (SearchCombobox (out ti, cmbProvider, SelectedProvider,1))
				cmbProvider.SetActiveIter (ti);
		}
		
		public void FillSourceCmb (ITranslateProvider Translator)
		{
			cmbDefaultSource.Clear ();
			
	        Gtk.CellRendererText cell = new Gtk.CellRendererText ();
			Gtk.CellRendererPixbuf pixbuf = new Gtk.CellRendererPixbuf ();
			cmbDefaultSource.PackStart( pixbuf, true);
			cmbDefaultSource.PackStart (cell, true);
			cmbDefaultSource.AddAttribute (pixbuf, "pixbuf" , 0);
	        cmbDefaultSource.AddAttribute (cell, "text", 1);	
			
			
			Gtk.ListStore SourceList = new Gtk.ListStore (typeof (Gdk.Pixbuf), typeof (string), typeof (string));

			Gdk.Pixbuf LanguageFlag = null;
			const int scale_height = 20;
			string[] Icon = null;
			if (Translator.SupportsAutoDetect)
				SourceList.AppendValues (null, Catalog.GetString ("Auto Detect (Recommended)"), Translator.AutoDetectCode);
			foreach (LanguageItem Lang in Translator.SupportedLanguages)
			{
				Icon = Lang.Icon.Split (new char[] {'@'});
				using (Gdk.Pixbuf temp = Gdk.Pixbuf.LoadFromResource (Icon[0])) {
					LanguageFlag = temp.ScaleSimple ((scale_height * temp.Width) / temp.Height, scale_height, Gdk.InterpType.Bilinear);
				}
				SourceList.AppendValues (LanguageFlag, Lang.Name, Lang.Code);
			}

			cmbDefaultSource.Model = SourceList;
			//set default source to auto if it's enabled, else try what's in gconf, else, just the first in the list
			Gtk.TreeIter ti;
			if ((Translator.SupportsAutoDetect) && (SearchCombobox (out ti, cmbDefaultSource, Translator.AutoDetectCode, 2)))
				cmbDefaultSource.SetActiveIter (ti);
			else if (SearchCombobox(out ti, cmbDefaultSource, SelectedSourceLang, 2))
				cmbDefaultSource.SetActiveIter (ti);
			else {
				cmbDefaultSource.Model.GetIterFirst (out ti);
				cmbDefaultSource.SetActiveIter (ti);
			}
		}
		
		private void FillIfaceCmb (ITranslateProvider Translator)
		{
			cmbDefaultIface.Clear ();
			
	        Gtk.CellRendererText cell = new Gtk.CellRendererText ();
			Gtk.CellRendererPixbuf pixbuf = new Gtk.CellRendererPixbuf ();
			cmbDefaultIface.PackStart (pixbuf, true);
			cmbDefaultIface.PackStart (cell, true);
			cmbDefaultIface.AddAttribute (pixbuf, "pixbuf" , 0);
	        cmbDefaultIface.AddAttribute (cell, "text", 1);	
			
			Gtk.ListStore IfaceList = new Gtk.ListStore (typeof (Gdk.Pixbuf), typeof (string), typeof (string));

			Gdk.Pixbuf LanguageFlag = null;
			const int scale_height = 20;
			string[] Icon = null;
			foreach (LanguageItem Lang in Translator.SupportedLanguages) {
				Icon = Lang.Icon.Split(new char[] {'@'});
				using (Gdk.Pixbuf temp = Gdk.Pixbuf.LoadFromResource(Icon[0])) {
					LanguageFlag = temp.ScaleSimple ((scale_height * temp.Width) / temp.Height, scale_height, Gdk.InterpType.Bilinear);
				}
				IfaceList.AppendValues (LanguageFlag, Lang.Name, Lang.Code);
			}

			cmbDefaultIface.Model = IfaceList;
			//set selection to what's in GConf, if we can
			Gtk.TreeIter ti;
			if (SearchCombobox (out ti, cmbDefaultIface, SelectedIfaceLang, 2))
				cmbDefaultIface.SetActiveIter(ti);
			else {
				cmbDefaultIface.Model.GetIterFirst (out ti);
				cmbDefaultIface.SetActiveIter (ti);
			}
		}
		
		private void FillEnableBox(ITranslateProvider Translator)
		{
			Gtk.ListStore LanguageList = new Gtk.ListStore (typeof (Gdk.Pixbuf), typeof (string));
			const int scale_height = 25;
			Gdk.Pixbuf LanguageFlag = null;
			string[] Icon = null;
			
			foreach (LanguageItem Lang in Translator.SupportedLanguages) {
				Icon = Lang.Icon.Split (new char[] {'@'});
				using (Gdk.Pixbuf temp = Gdk.Pixbuf.LoadFromResource(Icon[0])) {
					LanguageFlag = temp.ScaleSimple ((scale_height * temp.Width) / temp.Height, scale_height, Gdk.InterpType.Bilinear);
				}
				LanguageList.AppendValues (LanguageFlag, Lang.Name);
			}

			treeEnableLang.HeadersVisible = false;
			treeEnableLang.Model = LanguageList;
		}

		public bool SearchCombobox (out Gtk.TreeIter ti, Gtk.ComboBox box, string val, int col)
		{
			box.Model.GetIterFirst (out ti);
			do {
				if ((string)box.Model.GetValue (ti,col) == val)
					return true;
			} while (box.Model.IterNext (ref ti));
			return false;
		}

		protected virtual void ProviderSelectionChanged (object sender, System.EventArgs e)
		{
			Gtk.ComboBox ProviderBox = (Gtk.ComboBox)sender;
			Gtk.TreeIter ti;
			ProviderBox.GetActiveIter (out ti);
			SelectedProvider = (string)ProviderBox.Model.GetValue (ti,1);
			TranslateEngine.LoadValuesFromPrefs();
			ITranslateProvider Translator = TranslateEngine.Translator;
			
			//fill boxes
			FillSourceCmb (Translator);
			if (Translator.SupportsIfaceLang)
				FillIfaceCmb (Translator);
			else
			{
				cmbDefaultIface.Clear ();
				cmbDefaultIface.Model = null;
			}
			FillEnableBox (Translator);
		}

		protected virtual void SourceSelectionChanged (object sender, System.EventArgs e)
		{
			Gtk.ComboBox SourceBox = (Gtk.ComboBox)sender;
			Gtk.TreeIter ti;
			SourceBox.GetActiveIter (out ti);
			//column 2 holds the language "code"
			SelectedSourceLang = (string)SourceBox.Model.GetValue (ti,2);
		}

		protected virtual void IfaceSelectionChanged (object sender, System.EventArgs e)
		{
			Gtk.ComboBox IfaceBox = (Gtk.ComboBox)sender;
			Gtk.TreeIter ti;
			IfaceBox.GetActiveIter(out ti);
			//column 2 holds the language "code"
			SelectedIfaceLang = (string)IfaceBox.Model.GetValue (ti,2);
		}
		
		public static string SelectedProvider
		{
			get { 
				ITranslateProvider DefaultProvider = new Google ();
				return TranslatePluginPrefs.Get<string> ("SelectedProvider", DefaultProvider.Name);
			}
			set { TranslatePluginPrefs.Set<string> ("SelectedProvider", value); }
		}
		public static string SelectedSourceLang
		{
			get { 
				ITranslateProvider Translator = TranslateProviderFactory.GetProviderFromPreferences ();				
				return TranslatePluginPrefs.Get<string> ("SelectedSourceLanguage", Translator.DefaultSourceCode); 
			}
			set { TranslatePluginPrefs.Set<string> ("SelectedSourceLanguage", value); }
		}
		public static string SelectedIfaceLang
		{
			get { 
				ITranslateProvider Translator = TranslateProviderFactory.GetProviderFromPreferences ();
				return TranslatePluginPrefs.Get<string> ("SelectedIfaceLanguage", Translator.DefaultIfaceCode); 
			}
			set { TranslatePluginPrefs.Set<string> ("SelectedIfaceLanguage", value); }
		}
	}
}

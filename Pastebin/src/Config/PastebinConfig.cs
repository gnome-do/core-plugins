//  PastebinConfig.cs
//
//  GNOME Do is the legal property of its developers, whose names are too
//  numerous to list here.  Please refer to the COPYRIGHT file distributed with
//  this source distribution.
//
//  This program is free software: you can redistribute it and/or modify it
//  under the terms of the GNU General Public License as published by the Free
//  Software Foundation, either version 3 of the License, or (at your option)
//  any later version.
//
//  This program is distributed in the hope that it will be useful, but WITHOUT
//  ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
//  FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
//  more details.
//
//  You should have received a copy of the GNU General Public License along with
//  this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Do.Platform;

namespace Pastebin
{	
	public partial class PastebinConfig : Gtk.Bin
	{	
		static IPreferences prefs;
		
		public PastebinConfig ()
		{
			this.Build();
			SetupColumns();
			FillProviders();
		}	
		
		private void SetupColumns()
		{		
			//setup columns
			/*
			Gtk.TreeViewColumn IconCol = new Gtk.TreeViewColumn();
			Gtk.CellRendererPixbuf pixbuf = new Gtk.CellRendererPixbuf ();	
			IconCol.PackStart(pixbuf, false);
			treeCodes.AppendColumn(IconCol);
			IconCol.AddAttribute(pixbuf, "pixbuf", 0);
			*/
			
			Gtk.TreeViewColumn CodeCol = new Gtk.TreeViewColumn();
			Gtk.CellRendererText text = new Gtk.CellRendererText();
			CodeCol.PackStart(text, true);
			treeCodes.AppendColumn(CodeCol);
			CodeCol.AddAttribute(text, "text", 0);
		}		
		
		public static string SelectedProviderType {
			get { return prefs.Get<string> ("SelectedProviderType", typeof(Paste2).ToString()); }
			set { prefs.Set<string> ("SelectedProviderType", value); }
		}
		
		static PastebinConfig ()
		{
			prefs = Services.Preferences.Get<PastebinConfig> ();
		}
		
		private void FillProviders ()
		{ 
			cmbProvider.Clear ();
			
	        	Gtk.CellRendererText cell = new Gtk.CellRendererText ();
			cmbProvider.PackStart (cell, true);
			cmbProvider.AddAttribute (cell, "text" , 0);
			
			Gtk.ListStore ProvidersList = new Gtk.ListStore (typeof (string), typeof (string));

			// Get an instance of each pastebin provider in this assembly.
			var providers =
				from type in Assembly.GetExecutingAssembly ().GetTypes ()
				where type.GetInterface ("Pastebin.IPastebinProvider") != null && type.IsAbstract == false
				select Activator.CreateInstance (type);

			foreach (IPastebinProvider provider in providers) {
				ProvidersList.AppendValues (provider.Name, provider.GetType ().ToString ());
			}
			
			cmbProvider.Model = ProvidersList;
			//set selection to what's in GConf, if we can
			Gtk.TreeIter ti;
			if (SearchCombobox(out ti, cmbProvider, SelectedProviderType, 1))
				cmbProvider.SetActiveIter(ti);
		}
		
		public bool SearchCombobox (out Gtk.TreeIter ti, Gtk.ComboBox box, string val, int col)
		{		
			box.Model.GetIterFirst (out ti);
			do 
			{
				if ((string)box.Model.GetValue (ti,col) == val)
					return true;
			} while (box.Model.IterNext (ref ti));
			//haven't found it
			return false;
		}
		
		private void FillSyntaxBox (IPastebinProvider paster)
		{
			//Gtk.ListStore CodeList = new Gtk.ListStore(typeof (Gdk.Pixbuf), typeof (string));
			Gtk.ListStore CodeList = new Gtk.ListStore(typeof (string));
			int count = 1;
			//const int scale_height = 25;
			//Gdk.Pixbuf syntax_icon = null;
			//Gdk.Pixbuf temp = null;
			//string[] Icon = null;

			foreach (TextSyntaxItem syntax in paster.SupportedLanguages)
			{
				/*
				//first determine if this icon is built in - or - comes from resource
				Icon = syntax.Icon.Split (new char[] {'@'});
				if (Icon.Length > 1)
				{
					temp = Gdk.Pixbuf.LoadFromResource (Icon[0]);
					syntax_icon = temp.ScaleSimple ((scale_height * temp.Width) / temp.Height, scale_height, Gdk.InterpType.Bilinear);
				}
				else
				{
					//this section is commented because setting
					//syntax_icon = null throws a GLIB exception
					syntax_icon = null;
				}
				//CodeList.AppendValues (syntax_icon, syntax.Name);
				*/
				CodeList.AppendValues (syntax.Name);
				count++;
			}
			//get rid of our temporary pixbuf
			//temp.Dispose();

			treeCodes.HeadersVisible = false;
			treeCodes.Model = CodeList;
			
			lblCodes.Text = string.Format ("{0} syntax highlighting modes supported.", count);
		}		

		protected virtual void cmbProviderChanged (object sender, System.EventArgs e)
		{
			Gtk.ComboBox ProviderBox = (Gtk.ComboBox)sender;
			Gtk.TreeIter ti;
			ProviderBox.GetActiveIter (out ti);
			SelectedProviderType = (string)ProviderBox.Model.GetValue (ti,1);
			IPastebinProvider Pastebin = PastebinProviderFactory.GetProviderFromPreferences();
			
			//fill treeview with supported codes
			FillSyntaxBox (Pastebin);				
		}
	}
}

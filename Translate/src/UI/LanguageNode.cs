// LanguageNode.cs
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
using Mono.Unix;
using Do.Platform;

namespace Translate
{	
	public class LanguageNode
	{
		static IPreferences TranslatePluginPrefs;
		
		Gdk.Pixbuf icon;
		string name;
		string code;
		
		public LanguageNode (Gdk.Pixbuf icon, string name, string code)
		{
			TranslatePluginPrefs = Services.Preferences.Get<Translate.ConfigUI> ();
			this.icon = icon;
			this.name = name;
			this.code = code;
		}

		public Gdk.Pixbuf Icon
		{
			get { return icon; }
		}

		public string Name
		{
			get { return Catalog.GetString (name); }
		}

		public bool IsEnabled
		{
			get { return (bool)TranslatePluginPrefs.Get<bool> (this.code, true); }
			set { TranslatePluginPrefs.Set<bool> (this.code, value); }
		}
	}
}

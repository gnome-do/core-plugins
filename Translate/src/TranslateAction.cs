// TranslateAction.cs
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
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Mono.Addins;

using Do.Universe;
using Do.Universe.Common;
using Do.Platform;
using Do.Platform.Linux;

namespace Translate
{

	public class TranslateAction : Act, IConfigurable
	{

		const string UrlPattern = "^(https?://)"
	        + "?(([0-9a-zA-Z_!~*'().&=+$%-]+: )?[0-9a-zA-Z_!~*'().&=+$%-]+@)?" //user@
	        + @"(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP- 199.194.52.184
	        + "|" // allows either IP or domain
	        + @"([0-9a-zA-Z_!~*'()-]+\.)*" // tertiary domain(s)- www.
	        + @"([0-9a-zA-Z][0-9a-zA-Z-]{0,61})?[0-9a-zA-Z]\." // second level domain
	        + "[a-zA-Z]{2,6})" // first level domain- .com or .museum
	        + "(:[0-9]{1,4})?" // port number- :80
	        + "((/?)|" // a slash isn't required if there is no file name
	        + "(/[0-9a-zA-Z_!~*'().;?:@&=+$,%#-]+)+/?) *$";

		readonly Regex url_regex;

		public TranslateAction ()
		{
			url_regex = new Regex (UrlPattern, RegexOptions.Compiled);
		}

		public override string Name {
			get { return AddinManager.CurrentLocalizer.GetString ("Translate"); }
		}

		public override string Icon {
			get { return "globe.png@" + GetType ().Assembly.FullName; }
		}

		public override string Description {
			get { return AddinManager.CurrentLocalizer.GetString ("Translates text"); }
		}

		public override IEnumerable<Type> SupportedItemTypes {
			get {
				yield return typeof (TextItem);
				yield return typeof (IUrlItem);
				yield return typeof (IFileItem);
			}
		}
		
		public override bool SupportsItem (Item item)
		{
			ITranslateProvider Translator = TranslateEngine.Translator;

			if (item is IFileItem) {
				IFileItem file = item as IFileItem;
				if (Directory.Exists (file.Path)) return false;
				long kbSize = new FileInfo (file.Path).Length / 1024;
				return kbSize < 100;
			}
			if (item is ITextItem) {
				if (!Translator.SupportsUrlTranslate && url_regex.IsMatch ((item as ITextItem).Text))
					return false;
			}
			if (item is IUrlItem && !Translator.SupportsUrlTranslate)
				return false;
			return true;
		}

		public override IEnumerable<Type> SupportedModifierItemTypes {
			get { yield return typeof (LanguageItem); }
		}

		public override IEnumerable<Item> DynamicModifierItemsForItem (Item item)
		{
			ITranslateProvider Translator = TranslateEngine.Translator;
			foreach (LanguageItem Language in Translator.SupportedLanguages)
				yield return Language;
		}

		public override IEnumerable<Item> Perform (IEnumerable<Item> items, IEnumerable<Item> modItems)
		{
			ITranslateProvider Translator = TranslateEngine.Translator;
			string url = null;
			LanguageItem ToLang = (modItems.First () as LanguageItem);

			foreach (Item i in items) {
				if (i is ITextItem) {
					if (Translator.SupportsUrlTranslate && url_regex.IsMatch ((i as ITextItem).Text))
					    url = Translator.BuildUrlRequestUrl (ConfigUI.SelectedIfaceLang, ToLang.Code, ConfigUI.SelectedSourceLang, (i as ITextItem).Text);
					else
					    url = Translator.BuildTextRequestUrl (ConfigUI.SelectedIfaceLang, ToLang.Code, ConfigUI.SelectedSourceLang, (i as ITextItem).Text);
				}
				if (i is IUrlItem)
					url = Translator.BuildUrlRequestUrl (ConfigUI.SelectedIfaceLang, ToLang.Code, ConfigUI.SelectedSourceLang, (i as IUrlItem).Url);
				if (i is IFileItem)
					url = Translator.BuildTextRequestUrl (ConfigUI.SelectedIfaceLang, ToLang.Code, ConfigUI.SelectedSourceLang, File.ReadAllText ((i as IFileItem).Path));
				
				if (!string.IsNullOrEmpty (url))
					Services.Environment.OpenUrl (url);
			}
			yield break;
		}

		public Gtk.Bin GetConfiguration ()
		{
			return new ConfigUI ();
		}	
	}
}
// Google.cs
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
using System.Web;
using System.Text;
using System.Collections.Generic;
using Mono.Unix;

namespace Translate
{
	
	public class Google : ITranslateProvider
	{		
		IEnumerable<LanguageItem> supported_langauges;
		
		public Google ()
		{			
			supported_langauges = new [] {
				new LanguageItem (Catalog.GetString ("Arabic"),
												Catalog.GetString ("Translate to Arabic"),
												"ar",
												"arabic"),
				new LanguageItem (Catalog.GetString ("Bulgarian"),
												Catalog.GetString ("Translate to Bulgarian"),
												"bg",
												"bulgarian"),
				new LanguageItem (Catalog.GetString ("Catalon"),
												Catalog.GetString ("Translate to Catalon"),
												"ca",
												"catalan"),
				new LanguageItem (Catalog.GetString ("Chinese (Simplified)"),
												Catalog.GetString ("Translate to Chinese (Simplified)"),
												"zh-CN",
												"chinese-simp"),
				new LanguageItem (Catalog.GetString ("Chinese (Traditional)"),
												Catalog.GetString ("Translate to Chinese (Traditional)"),
												"zh-TW",
												"chinese-trad"),
				new LanguageItem (Catalog.GetString ("Croatian"),
												Catalog.GetString ("Translate to Croatian"),
												"hr",
												"croatian"),
				new LanguageItem (Catalog.GetString ("Czech"),
												Catalog.GetString ("Translate to Czech"),
												"cs",
												"czech"),
				new LanguageItem (Catalog.GetString ("Danish"),
												Catalog.GetString ("Translate to Danish"),
												"da",
												"danish"),
				new LanguageItem (Catalog.GetString ("Dutch"),
												Catalog.GetString ("Translate to Dutch"),
												"nl",
												"dutch"),
				new LanguageItem (Catalog.GetString ("English"),
												Catalog.GetString ("Translate to English"),
												"en",
												"english"),
				new LanguageItem (Catalog.GetString ("Filipino"),
												Catalog.GetString ("Translate to Filipino"),
												"tl",
												"filipino"),
				new LanguageItem (Catalog.GetString ("Finnish"),
												Catalog.GetString ("Translate to Finnish"),
												"fi",
												"finnish"),
				new LanguageItem (Catalog.GetString ("French"),
												Catalog.GetString ("Translate to French"),
												"fr",
												"french"),
				new LanguageItem (Catalog.GetString ("German"),
												Catalog.GetString ("Translate to German"),
												"de",
												"german"),
				new LanguageItem (Catalog.GetString ("Greek"),
												Catalog.GetString ("Translate to Greek"),
												"el",
												"greek"),
				new LanguageItem (Catalog.GetString ("Hebrew"),
												Catalog.GetString ("Translate to Hebrew"),
												"iw",
												"hebrew"),
				new LanguageItem (Catalog.GetString ("Hindi"),
												Catalog.GetString ("Translate to Hindi"),
												"hi",
												"hindi"),
				new LanguageItem (Catalog.GetString ("Indonesian"),
												Catalog.GetString ("Translate to Indonesian"),
												"id",
												"indonesian"),
				new LanguageItem (Catalog.GetString ("Italian"),
												Catalog.GetString ("Translate to Italian"),
												"it",
												"italian"),
				new LanguageItem (Catalog.GetString ("Japanese"),
												Catalog.GetString ("Translate to Japanese"),
												"ja",
												"japanese"),
				new LanguageItem (Catalog.GetString ("Korean"),
												Catalog.GetString ("Translate to Korean"),
												"ko",
												"korean"),
				new LanguageItem (Catalog.GetString ("Latvian"),
												Catalog.GetString ("Translate to Latvian"),
												"lv",
												"latvian"),
				new LanguageItem (Catalog.GetString ("Lithuanian"),
												Catalog.GetString ("Translate to Lithuanian"),
												"lt",
												"lithuanian"),
				new LanguageItem (Catalog.GetString ("Norwegian"),
												Catalog.GetString ("Translate to Norwegian"),
												"no",
												"norwegian"),
				new LanguageItem (Catalog.GetString ("Polish"),
												Catalog.GetString ("Translate to Polish"),
												"pl",
												"polish"),
				new LanguageItem (Catalog.GetString ("Portuguese"),
												Catalog.GetString ("Translate to Portuguese"),
												"pt",
												"portuguese"),
				new LanguageItem (Catalog.GetString ("Romanian"),
												Catalog.GetString ("Translate to Romanian"),
												"ro",
												"romanian"),
				new LanguageItem (Catalog.GetString ("Russian"),
												Catalog.GetString ("Translate to Russian"),
												"ru",
												"russian"),
				new LanguageItem (Catalog.GetString ("Serbian"),
												Catalog.GetString ("Translate to Serbian"),
												"sr",
												"serbian"),
				new LanguageItem (Catalog.GetString ("Slovak"),
												Catalog.GetString ("Translate to Slovak"),
												"sk",
												"slovak"),
				new LanguageItem (Catalog.GetString ("Slovenian"),
												Catalog.GetString ("Translate to Slovenian"),
												"sl",
												"slovenian"),
				new LanguageItem (Catalog.GetString ("Spanish"),
												Catalog.GetString ("Translate to Spanish"),
												"es",
												"spanish"),
				new LanguageItem (Catalog.GetString ("Swedish"),
												Catalog.GetString ("Translate to Swedish"),
												"sv",
												"swedish"),
				new LanguageItem (Catalog.GetString ("Ukranian"),
												Catalog.GetString ("Translate to Ukranian"),
												"uk",
												"ukranian"),
				new LanguageItem (Catalog.GetString ("Vietnamese"),
												Catalog.GetString ("Translate to Vietnamese"),
												"vi",
												"vietnamese"),
			};
		}
		
		public string Name {
			get { return "Google"; }
		}
		
		public string Icon {
			get { return "google.png@" + GetType ().Assembly.FullName; }
		}
		
		public string BuildTextRequestUrl (string ifaceLang, string toLang, string fromLang, string req)
		{
			string options_begin = "#";
			string options_separator = "|";
			string options_end = "";
			
			string request_url = "http://translate.google.com/translate_t";
			
			request_url += "?hl=" + ifaceLang;
			request_url += options_begin;
			request_url += fromLang;
			request_url += options_separator;
			request_url += toLang;
			request_url += options_separator;
			request_url += req;
			request_url += options_end;
			return request_url;
		}
		
		public string BuildUrlRequestUrl (string ifaceLang, string toLang, string fromLang, string req)
		{
			return "http://translate.google.com/translate?u=" +
				HttpUtility.UrlEncode (req) +
				"&hl=" + ifaceLang +
				"&ie=UTF-8" +
				"&sl=" + fromLang +
				"&tl=" + toLang;
		}
		
		public IEnumerable<LanguageItem> SupportedLanguages {
			get { return supported_langauges; }
		}
		
		public string DefaultSourceCode {
			get { return AutoDetectCode; }
		}
		
		public bool SupportsAutoDetect {
			get { return true; }
		}
		
		public string AutoDetectCode {
			get { return "auto"; }
		}
		
		public bool SupportsUrlTranslate {
			get { return true; }
		}
		
		public bool SupportsIfaceLang {
			get { return true; }
		}
		
		public string DefaultIfaceCode {
			get { return "en"; }
		}

	}
}

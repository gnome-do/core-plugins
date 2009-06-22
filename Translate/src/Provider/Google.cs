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
using Mono.Addins;

namespace Translate
{
	
	public class Google : ITranslateProvider
	{		
		IEnumerable<LanguageItem> supported_langauges;
		
		public Google ()
		{			
			supported_langauges = new [] {
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Albanian"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Albanian"), "sq", "albanian"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Arabic"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Arabic"), "ar", "arabic"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Bulgarian"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Bulgarian"), "bg", "bulgarian"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Catalon"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Catalon"),	"ca", "catalan"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Chinese"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Chinese"), "zh-CN", "chinese-simp"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Croatian"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Croatian"), "hr", "croatian"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Czech"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Czech"), "cs", "czech"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Danish"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Danish"), "da", "danish"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Dutch"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Dutch"), "nl", "dutch"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("English"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to English"), "en", "english"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Estonian"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Estonian"), "et", "estonian"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Filipino"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Filipino"),  "tl", "filipino"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Finnish"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Finnish"), "fi", "finnish"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("French"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to French"), "fr", "french"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Galician"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Galician"), "gl", "galician"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("German"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to German"), "de", "german"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Greek"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Greek"), "el", "greek"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Hebrew"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Hebrew"), "iw", "hebrew"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Hindi"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Hindi"), "hi", "hindi"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Hungarian"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Hungarian"), "hu", "hungarian"),				
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Indonesian"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Indonesian"), "id", "indonesian"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Italian"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Italian"), "it", "italian"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Japanese"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Japanese"), "ja", "japanese"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Korean"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Korean"), "ko", "korean"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Latvian"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Latvian"), "lv", "latvian"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Lithuanian"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Lithuanian"), "lt", "lithuanian"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Maltese"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Maltese"), "mt", "maltese"),				
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Norwegian"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Norwegian"), "no", "norwegian"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Polish"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Polish"), "pl", "polish"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Portuguese"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Portuguese"), "pt", "portuguese"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Romanian"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Romanian"), "ro", "romanian"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Russian"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Russian"), "ru", "russian"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Serbian"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Serbian"), "sr", "serbian"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Slovak"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Slovak"), "sk", "slovak"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Slovenian"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Slovenian"), "sl",	"slovenian"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Spanish"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Spanish"), "es", "spanish"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Swedish"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Swedish"), "sv", "swedish"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Thai"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Thai"), "th", "thai"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Turkish"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Turkish"), "tr", "turkish"),			
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Ukranian"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Ukranian"), "uk", "ukranian"),
				new LanguageItem (AddinManager.CurrentLocalizer.GetString ("Vietnamese"),
				    AddinManager.CurrentLocalizer.GetString ("Translate to Vietnamese"), "vi", "vietnamese"),
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

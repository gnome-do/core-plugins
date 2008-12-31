// ITranslateProvider.cs
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
using System.Collections.Generic;

namespace Translate
{
	
	public interface ITranslateProvider
	{
		//the class name _MUST_ be the same name provided by this property.
		string Name { get; }
		string Icon { get; }
		string BuildTextRequestUrl(string ifaceLang, string toLang, string fromLang, string req);
		string BuildUrlRequestUrl(string ifaceLang, string toLang, string fromLang, string req);
		List<LanguageItem> SupportedLanguages { get; }
		string DefaultSourceCode { get; }
		bool SupportsAutoDetect { get; }
		string AutoDetectCode { get; }
		bool SupportsUrlTranslate { get; }
		bool SupportsIfaceLang { get; }
		string DefaultIfaceCode { get; }
	}
}

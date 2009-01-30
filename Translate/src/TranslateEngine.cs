// Translate.cs created with MonoDevelop
// User: chris at 6:28 PM 1/22/2009
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;

namespace Translate
{
	
	public class TranslateEngine
	{
		
		static TranslateEngine ()
		{
			LoadValuesFromPrefs ();
		}
		
		public static ITranslateProvider Translator { get; private set; }
		
		public static void LoadValuesFromPrefs()
		{
			Translator = TranslateProviderFactory.GetProviderFromPreferences ();
		}
	}
}

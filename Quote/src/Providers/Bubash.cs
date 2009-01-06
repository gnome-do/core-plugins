/* Bubash.cs 
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this
 * source distribution.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

using Do.Platform;

namespace Quote
{

  public class Bubash : IQuoteProvider
  {
    const string urlRoot = "http://bubash.org/";
    const int SaveTagsTimeout = 60 * 10 * 1000; //every 10 minutes we save tags

    List<QuoteTagItem> tags;

    readonly string TagFileName;
    
    public Bubash()
    {
	Parameters = new NameValueCollection ();
	Parameters ["tags"] = "";
	Parameters ["quote"] = "";
	
	TagFileName = Path.Combine (Services.Paths.UserDataDirectory, "BubashTags.txt");

	tags = LoadSavedTags ();

	GLib.Timeout.Add (SaveTagsTimeout, () => { SaveTags (); return true; });
    }
    
    public Bubash (string quote) : this ()
    {
	Parameters ["quote"] = quote;
    }
    
    public Bubash (string quote, IEnumerable<QuoteTagItem> tags) : this ()
    {
	Parameters ["quote"] = quote;
	Parameters ["tags"] = string.Join (" ", tags.Select (item => item.Name).ToArray ());
    }
    
    public string Name {
      get { return "BuBash.org"; }
    }
    
    public string BaseUrl {
      get { return urlRoot + "submit"; }
    }
    
    public bool ShouldAllowAutoRedirect {
      get { return false; }
    }

    public IEnumerable<QuoteTagItem> SavedTags {
      get { return tags; }
    }
    
    public NameValueCollection Parameters { get; private set; }
    
    public void AddTag (QuoteTagItem tag)
    {
      tags.Add (tag);
    }
    
    public string GetQuoteUrlFromResponse (HttpWebResponse response)
    {
      return urlRoot + "/queue";
    }
    
    List<QuoteTagItem> LoadSavedTags ()
    {
      if (!File.Exists (TagFileName)) {
	Log.Debug ("{0} Does not exist, cannot load saved tags");
	return;
      }

      tags.Clear ();

      using (StreamReader sr = File.OpenText (TagFileName)) {
	string input;
	
	while (input = sr.ReadLine () != null) {
	  tags.Add (new QuoteTagItem (input));
	}
      }
    }

    void SaveTags ()
    {
      Log.Debug ("Loading tags from {0}", TagFileName);
      
      using (StreamWriter sw = new StreamWriter (TagFileName)) {
	tags.ForEach (item => sw.WriteLine (item));
      }
    }
  }
}

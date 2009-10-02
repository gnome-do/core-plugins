//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using Do;

namespace SqueezeCenter
{
	
	public static class Settings
	{
		
		public static void ReadSettings (string filename, ICollection<Setting> settings, bool storeMissingValues) 
		{
			// Console.WriteLine("Reading settings from " + filename);

			string line, key, val;
			int i;
			List<Setting> foundValues = new List<Setting> ();
			StreamReader fileReader;
			StreamWriter fileWriter;
						
			if (File.Exists (filename)) {
				
				try {
					using (fileReader = new StreamReader (filename)) {
					
						while (null != (line = fileReader.ReadLine ())) {
						
							line = line.Trim ();
							if (line.Length == 0 || line.StartsWith ("#")) continue;
							
							i = line.IndexOf ("=");
							if (i <= 0) continue;
							
							key = line.Substring (0, i).Trim ();
							val = line.Substring (i+1, line.Length - i - 1).Trim ();
							
							foreach (Setting setting in settings) {
								if (string.Equals (key, setting.Name, System.StringComparison.OrdinalIgnoreCase)) {
									setting.Value = val;
									foundValues.Add (setting);
									break;
								}
							}
						}
					}
				}
				catch (Exception ex) {
					Console.WriteLine ("SqueezeCenter: Error reading configuration file \"{0}\". Message: {1}", filename, ex.ToString ());
					return;	
				}
			}

			try {
				if (storeMissingValues && foundValues.Count < settings.Count) {
					using (fileWriter = new StreamWriter (filename, true)) {
						foreach (Setting setting in settings) {
							if (!foundValues.Contains (setting)) {
								fileWriter.WriteLine ("# {0}", setting.Description);
								fileWriter.WriteLine ("{0} = {1}", setting.Name, setting.DefaultValue.ToString ());
							}
						}
					}
				}
			}
			catch (Exception ex) {
				Console.WriteLine ("SqueezeCenter: Error writing configuration file \"{0}\". Message: {1}", filename, ex.ToString ());
			}
		}
		
		public static void SaveSettings (string filename, ICollection<Setting> settings) 
		{
			StreamWriter fileWriter;						
			
			using (fileWriter = new StreamWriter (filename, false)) 
			{
				foreach (Setting setting in settings) 
				{					
					fileWriter.WriteLine ("# {0}", setting.Description);
					fileWriter.WriteLine ("{0} = {1}", setting.Name, setting.Value.ToString ());					
				}
			}
		}

		public class Setting
		{
			string name, description, val;
			object defaultVal;
			
			public Setting (string name, string description, object defaultVal)
			{
				this.name = name;
				this.description = description;
				this.val = defaultVal.ToString ();
				this.defaultVal = defaultVal;
			}
								
			public string Name
			{
				get { return this.name; }
			}
				
			public string Description
			{
				get { return this.description; }
			}
								
			public object DefaultValue
			{
				get { return this.defaultVal; }
				set { this.defaultVal = value; }
			}
			
			public string Value
			{
				get { return this.val; }
				set { this.val = value; }
			}
			
			public int ValueAsInt
			{
				get {
					int result;
					if (!int.TryParse (this.val, out result))
						result = (int)DefaultValue;
					return result;
				}
				set {
					this.val = value.ToString ();
				}
			}
				
			public bool ValueAsBool
			{
				get {
					bool result;
					if (!bool.TryParse (this.val, out result))
						result = (bool)DefaultValue;
					return result;
				}
				set {
					this.val = value.ToString ();
				}
			}
		}
	}
}

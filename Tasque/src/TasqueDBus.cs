// TasqueDBus.cs
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, see <http://www.gnu.org/licenses/> or 
// write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, 
// Boston, MA 02111-1307 USA
//


using System;
using System.Linq;
using System.Collections.Generic;
using NDesk.DBus;
using org.freedesktop.DBus;

using Do.Platform;

using Do.Universe;


namespace Tasque.DBus
{
	[Interface("org.gnome.Tasque.RemoteControl")]
	public interface ITasque
	{
	    string CreateTask (string categoryName, string taskName, bool enterEditMode);
		string [] GetCategoryNames ();
		void ShowTasks ();
	}

	public class TasqueDBus
	{
		
		const string OBJECT_PATH = "/org/gnome/Tasque/RemoteControl";
		const string BUS_NAME = "org.gnome.Tasque";
		static ITasque Tasque;

		public TasqueDBus ()
		{
			try {
				Tasque = FindInstance ();
            } catch (Exception) {
            	Log.Error ("Could not locate Tasque on D-Bus. Make sure Tasque is running");
            }
	        
			BusG.Init();
		}
	
		static private ITasque FindInstance () 
        {
                if (!Bus.Session.NameHasOwner (BUS_NAME)) 
                        throw new Exception (String.Format("Name {0} has no owner", BUS_NAME));
    
                return Bus.Session.GetObject<ITasque> (BUS_NAME, new ObjectPath (OBJECT_PATH));
        }
		
		
		public IEnumerable<string> GetCategoryNames () 
		{
			return Tasque.GetCategoryNames ();
		}

		
		public string CreateTask (string category, string task) 
		{
			IEnumerable<string> categories = GetCategoryNames ();
			
			if (categories.Contains (category)) 
				return Tasque.CreateTask (category, task, false);
			else 
				return Tasque.CreateTask (categories.First (), task, false);
			
		}
		
		public void ShowTasks ()
		{
			Tasque.ShowTasks ();
		}
	}
}

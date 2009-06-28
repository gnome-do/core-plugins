/* XRandR.cs
 *
 * GNOME Do is the legal property of its developers. Please refer to the
 * COPYRIGHT file distributed with this source distribution.
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
 *
 * XRandR is a P/Invoke wrapper around libX11 and libXrandr
 */

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace XRandR
{
	public delegate void ResourceAction<T> (T res);
	public delegate void ResourceActionWithId<T> (int id, T res);

	public class Tools
	{
		public static T Structure<T> (IntPtr ptr) 
		{
			return (T) Marshal.PtrToStructure (ptr, typeof(T));
		}

		public interface Accessor<T> {
			void DoWith (int id, ResourceAction<T> func);
			IEnumerable<T> DoWith (int id);
			void AllWithId(ResourceActionWithId<T> func);
			IEnumerable<T> All{get;}
			IEnumerable<int> Ids{get;}
		}
		public delegate IntPtr RetrieveFunc (int id);
		public delegate void FreeFunc (IntPtr element);
		public class AccessorImpl<T> : Accessor<T>{
			private RetrieveFunc getF;
			private FreeFunc freeF;
			private IEnumerable<int> ids;
			public AccessorImpl (RetrieveFunc getF, FreeFunc freeF, IEnumerable<int> ids)
			{
				this.getF = getF;
				this.freeF = freeF;
				this.ids = ids;
			}
			
			public IEnumerable<T> All {
				get{
					foreach(int id in ids){
						IntPtr ptr = getF (id);
						yield return Structure<T> (ptr);
						freeF (ptr);
					}
				}
			}
			public void AllWithId (ResourceActionWithId<T> func) 
			{
				foreach(int id in ids) {
					IntPtr ptr = getF (id);
					func (id, Structure<T> (ptr));
					freeF (ptr);
				}
			}
			public void DoWith(int id, ResourceAction<T> func) 
			{
				IntPtr ptr = getF (id);
				func (Structure<T> (ptr));
				freeF (ptr);
			}
			public IEnumerable<T> DoWith (int id) 
			{
				IntPtr ptr = getF (id);
				yield return Structure<T> (ptr);
				freeF (ptr);
			}
			public IEnumerable<int> Ids{
				get{
					return ids;
				}
			}
		}
		
		// some helper to access different sorts of unmanaged arrays
		
		// defined as int * in a structure
		public static int[] PtrToIntArray(IntPtr ptr, int numElements)
		{
			int[] res = new int[numElements];
			for (int i=0;i<numElements;i++)
				res[i] = Marshal.ReadIntPtr (ptr, IntPtr.Size * i).ToInt32 ();
			return res;
		}
		// defined as struct** in a structure
		public static T[] PtrToStructurePtrArray<T> (IntPtr ptr, int numElements)
		{
			T[] res = new T[numElements];
			for (int i=0;i<numElements;i++){
				res[i] = (T) Marshal.PtrToStructure (Marshal.ReadIntPtr (ptr, IntPtr.Size * i), (Type)typeof(T));
			}
			return res;
		}
		// defined as struct* in a structure
		public static T[] PtrToStructureArray<T>(IntPtr ptr,int numElements)
		{
			T[] res = new T[numElements];
			for (int i=0;i<numElements;i++)
				res[i] = (T) Marshal.PtrToStructure (new IntPtr (ptr.ToInt64 () + i * Marshal.SizeOf (typeof(T)))
				                                   ,typeof(T));
			return res;
		}
		
		public static void LogStructure (object o)
		{
			Type t = o.GetType ();
			
			Do.Platform.Log<XRandR.Tools>.Debug ("Dumping object of type {0} Size: {1}", t.Name, Marshal.SizeOf (t));

			foreach(System.Reflection.FieldInfo fi in t.GetFields ())
				Do.Platform.Log<XRandR.Tools>.Debug ("\t{0} (+ {2}) = {1}", fi.Name, fi.GetValue(o), Marshal.OffsetOf (t,fi.Name));
		}
		
		public class XErrorException:Exception
		{
			XErrorEvent xevent;
			string error_text;
			internal XErrorException(XErrorEvent xevent, string text){
				this.xevent = xevent;
				this.error_text = text;
			}
			public override string ToString() {
				return "got X error: "+"display:"+xevent.display+
			                  " error:"+((int)xevent.error_code)+"("+error_text+")"+
			                  " serial:"+xevent.serial+
			                  " request:"+xevent.request_code+
			                  " minor:"+xevent.minor_code;			                  
			}
		}
		
		public static string GetErrorText (IntPtr display, XErrorEvent xevent)
		{
			StringBuilder sb = new StringBuilder (1000);
			Native.XGetErrorText (display, xevent.error_code, sb, sb.Capacity);
			return sb.ToString ();			
		}
		public static IntPtr IgnoreErrorHandler (IntPtr display, IntPtr ev)
		{
			XErrorEvent xevent = Structure<XErrorEvent> (ev);
			string text = GetErrorText (display, xevent);
			XErrorException excp = new XErrorException (xevent, text);
			
			Do.Platform.Log<XRandR.Tools>.Debug ("XRandR plugin: {0}\n{1}", excp.ToString (), Environment.StackTrace);

			// don't know if it is a good idea to throw an exception out 
			// of unmanaged code? But seems to work well. 
			throw excp;
		}
	}
	public class Wrapper
	{
		public static void DoWithDefaultDisplay (ResourceAction<IntPtr> func)
		{
			foreach(IntPtr display in DefaultDisplay ())
				func(display);
		}
		// IEnumerable wrapper around resource, makes sure resources are freed after usage.
		// It is the reponsibility of the user to don't leak any pointers outside of the foreach block.
		public static IEnumerable<IntPtr> DefaultDisplay()
		{
			IntPtr oldHandler = Native.XSetErrorHandler (Marshal.GetFunctionPointerForDelegate (new Native.ErrorHandler (Tools.IgnoreErrorHandler)));
			IntPtr display = Native.XOpenDisplay (null);
			try{
				yield return display;
			}
			finally{
				Native.XCloseDisplay (display);
				Native.XSetErrorHandler (oldHandler);
			}
		}
		public static void DoWithScreenResources (IntPtr display, ResourceAction<ScreenResources> func)
		{
			foreach(ScreenResources res in ScreenResources (display))
				func(res);
		}
		public static IEnumerable<ScreenResources> ScreenResources (IntPtr display)
		{
			IntPtr w = Native.XRootWindow (display, 0);
			IntPtr res = Native.XRRGetScreenResources (display, w);
			yield return new ScreenResources (display, res);
			Native.XRRFreeScreenResources (res);
		}
		public static void DoWithScreenResources (ResourceAction<ScreenResources> func)
		{
			DoWithDefaultDisplay (delegate (IntPtr display){DoWithScreenResources (display, func);});
		}
		public static IEnumerable<ScreenResources> ScreenResources()
		{
			foreach(IntPtr display in DefaultDisplay ())
				foreach(ScreenResources res in ScreenResources (display))
					yield return res;
		}
	}
	public class ScreenResources {
		IntPtr display;
		IntPtr presources;
		XRRScreenResources resources;
		Dictionary<int,XRRModeInfo> modes = new Dictionary<int,XRRModeInfo>();
		
		internal ScreenResources(IntPtr d, IntPtr presources)
		{
			this.resources = Tools.Structure<XRRScreenResources> (presources);
			Tools.LogStructure (this.resources);
			this.presources = presources;
			this.display = d;
			
			foreach(XRRModeInfo mode in Tools.PtrToStructureArray<XRRModeInfo>(resources.modes,resources.nmode)){
				modes[mode.id.ToInt32()] = mode;
			}
		}

		public Tools.Accessor<XRROutputInfo> Outputs {
			get{
				return new Tools.AccessorImpl<XRROutputInfo> (delegate (int id) {
					                                      return Native.XRRGetOutputInfo (display, presources,id);
				                                       }
				                                       ,Native.XRRFreeOutputInfo
				                                       ,Tools.PtrToIntArray (resources.outputs, resources.noutput));
			}
		}
		public Tools.Accessor<XRRCrtcInfo> Crtcs {
			get{
				return new Tools.AccessorImpl<XRRCrtcInfo> (delegate (int id) {
					                                      return Native.XRRGetCrtcInfo (display, presources, id);
				                                       }
				                                       ,Native.XRRFreeCrtcInfo
				                                       ,Tools.PtrToIntArray (resources.crtcs, resources.ncrtc));
			}
		}
		
		public XRRModeInfo GetMode(int id)
		{
			return modes[id];
		}
		public IEnumerable<XRRModeInfo> Modes()
		{
			return modes.Values;
		}
		public IEnumerable<XRRModeInfo> ModesOfOutput(XRROutputInfo output)
		{
			foreach(int mode_id in Tools.PtrToIntArray (output.modes, output.nmode))
				yield return GetMode (mode_id);
		}
		
		public XRRScreenResources Resources{
			get{
				return resources;
			}
		}
		
		public static void SafeSetConfig (IntPtr display, IntPtr res, IntPtr crtc_id, IntPtr timestamp, int x, int y, IntPtr mode_id, int rotation, int[] outputs)
		{
			try{
				IntPtr ptr = Marshal.AllocHGlobal (sizeof(int) * outputs.Length);
				for(int i=0;i<outputs.Length;i++)
					Marshal.WriteInt32 (ptr, sizeof(int)*i, outputs[i]);
				Native.XRRSetCrtcConfig (display, res, crtc_id, timestamp, x, y, mode_id, rotation, ptr, outputs.Length);
			    Marshal.FreeHGlobal (ptr);
			}
			catch(Tools.XErrorException excp){
				Do.Platform.Log<XRandR.Tools>.Debug ("Error when calling XRRSetCtrcConfig: 0x{0:x},{1},{2},{3},0x{4:x},{5},[{6}]"
				                  ,crtc_id
				                  ,timestamp
				                  ,x
				                  ,y
				                  ,mode_id
				                  ,rotation
				                  ,outputs);
				throw excp;
			}
		}
		
		// Sets the mode of an output. Doesn't change any settings such as position offset or rotation.
		public void setMode (int output_id, int mode_id)
		{
			// use xrandr command line tool to set mode for now, since it is more reliable in
			//  - changing screen size if necessary and calculating DPI and mm sizes
			//  - finding a good CRTC allocation
			string cmd = string.Format ("xrandr --output 0x{0:x} --mode 0x{1:x}", output_id, mode_id);
			
			Do.Platform.Log<XRandR.Tools>.Debug ("Setting mode using: '{0}'", cmd);

			System.Diagnostics.Process.Start (cmd);
			/*foreach(XRROutputInfo output in Outputs.doWith(output_id)){
				int crtc_id = output.crtc_id;

				if (mode_id != 0){
					if (crtc_id == 0) // if output is switched off, output has no crtc defined, so we use 1st one
						crtc_id = Tools.PtrToIntArray(output.crtcs,output.ncrtc)[0];
						
					foreach(XRRCrtcInfo crtc in Crtcs.doWith(crtc_id))
						safeSetConfig(display,presources,crtc_id,0,crtc.x,crtc.y,mode_id,crtc.rotation,new[]{output_id});
				}
				else  // switch off output, by setting the mode of the crtc of the output to 0 
					safeSetConfig(display,presources,crtc_id,output.timestamp,0,0,0,1,new int[0]);
			}*/
		}
	}
	
	// for testing purposes
	class MainClass
	{
		public static void PrintModeInfo (XRRModeInfo mode)
		{
			Console.WriteLine ("Id: "+mode.id+" Name: "+mode.name+" width: "+mode.width+" height: "+mode.height);
		}			
		public static void PrintOutputInfo (int id,XRROutputInfo output)
		{
			Console.WriteLine ("Id: {0} Name: {1} Connection: {2} crtc:{3}", id, output.name, output.connection, output.crtc_id);
		}
		public static void Main(string[] args)
		{
			Tools.LogStructure (new XRRScreenResources());
			Tools.LogStructure (new XRRCrtcInfo());
			Tools.LogStructure (new XRRModeInfo());
			Tools.LogStructure (new XRROutputInfo());
		}
	}
}

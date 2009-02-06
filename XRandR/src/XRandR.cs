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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace XRandR
{
	public delegate void ResourceAction<T>(T res);
	public delegate void ResourceActionWithId<T>(int id,T res);
	
	[StructLayout (LayoutKind.Sequential)]
	public struct XRROutputInfo {
	    public int timestamp;
	    
		public int	    crtc_id;

		public string	    name;
	    public int		    nameLen;

		public int   mm_width;
	    public int   mm_height;

		public short	    connection;
	    public short   subpixel_order;

		public int		    ncrtc;
	    public IntPtr	    crtcs;

		public int		    nclone;
		public IntPtr   	clones;
	    
		public int		    nmode;
	    public int		    npreferred;
	    public IntPtr	    modes;
	};

	[StructLayout (LayoutKind.Sequential)]
	public struct XErrorEvent{
        public int type;
        public IntPtr display;   /* Display the event was read from */
		public long serial;/* serial number of failed request */
        public byte error_code;/* error code of failed request */
        public byte request_code;/* Major op-code of failed request */
        public byte minor_code;/* Minor op-code of failed request */
        public int resourceid;     /* resource id */
	};
	
	[StructLayout (LayoutKind.Sequential)]
	public struct XRRModeInfo {
		public int	id;
		public int	width;
		public int	height;
		public int	dotClock;
		public int	hSyncStart;
		public int	hSyncEnd;
		public int	hTotal;
		public int	hSkew;
		public int	vSyncStart;
		public int	vSyncEnd;
		public int	vTotal;
		public string  name;
		public int	nameLength;
		public int	modeFlags;
	};
	
	[StructLayout (LayoutKind.Sequential)]
	public struct XRRCrtcInfo {
		public int timestamp;
		public int x;
		public int y;
		public int width, height;
		public int mode;
		public short rotation;

		public int noutput;
		public IntPtr outputs;
		
		public IntPtr rotations;
		
		public int npossible;
		public IntPtr possible;
	};
	
	[StructLayout(LayoutKind.Sequential)]
	public struct XRRScreenResources{
		public int	timestamp;
		public int	configTimestamp;

		public int		ncrtc;
		public IntPtr	crtcs;
		
		public int		noutput;
		public IntPtr   outputs;
		
		public int		nmode;
		public IntPtr	modes;
	}
	
	public class External{
		[DllImport("libX11")]
		public static extern IntPtr XOpenDisplay([MarshalAs(UnmanagedType.LPTStr)] string name);
		[DllImport("libX11")]
        public static extern int XCloseDisplay(IntPtr display);
		[DllImport("libX11")]
		public static extern IntPtr XRootWindow(IntPtr display,int screen);
		
		public delegate IntPtr ErrorHandler(IntPtr display,IntPtr ev);
		[DllImport("libX11")]
		public static extern IntPtr XSetErrorHandler(IntPtr handler);
		
		[DllImport("libX11")]
		public static extern int XGetErrorText(IntPtr display, int code, StringBuilder sb,
              int length);
		
		[DllImport("libXrandr")]
		public static extern IntPtr XRRGetScreenResources (IntPtr dpy, IntPtr window);
		
		[DllImport("libXrandr")]
		public static extern void XRRFreeScreenResources (IntPtr resources);

		[DllImport("libXrandr")]
		public static extern IntPtr XRRGetOutputInfo (IntPtr dpy, IntPtr resources, int output_id);
		
		[DllImport("libXrandr")]
		public static extern void XRRFreeOutputInfo (IntPtr outputInfo);
		
		[DllImport("libXrandr")]
		public static extern IntPtr XRRGetCrtcInfo (IntPtr dpy, IntPtr resources, int crtc_id);
		
		[DllImport("libXrandr")]
		public static extern void XRRFreeCrtcInfo (IntPtr crtcInfo);
		
		[DllImport("libXrandr")]
		public static extern int XRRSetCrtcConfig (IntPtr dpy,
		                                           IntPtr resources,
		                                           int crtc_id,
		                                           int timestamp,
		                                           int x, int y,
		                                           int mode_id,
		                                           int rotation,
		                                           IntPtr outputs,
		                                           int noutputs);
		
		public static T Structure<T>(IntPtr ptr){
			return (T)Marshal.PtrToStructure(ptr,typeof(T));
		}
		/*public delegate void DoWithResource<T>(int id,ResourceAction<T> func);
		public static DoWithResource<T> Accessor<T>(RetrieveFunc getF,FreeFunc freeF){
			return delegate(int id,ResourceAction<T> func){
				IntPtr p = getF(id);
				func(External.Structure<T>(p));
				freeF(p);
			};
		}
		public delegate void ForEachFunc<T>(IntPtr arrayPtr,int numEls,ResourceAction<T> func);
		
		public static ForEachFunc<T> CreateForEach<T>(RetrieveFunc getF,FreeFunc freeF){
			return delegate(IntPtr array,int numEls,ResourceAction<T> func){
				foreach(int id in PtrToIntArray(array,numEls)){
					IntPtr ptr = getF(id);
					func((T)Marshal.PtrToStructure(ptr,typeof(T)));
					freeF(ptr);
				}
			};
		}
		public static ForEachFunc<T> CreateForEach<T>(DoWithResource<T> acc){
			return delegate(IntPtr array,int numEls,ResourceAction<T> func){
				foreach(int id in PtrToIntArray(array,numEls))
					acc(id,func);
			};
		}*/

		public interface Accessor<T>{
			void doWith(int id,ResourceAction<T> func);
			IEnumerable<T> doWith(int id);
			void AllWithId(ResourceActionWithId<T> func);
			IEnumerable<T> All{get;}
			IEnumerable<int> Ids{get;}
		}
		public delegate IntPtr RetrieveFunc(int id);
		public delegate void FreeFunc(IntPtr element);
		public class AccessorImpl<T> : Accessor<T>{
			private RetrieveFunc getF;
			private FreeFunc freeF;
			private IEnumerable<int> ids;
			public AccessorImpl(RetrieveFunc getF,FreeFunc freeF,IEnumerable<int> ids){
				this.getF = getF;
				this.freeF = freeF;
				this.ids = ids;
			}
			
			public IEnumerable<T> All{
				get{
					foreach(int id in ids){
						IntPtr ptr = getF(id);
						yield return Structure<T>(ptr);
						freeF(ptr);
					}
				}
			}
			public void AllWithId(ResourceActionWithId<T> func){
				foreach(int id in ids){
					IntPtr ptr = getF(id);
					func(id,Structure<T>(ptr));
					freeF(ptr);
				}
			}
			public void doWith(int id,ResourceAction<T> func){
				IntPtr ptr = getF(id);
				func(Structure<T>(ptr));
				freeF(ptr);
			}
			public IEnumerable<T> doWith(int id){
				IntPtr ptr = getF(id);
				yield return Structure<T>(ptr);
				freeF(ptr);
			}
			public IEnumerable<int> Ids{
				get{
					return ids;
				}
			}
		}
		
		// some helper to access different sorts of unmanaged arrays
		
		// defined as int * in a structure
		public static int[] PtrToIntArray(IntPtr ptr,int numElements){
			int[] res = new int[numElements];
			for (int i=0;i<numElements;i++)
				res[i] = Marshal.ReadIntPtr(ptr,IntPtr.Size * i).ToInt32();
			return res;
		}
		// defined as struct** in a structure
		public static T[] PtrToStructurePtrArray<T>(IntPtr ptr,int numElements){
			T[] res = new T[numElements];
			for (int i=0;i<numElements;i++){
				res[i] = (T)Marshal.PtrToStructure(Marshal.ReadIntPtr(ptr,IntPtr.Size * i),(Type)typeof(T));
			}
			return res;
		}
		// defined as struct* in a structure
		public static T[] PtrToStructureArray<T>(IntPtr ptr,int numElements){
			T[] res = new T[numElements];
			for (int i=0;i<numElements;i++)
				res[i] = (T)Marshal.PtrToStructure(new IntPtr(ptr.ToInt32() + i * Marshal.SizeOf(typeof(T)))
				                                   ,typeof(T));
			return res;
		}
		
		public class XErrorException:Exception{
			XErrorEvent xevent;
			string error_text;
			internal XErrorException(XErrorEvent xevent,string text){
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
		
		public static string GetErrorText(IntPtr display,XErrorEvent xevent){
			StringBuilder sb = new StringBuilder(1000);
			XGetErrorText(display,xevent.error_code,sb,sb.Capacity);
			return sb.ToString();			
		}
		public static IntPtr ignoreErrorHandler(IntPtr display,IntPtr ev){
			XErrorEvent xevent = Structure<XErrorEvent>(ev);
			string text = GetErrorText(display,xevent);
			XErrorException excp = new XErrorException(xevent,text);
			
			Console.WriteLine("XRandR plugin: "+excp.ToString());
			Console.WriteLine(Environment.StackTrace);
			
			// don't know if it is a good idea to throw an exception out 
			// of unmanaged code? But seems to work well. 
			throw excp;
		}
		public static void doWithDefaultDisplay(ResourceAction<IntPtr> func){
			foreach(IntPtr display in DefaultDisplay())
				func(display);
		}
		// IEnumerable wrapper around resource, makes sure resources are freed after usage.
		// It is the reponsibility of the user to don't leak any pointers outside of the foreach block.
		public static IEnumerable<IntPtr> DefaultDisplay(){
			IntPtr oldHandler = XSetErrorHandler(Marshal.GetFunctionPointerForDelegate(new ErrorHandler(ignoreErrorHandler)));
			IntPtr display = XOpenDisplay(null);
			try{
				yield return display;
			}
			finally{
				XCloseDisplay(display);
				XSetErrorHandler(oldHandler);
			}
		}
		public static void doWithScreenResources(IntPtr display,ResourceAction<ScreenResources> func){
			foreach(ScreenResources res in ScreenResources(display))
				func(res);
		}
		public static IEnumerable<ScreenResources> ScreenResources(IntPtr display){
			IntPtr w = External.XRootWindow(display,0);
			IntPtr res = External.XRRGetScreenResources(display,w);
			yield return new ScreenResources(display,res);
			External.XRRFreeScreenResources(res);
		}
		public static void doWithScreenResources(ResourceAction<ScreenResources> func){
			doWithDefaultDisplay(delegate(IntPtr display){doWithScreenResources(display,func);});
		}
		public static IEnumerable<ScreenResources> ScreenResources(){
			foreach(IntPtr display in DefaultDisplay())
				foreach(ScreenResources res in ScreenResources(display))
					yield return res;
		}
	}
	public class ScreenResources{
		IntPtr display;
		IntPtr presources;
		XRRScreenResources resources;
		Dictionary<int,XRRModeInfo> modes = new Dictionary<int,XRRModeInfo>();
		
		internal ScreenResources(IntPtr d,IntPtr presources){
			this.resources = External.Structure<XRRScreenResources>(presources);
			this.presources = presources;
			this.display = d;
			
			foreach(XRRModeInfo mode in External.PtrToStructureArray<XRRModeInfo>(resources.modes,resources.nmode)){
				modes[mode.id] = mode;
			}
		}

		public External.Accessor<XRROutputInfo> Outputs{
			get{
				return new External.AccessorImpl<XRROutputInfo>(delegate(int id){
					                                      return External.XRRGetOutputInfo(display,presources,id);
				                                       }
				                                       ,External.XRRFreeOutputInfo
				                                       ,External.PtrToIntArray(resources.outputs,resources.noutput));
			}
		}
		public External.Accessor<XRRCrtcInfo> Crtcs{
			get{
				return new External.AccessorImpl<XRRCrtcInfo>(delegate(int id){
					                                      return External.XRRGetCrtcInfo(display,presources,id);
				                                       }
				                                       ,External.XRRFreeCrtcInfo
				                                       ,External.PtrToIntArray(resources.crtcs,resources.ncrtc));
			}
		}
		
		public XRRModeInfo GetMode(int id){
			return modes[id];
		}
		public IEnumerable<XRRModeInfo> Modes(){
			return modes.Values;
		}
		public IEnumerable<XRRModeInfo> ModesOfOutput(XRROutputInfo output){
			foreach(int mode_id in External.PtrToIntArray(output.modes,output.nmode))
				yield return GetMode(mode_id);
		}
		
		public XRRScreenResources Resources{
			get{
				return resources;
			}
		}
		
		// Sets the mode of an output. Doesn't change any settings such as position offset or rotation.
		public void setMode(int output_id,int mode_id){
			foreach(XRROutputInfo output in Outputs.doWith(output_id)){
				int crtc_id = output.crtc_id;

				if (mode_id != 0){
					if (crtc_id == 0) // if output is switched off, output has no crtc defined, so we use 1st one
						crtc_id = External.PtrToIntArray(output.crtcs,output.ncrtc)[0];
						
					foreach(XRRCrtcInfo crtc in Crtcs.doWith(crtc_id)){
					    IntPtr ptr = Marshal.AllocHGlobal(sizeof(int));
					    Marshal.WriteInt32(ptr,output_id);
						External.XRRSetCrtcConfig(display,presources,crtc_id,output.timestamp,crtc.x,crtc.y,mode_id,crtc.rotation,ptr,1);
					    Marshal.FreeHGlobal(ptr);
				    }
				}
				else  // switch off output, by setting the mode of the crtc of the output to 0 
					External.XRRSetCrtcConfig(display,presources,crtc_id,output.timestamp,0,0,0,1,new IntPtr(0),0);
			}
		}
	}
	
	class MainClass
	{
		public static void printModeInfo(XRRModeInfo mode){
			Console.WriteLine("Id: "+mode.id+" Name: "+mode.name+" width: "+mode.width+" height: "+mode.height);
		}			
		public static void printOutputInfo(int id,XRROutputInfo output){
			Console.WriteLine("Id: "+id+" Name: "+output.name+" Connection: "+output.connection);
		}
		public static void Main(string[] args)
		{
			foreach(ScreenResources res in External.ScreenResources()){
				foreach(XRRModeInfo mode in res.Modes())
					printModeInfo(mode);
				
				res.Outputs.AllWithId(printOutputInfo);
				
				foreach(XRROutputInfo output in res.Outputs.All){
					Console.WriteLine("Modes for "+output.name);
					foreach (XRRModeInfo mode in res.ModesOfOutput(output))
						printModeInfo(mode);
				}
				res.setMode(60,0);
			};
		}
	}
}
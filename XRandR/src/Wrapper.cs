// Wrapper.cs created with MonoDevelop
// User: johannes at 5:01 PM 2/4/2009
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Runtime.InteropServices;
using Window = System.IntPtr;
using Display = System.IntPtr;

namespace XRandR
{
	
	
	public class Wrapper
	{
		public static string[] OutputNames {
			get{
				IntPtr d = XOpenDisplay(null);
				Console.WriteLine("Open display "+d);
			
				Window w = XRootWindow(d,0);
				Console.WriteLine("root window: " + w);
			
				IntPtr res = XRRGetScreenResources(d,w);
				Console.WriteLine("resources "+res);

				XRRScreenResources res2 = (XRRScreenResources) Marshal.PtrToStructure(res,typeof(XRRScreenResources));
			
				Console.WriteLine("num outputs: "+res2.noutput);
				Console.WriteLine("num crtcs: "+res2.ncrtc);
				Console.WriteLine("num modes: "+res2.nmode);
				Console.WriteLine("output 0: "+Marshal.ReadIntPtr(res2.outputs,0));
			
				string []names = new string[res2.noutput];
				int i=0;
				foreach(IntPtr output in PtrToArray(res2.outputs,res2.noutput))
				{
					IntPtr pinfo = XRRGetOutputInfo(d,res,output);
					XRROutputInfo info = (XRROutputInfo) Marshal.PtrToStructure(pinfo,typeof(XRROutputInfo));
					//Console.WriteLine("Name: "+info.name);
					names[i]=info.name;
					i++;
				}			
			
				Console.WriteLine("Close display "+XCloseDisplay(d));
				return names;
			}
		}
		
		public static IntPtr[] PtrToArray(IntPtr ptr,int numElements){
			IntPtr[] res = new IntPtr[numElements];
			for (int i=0;i<numElements;i++)
				res[i] = Marshal.ReadIntPtr(ptr,IntPtr.Size * i);
			return res;
		}
		
		[DllImport("libX11")]
		public static extern Display XOpenDisplay([MarshalAs(UnmanagedType.LPTStr)] string name);
		[DllImport("libX11")]
        public static extern int XCloseDisplay(Display display);
		[DllImport("libX11")]
		public static extern Window XRootWindow(Display d,int screen);
		
		[DllImport("libXrandr")]
		public static extern IntPtr XRRGetScreenResources (Display dpy, Window window);
		
		[StructLayout (LayoutKind.Sequential)]
		public struct XRRScreenResources{
			int	timestamp;
			int	configTimestamp;
			public int		ncrtc;
			IntPtr	crtcs;
			public int		noutput;
			public IntPtr   outputs;
			public int		nmode;
			IntPtr	modes;
		}
		
		[DllImport("libXrandr")]
		public static extern void XRRFreeScreenResources (IntPtr resources);

		public struct XRROutputInfo {
		    int	    timestamp;
		    IntPtr	    crtc;
		    public string	    name;
		    int		    nameLen;
		    long   mm_width;
		    long   mm_height;
		    IntPtr	    connection;
		    IntPtr   subpixel_order;
		    int		    ncrtc;
		    IntPtr	    crtcs;
		    int		    nclone;
			IntPtr   	clones;
		    int		    nmode;
		    int		    npreferred;
		    IntPtr	    modes;
		};

		[DllImport("libXrandr")]
		public static extern IntPtr XRRGetOutputInfo (Display dpy, IntPtr resources, IntPtr output);
	}
}

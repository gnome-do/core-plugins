
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace XRandR
{
	[StructLayout (LayoutKind.Sequential)]
	public struct XErrorEvent
	{
		public int type;
		public IntPtr display;   /* Display the event was read from */
		public long serial;/* serial number of failed request */
		public byte error_code;/* error code of failed request */
		public byte request_code;/* Major op-code of failed request */
		public byte minor_code;/* Minor op-code of failed request */
		public int resourceid;     /* resource id */
	};
	
	// structures as defined in xrandr.h
	
	[StructLayout (LayoutKind.Sequential)]
	public struct XRROutputInfo
	{
		public IntPtr timestamp;
		
		public int crtc_id;
		
		public string name;
		public int nameLen;
		
		public IntPtr mm_width;
		public IntPtr mm_height;
		
		public short connection;
		public short subpixel_order;
		
		public int ncrtc;
		public IntPtr crtcs;
		
		public int nclone;
		public IntPtr clones;
		
		public int nmode;
		public int npreferred;
		public IntPtr modes;
	};
	
	[StructLayout (LayoutKind.Sequential)]
	public struct XRRModeInfo
	{
		public IntPtr id;
		public int width;
		public int height;
		public IntPtr dotClock;
		public int hSyncStart;
		public int hSyncEnd;
		public int hTotal;
		public int hSkew;
		public int vSyncStart;
		public int vSyncEnd;
		public int vTotal;
		public string name;
		public int nameLength;
		public IntPtr modeFlags;
	};
	
	[StructLayout (LayoutKind.Sequential)]
	public struct XRRCrtcInfo
	{
		public IntPtr timestamp;
		public int x;
		public int y;
		public int width;
		public int height;
		
		public IntPtr mode;
		public short rotation;
		
		public int noutput;
		public IntPtr outputs;

		public int rotations;

		public int npossible;
		public IntPtr possible;
	};
	
	[StructLayout(LayoutKind.Sequential)]
	public struct XRRScreenResources
	{
		public IntPtr timestamp;
		public IntPtr configTimestamp;

		public int ncrtc;
		public IntPtr crtcs;
		
		public int noutput;
		public IntPtr outputs;
		
		public int nmode;
		public IntPtr modes;
	}
	
	public class Native
	{
		[DllImport("libX11")]
		public static extern IntPtr XOpenDisplay ([MarshalAs(UnmanagedType.LPTStr)] string name);
		[DllImport("libX11")]
        public static extern int XCloseDisplay (IntPtr display);
		[DllImport("libX11")]
		public static extern IntPtr XRootWindow (IntPtr display, int screen);
		
		public delegate IntPtr ErrorHandler (IntPtr display, IntPtr ev);
		[DllImport("libX11")]
		public static extern IntPtr XSetErrorHandler (IntPtr handler);
		
		[DllImport("libX11")]
		public static extern int XGetErrorText (IntPtr display, int code, StringBuilder sb, int length);
		
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
		                                           IntPtr crtc_id,
		                                           IntPtr timestamp,
		                                           int x, int y,
		                                           IntPtr mode_id,
		                                           int rotation,
		                                           IntPtr outputs,
		                                           int noutputs);
		
		[DllImport("libXrandr")]
		public static extern void XRRSetScreenSize (IntPtr dpy, IntPtr window,
		                                            int width, int height,
		                                            int mmWidth, int mmHeight);
	}
}

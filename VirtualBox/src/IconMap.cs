// IconMap.cs created with MonoDevelop
// User: chris at 8:36 PM 2/4/2009
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Collections.Generic;

namespace VirtualBox
{
	
	
	public static class IconMap
	{
		static Dictionary<string, string> iconMap = new Dictionary<string,string> ();
		
		static IconMap()
		{
			iconMap.Add("Other", "unknown");
			iconMap.Add("DOS", "dos");
			iconMap.Add("L4", "l4");
			iconMap.Add("Netware", "netware");
			iconMap.Add("Windows31", "win31");
			iconMap.Add("Windows95", "win95");
			iconMap.Add("Windows98", "win98");
			iconMap.Add("WindowsMe", "winme");
			iconMap.Add("WindowsNT", "winnt4");
			iconMap.Add("WindowsNT4", "winnt4");
			iconMap.Add("Windows2000", "win2k");
			iconMap.Add("WindowsXP", "winxp");
			iconMap.Add("WindowsXP_64", "winxp_64");
			iconMap.Add("Windows2003", "win2k3");
			iconMap.Add("Windows2003_64", "win2k3_64");
			iconMap.Add("WindowsVista", "winvista");
			iconMap.Add("WindowsVista_64", "winvista_64");
			iconMap.Add("Windows2008", "win2k8");
			iconMap.Add("Windows2008_64", "win2k8_64");
			iconMap.Add("Windows7", "win7");
			iconMap.Add("Windows7_64", "win7_64");
			iconMap.Add("OS2Warp3", "os2warp3");
			iconMap.Add("OS2Warp4", "os2warp4");
			iconMap.Add("OS2Warp45", "os2warp45");
			iconMap.Add("OS2eCS", "os2ecs");
			iconMap.Add("OS2", "os2_other");
			iconMap.Add("Linux22", "linux22");
			iconMap.Add("Linux24", "linux24");
			iconMap.Add("Linux24_64", "linux24_64");
			iconMap.Add("Linux26", "linux26");
			iconMap.Add("Linux26_64", "linux26_64");
			iconMap.Add("ArchLinux", "archlinux");
			iconMap.Add("ArchLinux_64", "archlinux_64");
			iconMap.Add("Debian", "debian");
			iconMap.Add("Debian_64", "debian_64");
			iconMap.Add("OpenSUSE", "opensuse");
			iconMap.Add("OpenSUSE_64", "opensuse_64");
			iconMap.Add("Fedora", "fedoracore");
			iconMap.Add("Fedora_64", "fedoracore_64");
			iconMap.Add("Gentoo", "gentoo");
			iconMap.Add("Gentoo_64", "gentoo_64");
			iconMap.Add("Mandriva", "mandriva");
			iconMap.Add("Mandriva_64", "mandriva_64");
			iconMap.Add("RedHat", "redhat");
			iconMap.Add("RedHat_64", "redhat_64");
			iconMap.Add("Ubuntu", "ubuntu");
			iconMap.Add("Ubuntu_64", "ubuntu_64");
			iconMap.Add("Xandros", "xandros");
			iconMap.Add("Xandros_64", "xandros_64");
			iconMap.Add("Linux", "linux");
			iconMap.Add("FreeBSD", "freebsd");
			iconMap.Add("FreeBSD_64", "freebsd_64");
			iconMap.Add("OpenBSD", "openbsd");
			iconMap.Add("OpenBSD_64", "openbsd-64");
			iconMap.Add("NetBSD", "netbsd");
			iconMap.Add("NetBSD_64", "netbsd_64");
			iconMap.Add("Solaris", "solaris");
			iconMap.Add("Solaris_64", "solaris_64");
			iconMap.Add("OpenSolaris", "opensolaris");
			iconMap.Add("OpenSolaris_64", "opensolaris_64");
			iconMap.Add("QNX", "qnx");
		}
		public static string LookUp (string osType)
		{
			if (iconMap.ContainsKey(osType))
			    return iconMap[osType];
			else return "unknown";
		}
	}
}

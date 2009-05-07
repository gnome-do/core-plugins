dnl Process this file with autoconf to produce a configure script.
AC_PREREQ([2.54])
AC_INIT([gnome-do-plugins], [0.8.1.3])
AM_INIT_AUTOMAKE([foreign tar-pax])


AC_CONFIG_MACRO_DIR([m4])

dnl -- Intl

IT_PROG_INTLTOOL([0.35.0])
SHAMROCK_CONFIGURE_I18N(gnome-do-plugins)

ASM_VERSION="$VERSION"
AC_SUBST(ASM_VERSION)

dnl pkg-config
AC_PATH_PROG(PKG_CONFIG, pkg-config, no)
if test "x$PKG_CONFIG" = "xno"; then
        AC_MSG_ERROR([You need to install pkg-config])
fi

dnl We need mautil (at least for now)
AC_PATH_PROG(MAUTIL, mautil, no)
if test "x$MAUTIL" = "xno" ; then
   	AC_MSG_ERROR([You need to install mautil (from mono-addins)])
fi
AC_SUBST([MAUTIL])

AC_PROG_INSTALL

top_srcdir=$(readlink -f $0 | sed -e s/configure$//)
SHAMROCK_FIND_C_SHARP_3_0_COMPILER

dnl Expansions
SHAMROCK_EXPAND_LIBDIR
SHAMROCK_EXPAND_BINDIR
SHAMROCK_EXPAND_DATADIR

PKG_CHECK_MODULES([BANSHEE_INDEXER], [banshee-1-collection-indexer >= 1.4.2])
PKG_CHECK_MODULES([EVOLUTION_SHARP], [evolution-sharp])
PKG_CHECK_MODULES([FLICKRNET], [flickrnet])
PKG_CHECK_MODULES([GCONF_SHARP_20], [gconf-sharp-2.0])
PKG_CHECK_MODULES([GLADE_SHARP_20], [glade-sharp-2.0])
PKG_CHECK_MODULES([GLIB_SHARP_20], [glib-sharp-2.0])
PKG_CHECK_MODULES([GNOME_DESKTOP_SHARP_20], [gnome-desktop-sharp-2.0])
PKG_CHECK_MODULES([GNOME_KEYRING_SHARP], [gnome-keyring-sharp-1.0])
PKG_CHECK_MODULES([GNOME_SHARP_20], [gnome-sharp-2.0])
PKG_CHECK_MODULES([GNOME_VFS_SHARP_20], [gnome-vfs-sharp-2.0])
PKG_CHECK_MODULES([GTK_SHARP_20], [gtk-sharp-2.0])
PKG_CHECK_MODULES([MONO_ADDINS], [mono-addins])
PKG_CHECK_MODULES([MONO_ADDINS_GUI], [mono-addins-gui])
PKG_CHECK_MODULES([MONO_ADDINS_SETUP], [mono-addins-setup])
PKG_CHECK_MODULES([NDESK_DBUS_10], [ndesk-dbus-1.0])
PKG_CHECK_MODULES([NDESK_DBUS_GLIB_10], [ndesk-dbus-glib-1.0])
PKG_CHECK_MODULES([NOTIFY_SHARP], [notify-sharp])
PKG_CHECK_MODULES([WNCK_SHARP_10], [wnck-sharp-1.0])

PKG_CHECK_MODULES([GDATA_SHARP_ACL], [gdata-sharp-acl])
PKG_CHECK_MODULES([GDATA_SHARP_CALENDAR], [gdata-sharp-core gdata-sharp-calendar])
PKG_CHECK_MODULES([GDATA_SHARP_DOCUMENTS], [gdata-sharp-core gdata-sharp-documents])
PKG_CHECK_MODULES([GDATA_SHARP_CONTACTS], [gdata-sharp-core gdata-sharp-contacts ])

dnl We need to copy Banshee.CollectionIndexer.dll; it's not in the GAC
BANSHEE_INDEXER_DLL=$($PKG_CONFIG --variable=Libraries banshee-1-collection-indexer)

AC_SUBST([BANSHEE_INDEXER_DLL])
AC_SUBST([EVOLUTION_SHARP_LIBS])
AC_SUBST([FLICKRNET_LIBS])
AC_SUBST([GCONF_SHARP_20_LIBS])
AC_SUBST([GDATA_SHARP_ACL_LIBS])
AC_SUBST([GDATA_SHARP_CALENDAR_LIBS])
AC_SUBST([GDATA_SHARP_CONTACTS_LIBS])
AC_SUBST([GDATA_SHARP_DOCUMENTS_LIBS])
AC_SUBST([GLADE_SHARP_20_LIBS])
AC_SUBST([GLIB_SHARP_20_LIBS])
AC_SUBST([GNOME_DESKTOP_SHARP_20_LIBS])
AC_SUBST([GNOME_KEYRING_SHARP_LIBS])
AC_SUBST([GNOME_SHARP_20_LIBS])
AC_SUBST([GNOME_VFS_SHARP_20_LIBS])
AC_SUBST([GTK_SHARP_20_LIBS])
AC_SUBST([MONO_ADDINS_LIBS])
AC_SUBST([MONO_ADDINS_GUI_LIBS])
AC_SUBST([MONO_ADDINS_SETUP_LIBS])
AC_SUBST([NDESK_DBUS_10_LIBS])
AC_SUBST([NDESK_DBUS_GLIB_10_LIBS])
AC_SUBST([NOTIFY_SHARP_LIBS])
AC_SUBST([WNCK_SHARP_10_LIBS])

dnl Do core references
PKG_CHECK_MODULES([DO_PLATFORM], [do.platform >= 0.8])
PKG_CHECK_MODULES([DO_PLATFORM_LINUX], [do.platform.linux >= 0.8])
PKG_CHECK_MODULES([DO_UNIVERSE], [do.universe >= 0.8])
PKG_CHECK_MODULES([DO_INTERFACE_WINK], [do.interface.wink >= 0.8])

AC_SUBST([DO_PLATFORM_LIBS])
AC_SUBST([DO_PLATFORM_LINUX_LIBS])
AC_SUBST([DO_UNIVERSE_LIBS])
AC_SUBST([DO_INTERFACE_WINK_LIBS])

AC_ARG_ENABLE(debug,
	AC_HELP_STRING([--enable-debug],
		[Use 'DEBUG' Configuration [default=YES]]),
		enable_debug=yes, enable_debug=no)
AM_CONDITIONAL(ENABLE_DEBUG, test x$enable_debug = xyes)
if test "x$enable_debug" = "xyes" ; then
	CONFIG_REQUESTED="yes"
fi
AC_ARG_ENABLE(release,
	AC_HELP_STRING([--enable-release],
		[Use 'RELEASE' Configuration [default=NO]]),
		enable_release=yes, enable_release=no)
AM_CONDITIONAL(ENABLE_RELEASE, test x$enable_release = xyes)
if test "x$enable_release" = "xyes" ; then
	CONFIG_REQUESTED="yes"
fi
if test -z "$CONFIG_REQUESTED" ; then
	AM_CONDITIONAL(ENABLE_DEBUG, true)
	enable_debug=yes
fi

AC_CONFIG_FILES([
Alias/Makefile
AptURL/Makefile
Archive/Makefile
Banshee/Makefile
Bibtex/Makefile
Cl.ickable/Makefile
ClawsMail/Makefile
Confluence/Makefile
Del.icio.us/Makefile
DiskMounter/Makefile
Emesene/Makefile
EOG-Slideshow/Makefile
Epiphany/Makefile
Evolution/Makefile
File/Makefile
Firefox/Makefile
Flickr/Makefile
GNOME-Dictionary/Makefile
GNOME-Screenshot/Makefile
GNOME-Session/Makefile
GNOME-Terminal/Makefile
GoogleCalculator/Makefile
GoogleCalendar/Makefile
GoogleContacts/Makefile
GoogleDocs/Makefile
GoogleMaps/Makefile
GoogleSearch/Makefile
ImageShack/Makefile
JIRA/Makefile
Launchpad/Makefile
LocateFiles/Makefile
ManLookUp/Makefile
Microblogging/Makefile
MPD/Makefile
NX/Makefile
OpenSearch/Makefile
Opera/Makefile
Pastebin/Makefile
Pidgin/Makefile
PingFM/Makefile
Putty/Makefile
Quote/Makefile
RememberTheMilk/Makefile
Rhythmbox/Makefile
Riptide/Makefile
RSS/Makefile
Shelf/Makefile
Skype/Makefile
SqueezeCenter/Makefile
SSH/Makefile
StockQuote/Makefile
RequestTracker/Makefile
SystemServices/Makefile
Tasque/Makefile
TerminalServerClient/Makefile
Text/Makefile
Thunderbird/Makefile
TinyUrl/Makefile
Tracker/Makefile
Translate/Makefile
Tomboy/Makefile
Vinagre/Makefile
VirtualBox/Makefile
VolumeControl/Makefile
WindowManager/Makefile
Woof/Makefile
Wordnet/Makefile
Xmms2/Makefile
YouTube/Makefile
Zim/Makefile
Makefile
po/Makefile.in
BundledLibraries/Makefile
])

AC_OUTPUT

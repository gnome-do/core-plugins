dnl Warning: This is an automatically generated file, do not edit!
dnl Process this file with autoconf to produce a configure script.
AC_PREREQ([2.54])
AC_INIT([DoPlugins], [0.1])
AM_INIT_AUTOMAKE([foreign])

dnl pkg-config
AC_PATH_PROG(PKG_CONFIG, pkg-config, no)
if test "x$PKG_CONFIG" = "xno"; then
        AC_MSG_ERROR([You need to install pkg-config])
fi

SHAMROCK_EXPAND_LIBDIR
SHAMROCK_EXPAND_BINDIR
SHAMROCK_EXPAND_DATADIR

AC_PROG_INSTALL

AC_PATH_PROG(GMCS, gmcs, no)
if test "x$GMCS" = "xno"; then
        AC_MSG_ERROR([gmcs Not found])
fi


AC_ARG_ENABLE(debug,
	AC_HELP_STRING([--enable-debug],
		[Use 'DEBUG' Configuration [default=NO]]),
		enable_debug=yes, enable_debug=no)
AM_CONDITIONAL(ENABLE_DEBUG, test x$enable_debug = xyes)
if test "x$enable_debug" = "xyes" ; then
	CONFIG_REQUESTED="yes"
fi
AC_ARG_ENABLE(release,
	AC_HELP_STRING([--enable-release],
		[Use 'RELEASE' Configuration [default=YES]]),
		enable_release=yes, enable_release=no)
AM_CONDITIONAL(ENABLE_RELEASE, test x$enable_release = xyes)
if test "x$enable_release" = "xyes" ; then
	CONFIG_REQUESTED="yes"
fi
if test -z "$CONFIG_REQUESTED" ; then
	AM_CONDITIONAL(ENABLE_RELEASE, true)
	enable_release=yes
fi


dnl package checks, common for all configs
PKG_CHECK_MODULES([GNOME_DESKTOP_SHARP_20], [gnome-desktop-sharp-2.0])
PKG_CHECK_MODULES([NDESK_DBUS_10], [ndesk-dbus-1.0])
PKG_CHECK_MODULES([GNOME_VFS_SHARP_20], [gnome-vfs-sharp-2.0])
PKG_CHECK_MODULES([FLICKRNET], [flickrnet])
PKG_CHECK_MODULES([EVOLUTION_SHARP], [evolution-sharp])
PKG_CHECK_MODULES([WNCK_SHARP_10], [wnck-sharp-1.0])
PKG_CHECK_MODULES([GCONF_SHARP_20], [gconf-sharp-2.0])
PKG_CHECK_MODULES([GTK_SHARP_20], [gtk-sharp-2.0])
PKG_CHECK_MODULES([NDESK_DBUS_GLIB_10], [ndesk-dbus-glib-1.0])
PKG_CHECK_MODULES([GLIB_SHARP_20], [glib-sharp-2.0])

dnl package checks, per config


AC_CONFIG_FILES([
Twitter/twitter.pc
GNOME-Terminal/Makefile
GoogleSearch/inlinegooglesearch.pc
Text/Makefile
RSS/Makefile
Pastebin/pastebin.pc
Text/text.pc
Vinagre/vinagre.pc
Tomboy/Makefile
VolumeControl/volumecontrol.pc
SSH/ssh.pc
ManLookUp/manlookup.pc
GCalendar/Makefile
LocateFiles/Makefile
Riptide/Makefile
OpenSearch/opensearch.pc
Tasque/tasque.pc
GoogleCalculator/googlecalculator.pc
WindowManager/windowmanager.pc
Tasque/Makefile
GNOME-Dictionary/gnome-dictionary.pc
GNOME-Session/gnome-session.pc
Banshee/banshee.pc
ManLookUp/Makefile
Pastebin/Makefile
Archive/archive.pc
Rhythmbox/Makefile
Cl.ickable/cl.ickable.pc
GoogleCalculator/Makefile
Shelf/Makefile
GMailContacts/gmailcontacts.pc
WindowManager/Makefile
GoogleMaps/Makefile
RSS/rss.pc
SSH/Makefile
Banshee/Makefile
Shelf/shelf.pc
del.icio.us/Makefile
GoogleMaps/googlemaps.pc
GNOME-Screenshot/gnome-screenshot.pc
File/file.pc
Cl.ickable/Makefile
GoogleSearch/Makefile
Riptide/riptide.pc
Launchpad/Makefile
Amarok/amarok.pc
GNOME-Screenshot/Makefile
ImageShack/imageshack.pc
Archive/Makefile
GMailContacts/Makefile
VolumeControl/Makefile
GNOME-Terminal/gnome-terminal.pc
Evolution/Makefile
GDocs/gdocs.pc
Flickr/flickr.pc
Vinagre/Makefile
Amarok/Makefile
Firefox/firefox.pc
Evolution/evolution.pc
Epiphany/Makefile
OpenSearch/Makefile
GNOME-Session/Makefile
Flickr/Makefile
File/Makefile
Epiphany/epiphany.pc
Pidgin/pidgin.pc
Rhythmbox/rhythmbox.pc
ImageShack/Makefile
GDocs/Makefile
Tomboy/tomboy.pc
LocateFiles/locatefiles.pc
Pidgin/Makefile
Launchpad/launchpad.pc
Twitter/Makefile
DiskMounter/Makefile
DiskMounter/diskmounter.pc
del.icio.us/del.icio.us.pc
Makefile
GNOME-Dictionary/Makefile
GCalendar/gcalendar.pc
Firefox/Makefile

])

AC_OUTPUT

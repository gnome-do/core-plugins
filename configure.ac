dnl Warning: This is an automatically generated file, do not edit!
dnl Process this file with autoconf to produce a configure script.
AC_PREREQ([2.54])
AC_INIT([DoPlugins], [0.4.9.9])
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


dnl package checks, common for all configs
PKG_CHECK_MODULES([GNOME_DESKTOP_SHARP_20], [gnome-desktop-sharp-2.0])
PKG_CHECK_MODULES([WNCK_SHARP_10], [wnck-sharp-1.0])
PKG_CHECK_MODULES([GNOME_VFS_SHARP_20], [gnome-vfs-sharp-2.0])
PKG_CHECK_MODULES([GLIB_SHARP_20], [glib-sharp-2.0])
PKG_CHECK_MODULES([GTK_SHARP_20], [gtk-sharp-2.0])
PKG_CHECK_MODULES([GCONF_SHARP_20], [gconf-sharp-2.0])
PKG_CHECK_MODULES([DOADDINS], [do.addins])
PKG_CHECK_MODULES([NDESK_DBUS_10], [ndesk-dbus-1.0])
PKG_CHECK_MODULES([EVOLUTION_SHARP], [evolution-sharp])
PKG_CHECK_MODULES([FLICKRNET], [flickrnet])
PKG_CHECK_MODULES([NDESK_DBUS_GLIB_10], [ndesk-dbus-glib-1.0])

dnl package checks, per config


AC_CONFIG_FILES([
Twitter/twitter.pc
Tasque/Makefile
GNOME-Screenshot/Makefile
GCalendar/Makefile
Banshee/Makefile
Do.Music/Makefile
Pastebin/pastebin.pc
Do.Music/DoMusicBanshee/Makefile
Text/text.pc
Vinagre/vinagre.pc
VolumeControl/volumecontrol.pc
SSH/ssh.pc
Archive/Makefile
DiskMounter/Makefile
OpenSearch/opensearch.pc
Tasque/tasque.pc
Vinagre/Makefile
GoogleCalculator/googlecalculator.pc
WindowManager/windowmanager.pc
Do.Music/DoMusicMPD/Makefile
SSH/Makefile
GNOME-Dictionary/gnome-dictionary.pc
GNOME-Session/gnome-session.pc
Banshee/banshee.pc
Do.Music/DoMusicBanshee/domusicbanshee.pc
GNOME-Dictionary/Makefile
Rhythmbox/Makefile
Archive/archive.pc
WindowManager/Makefile
Launchpad/Makefile
Twitter/Makefile
Pastebin/Makefile
Amarok/Makefile
GMailContacts/gmailcontacts.pc
Firefox/Makefile
RSS/rss.pc
GoogleMaps/Makefile
Flickr/Makefile
LocateFiles/Makefile
Evolution/Makefile
Do.Music/DoMusicAmarok/domusicamarok.pc
Shelf/shelf.pc
GoogleMaps/googlemaps.pc
GNOME-Screenshot/gnome-screenshot.pc
File/Makefile
File/file.pc
Riptide/riptide.pc
GMailContacts/Makefile
Pidgin/Makefile
Amarok/amarok.pc
VolumeControl/Makefile
Do.Music/DoMusicAmarok/Makefile
Shelf/Makefile
GNOME-Terminal/gnome-terminal.pc
GNOME-Terminal/Makefile
GNOME-Session/Makefile
Flickr/flickr.pc
Riptide/Makefile
Firefox/firefox.pc
Evolution/evolution.pc
OpenSearch/Makefile
Text/Makefile
Epiphany/epiphany.pc
Pidgin/pidgin.pc
Epiphany/Makefile
Rhythmbox/rhythmbox.pc
Tomboy/Makefile
Tomboy/tomboy.pc
GoogleCalculator/Makefile
LocateFiles/locatefiles.pc
Launchpad/launchpad.pc
RSS/Makefile
DiskMounter/diskmounter.pc
Makefile
GCalendar/gcalendar.pc
Do.Music/DoMusicMPD/domusicmpd.pc

])

AC_OUTPUT

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
		[Use 'DEBUG' Configuration [default=NO]]),
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
AC_ARG_ENABLE(repo,
	AC_HELP_STRING([--enable-repo],
		[Use 'REPO' Configuration [default=YES]]),
		enable_repo=yes, enable_repo=no)
AM_CONDITIONAL(ENABLE_REPO, test x$enable_repo = xyes)
if test "x$enable_repo" = "xyes" ; then
	CONFIG_REQUESTED="yes"
fi
if test -z "$CONFIG_REQUESTED" ; then
	AM_CONDITIONAL(ENABLE_REPO, true)
	enable_repo=yes
fi


dnl package checks, common for all configs
PKG_CHECK_MODULES([DODBUS], [do.dbus])
PKG_CHECK_MODULES([WNCK_SHARP_10], [wnck-sharp-1.0])
PKG_CHECK_MODULES([NDESK_DBUS_10], [ndesk-dbus-1.0])
PKG_CHECK_MODULES([GCONF_SHARP_20], [gconf-sharp-2.0])
PKG_CHECK_MODULES([EVOLUTION_SHARP], [evolution-sharp])
PKG_CHECK_MODULES([GNOME_DESKTOP_SHARP_20], [gnome-desktop-sharp-2.0])
PKG_CHECK_MODULES([GTK_SHARP_20], [gtk-sharp-2.0])
PKG_CHECK_MODULES([NDESK_DBUS_GLIB_10], [ndesk-dbus-glib-1.0])
PKG_CHECK_MODULES([DOADDINS], [do.addins])
PKG_CHECK_MODULES([GLIB_SHARP_20], [glib-sharp-2.0])

dnl package checks, per config

AC_CONFIG_FILES([
    Amarok/Makefile
    Amarok/amarok.pc
    Banshee/Makefile
    Banshee/banshee.pc
    Epiphany/Makefile
    Epiphany/epiphany.pc
    Evolution/Makefile
    Evolution/evolution.pc
    File/Makefile
    File/file.pc
    Firefox/Makefile
    Firefox/firefox.pc
    GCalendar/Makefile
    GCalendar/gcalendar.pc
    GNOME-Screenshot/Makefile
    GNOME-Screenshot/gnome-screenshot.pc
    GNOME-Session/Makefile
    GNOME-Session/gnome-session.pc
    GoogleCalculator/Makefile
    GoogleCalculator/googlecalculator.pc
    Launchpad/Makefile
    Launchpad/launchpad.pc
    LocateFiles/Makefile
    LocateFiles/locatefiles.pc
    Makefile
    OpenSearch/Makefile
    OpenSearch/opensearch.pc
    Pastebin/Makefile
    Pastebin/pastebin.pc
    Pidgin/Makefile
    Pidgin/pidgin.pc
    Rhythmbox/Makefile
    Rhythmbox/rhythmbox.pc
    SSH/Makefile
    SSH/ssh.pc
    SimplePlugins/Makefile
    SimplePlugins/simpleplugins.pc
    Tasque/Makefile
    Tasque/tasque.pc
    Thunderbird/Makefile
    Thunderbird/thunderbird.pc
    Tomboy/Makefile
    Tomboy/tomboy.pc
    Twitter/Makefile
    Twitter/twitter.pc
    Vinagre/Makefile
    Vinagre/vinagre.pc
    WindowManager/Makefile
    WindowManager/windowmanager.pc
])

AC_OUTPUT

dnl Warning: This is an automatically generated file, do not edit!
dnl Process this file with autoconf to produce a configure script.
AC_PREREQ([2.54])
AC_INIT([Do Plugins], [0.4.0.1])
AM_INIT_AUTOMAKE([foreign])
ASM_VERSION="$VERSION"
AC_SUBST(ASM_VERSION)


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
PKG_CHECK_MODULES([GTK_SHARP_20], [gtk-sharp-2.0])
PKG_CHECK_MODULES([NDESK_DBUS_GLIB_10], [ndesk-dbus-glib-1.0])
PKG_CHECK_MODULES([DODBUS], [do.dbus])
PKG_CHECK_MODULES([EVOLUTION_SHARP], [evolution-sharp])
PKG_CHECK_MODULES([NDESK_DBUS_10], [ndesk-dbus-1.0])
PKG_CHECK_MODULES([GCONF_SHARP_20], [gconf-sharp-2.0])
PKG_CHECK_MODULES([DOADDINS], [do.addins])

dnl package checks, per config


AC_CONFIG_FILES([
Twitter/twitter.pc
GoogleCalculator/Makefile
Banshee/Makefile
Epiphany/Makefile
Pastebin/pastebin.pc
LocateFiles/Makefile
SSH/ssh.pc
OpenSearch/opensearch.pc
GoogleCalculator/googlecalculator.pc
Rhythmbox/Makefile
GNOME-Session/gnome-session.pc
Amarok/Makefile
Banshee/banshee.pc
Makefile
OpenSearch/Makefile
Launchpad/Makefile
Pidgin/Makefile
File/file.pc
Amarok/amarok.pc
Evolution/Makefile
SimplePlugins/Makefile
Twitter/Makefile
SSH/Makefile
GNOME-Session/Makefile
Pastebin/Makefile
Thunderbird/Makefile
Tomboy/Makefile
Evolution/evolution.pc
Epiphany/epiphany.pc
Pidgin/pidgin.pc
Rhythmbox/rhythmbox.pc
Thunderbird/thunderbird.pc
Tomboy/tomboy.pc
LocateFiles/locatefiles.pc
SimplePlugins/simpleplugins.pc
Launchpad/launchpad.pc
File/Makefile
Amarok/src/AssemblyInfo.cs
Banshee/src/AssemblyInfo.cs 
Evolution/src/AssemblyInfo.cs
LocateFiles/src/AssemblyInfo.cs
Pidgin/src/AssemblyInfo.cs
Rhythmbox/src/AssemblyInfo.cs
Thunderbird/src/AssemblyInfo.cs
Launchpad/src/AssemblyInfo.cs 
OpenSearch/src/AssemblyInfo.cs
Pastebin/src/AssemblyInfo.cs
Amarok/Amarok.mdp
Banshee/Banshee.mdp
Epiphany/Epiphany.mdp
Evolution/Evolution.mdp
File/File.mdp
GNOME-Session/GNOME-Session.mdp
GoogleCalculator/GoogleCalculator.mdp
Launchpad/Launchpad.mdp
LocateFiles/LocateFiles.mdp
OpenSearch/OpenSearch.mdp
Pastebin/Pastebin.mdp
Pidgin/Pidgin.mdp
Rhythmbox/Rhythmbox.mdp
SSH/SSH.mdp
SimplePlugins/SimplePlugins.mdp
Templates/Templates.mdp
Thunderbird/Thunderbird.mdp
Tomboy/Tomboy.mdp
Twitter/Twitter.mdp

])

AC_OUTPUT

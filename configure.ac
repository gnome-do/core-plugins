AC_PREREQ([2.54])
AC_INIT([Do-plugins], [0.3.0])

AM_INIT_AUTOMAKE([-Wall -Werror foreign])
AM_MAINTAINER_MODE

AC_PROG_INSTALL

dnl pkg-config
AC_PATH_PROG(PKG_CONFIG, pkg-config, no)
if test "x$PKG_CONFIG" = "xno"; then
        AC_MSG_ERROR([You need to install pkg-config])
fi

dnl Handle various possible C# compilers
AC_PATH_PROG(CSC, csc, no)
AC_PATH_PROG(GMCS, gmcs, no)
AC_PATH_PROG(RUNTIME, mono, no)
CS="C#"

if test "x$CSC" = "xno" ; then
        if test "x$GMCS" = "xno" ; then
                if test "x$MCS" = "xno" ; then
                        AC_MSG_ERROR([No $CS compiler found])
                else
                        CSC=$MCS
                        ENABLE_DBUS="no"
                fi
        else
                CSC=$GMCS
        fi
fi

AC_SUBST(CSC)
AC_SUBST(RUNTIME)

dnl - Check dependencies

PKG_CHECK_MODULES([DO_ADDINS], [do.addins])
PKG_CHECK_MODULES([DO_DBUS], [do.dbus])
PKG_CHECK_MODULES([EVOLUTION_SHARP], [evolution-sharp])

AC_SUBST(DO_ADDINS_LIBS)
AC_SUBST(DO_DBUS_LIBS)
AC_SUBST(EVOLUTION_SHARP_LIBS)

dnl Plugin install paths

AC_ARG_WITH(plugindir, [  --with-plugindir=DIR    Plugin install path [$datadir/gnome-do/plugins] ],
		       [plugindir="$withval"], 
		       [plugindir="$datadir/gnome-do/plugins"])

AC_SUBST(plugindir)

CSFLAGS=

AC_SUBST(CSFLAGS)

AC_CONFIG_FILES([
	Makefile
	GNOME-Session/Makefile
	Evolution/Makefile
	Evolution/src/Makefile
	Epiphany/Makefile
	Amarok/Makefile
	Rhythmbox/Makefile
	Rhythmbox/src/Makefile
	Pidgin/Makefile
	Pidgin/src/Makefile
	LocateFiles/Makefile
	Thunderbird/Makefile
	Thunderbird/src/Makefile
	GoogleCalculator/Makefile
	OpenSearch/Makefile
	OpenSearch/src/Makefile
	Pastebin/Makefile
	Pastebin/src/Makefile
	SimplePlugins/Makefile
	Templates/Makefile
])

AC_OUTPUT

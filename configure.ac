AC_PREREQ(2.61)
AC_INIT([Do-plugins],[0.4.0])

AM_INIT_AUTOMAKE([-Wall -Werror foreign])
AM_MAINTAINER_MODE

AC_PROG_INSTALL

dnl pkg-config
AC_PATH_PROG(PKG_CONFIG, pkg-config, no)
if test "x$PKG_CONFIG" = "xno"; then
        AC_MSG_ERROR([You need to install pkg-config])
fi

dnl Handle Boo compiler
AC_PATH_PROG(BOOC, booc, no)
BOO="Boo"

if test "x$BOOC" = "xno" ; then
	AC_MSG_ERROR([No $BOO compiler found])
else
	BOOC=$BOOC
fi

AC_SUBST(BOOC)

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

AC_SUBST(DO_ADDINS_LIBS)
AC_SUBST(DO_DBUS_LIBS)


dnl - Evolution plugin

AC_ARG_ENABLE([evolution-plugin],
	AS_HELP_STRING([--enable-evolution-plugin],[Enable the Evolution plugin]),
	enable_evo_plugin=$enableval,
	enable_evo_plugin=auto)

if test "x$enable_evo_plugin" != "xno" ; then
PKG_CHECK_MODULES([EVOLUTION_SHARP],
		  [evolution-sharp],
		  has_evo_deps=yes,
		  has_evo_deps=no)
AC_SUBST(EVOLUTION_SHARP_LIBS)
fi

if test "x$has_evo_deps" = "xno" ; then
	if test "x$enable_evo_plugin" = "xyes" ; then
		dnl Error out if explicitly asked for the Evolution plugin
		AC_MSG_ERROR([Could not find evolution-sharp])
	fi
	enable_evo_plugin="no"
else
	if test "x$enable_evo_plugin" = "xauto" ; then
		enable_evo_plugin="yes"
	fi
fi

AM_CONDITIONAL(ENABLE_EVO_PLUGIN, test "x$enable_evo_plugin" = "xyes")

dnl - Amarok plugin
AC_ARG_ENABLE([amarok-plugin],
	AS_HELP_STRING([--enable-amarok-plugin],[Enable the Amarok plugin]),
	enable_amarok_plugin=$enableval,
	enable_amarok_plugin=yes)

AM_CONDITIONAL(ENABLE_AMAROK_PLUGIN, test "x$enable_amarok_plugin" = "xyes")

dnl - OpenSearch plugin
AC_ARG_ENABLE([opensearch-plugin],
	AS_HELP_STRING([--enable-opensearch-plugin],[Enable the OpenSearch plugin]),
	enable_opensearch_plugin=$enableval,
	enable_opensearch_plugin=yes)

AM_CONDITIONAL(ENABLE_OPENSEARCH_PLUGIN, 
               test "x$enable_opensearch_plugin" = "xyes")

dnl - Pastebin plugin
AC_ARG_ENABLE([pastebin-plugin],
	AS_HELP_STRING([--enable-pastebin-plugin],[Enable the Pastebin plugin]),
	enable_pastebin_plugin=$enableval,
	enable_pastebin_plugin=yes)

AM_CONDITIONAL(ENABLE_PASTEBIN_PLUGIN, test "x$enable_pastebin_plugin" = "xyes")

dnl - Banshee plugin
AC_ARG_ENABLE([banshee-plugin],
	AS_HELP_STRING([--enable-banshee-plugin],[Enable the Banshee plugin]),
	enable_banshee_plugin=$enableval,
	enable_banshee_plugin=auto)

if test "x$enable_banshee_plugin" != "xno" ; then
PKG_CHECK_MODULES([NDESK_DBUS],
		  [ndesk-dbus-1.0 ndesk-dbus-glib-1.0],
		  has_banshee_deps=yes,
		  has_banshee_deps=no)
AC_SUBST(NDESK_DBUS_LIBS)
fi

if test "x$has_banshee_deps" = "xno" ; then
	if test "x$enable_banshee_plugin" = "xyes" ; then
		dnl Error out if explicitly asked for the BANSHEE plugin
		AC_MSG_ERROR([Could not find ndesk-dbus or ndesk-dbus-glib])
	fi
	enable_banshee_plugin="no"
else
	if test "x$enable_banshee_plugin" = "xauto" ; then
		enable_banshee_plugin="yes"
	fi
fi

AM_CONDITIONAL(ENABLE_BANSHEE_PLUGIN, test "x$enable_banshee_plugin" = "xyes")


dnl - SSH plugin
AC_ARG_ENABLE([ssh-plugin],
	AS_HELP_STRING([--enable-ssh-plugin],[Enable the SSH plugin]),
	enable_ssh_plugin=$enableval,
	enable_ssh_plugin=yes)

if test "x$enable_ssh_plugin" != "xno" ; then
PKG_CHECK_MODULES([GCONF_SHARP],
		  [gconf-sharp-2.0],
		  has_ssh_deps=yes,
		  has_ssh_deps=no)
AC_SUBST(GCONF_SHARP_LIBS)
fi

if test "x$has_ssh_deps" = "xno" ; then
	if test "x$enable_ssh_plugin" = "xyes" ; then
		dnl Error out if explicitly asked for the SSH plugin
		AC_MSG_ERROR([Could not find gconf-sharp-2.0])
	fi
	enable_ssh_plugin="no"
else
	if test "x$enable_ssh_plugin" = "xauto" ; then
		enable_ssh_plugin="yes"
	fi
fi

AM_CONDITIONAL(ENABLE_SSH_PLUGIN, test "x$enable_ssh_plugin" = "xyes")

dnl - Launchpad plugin
AC_ARG_ENABLE([launchpad-plugin],
	AS_HELP_STRING([--enable-launchpad-plugin],[Enable the Launchpad plugin]),
	enable_launchpad_plugin=$enableval,
	enable_launchpad_plugin=yes)

AM_CONDITIONAL(ENABLE_LAUNCHPAD_PLUGIN, test "x$enable_launchpad_plugin" = "xyes")

dnl - Tasque plugin
AC_ARG_ENABLE([tasque-plugin],
        AS_HELP_STRING([--enable-tasque-plugin],[Enable the Tasque plugin]),
        enable_tasque_plugin=$enableval,
        enable_tasque_plugin=yes)

AM_CONDITIONAL(ENABLE_TASQUE_PLUGIN, test "x$enable_tasque_plugin" = "xyes")

dnl - Tomboy plugin
AC_ARG_ENABLE([tomboy-plugin],
	AS_HELP_STRING([--enable-tomboy-plugin],[Enable the Tomboy plugin]),
	enable_tomboy_plugin=$enableval,
	enable_tomboy_plugin=yes)

AM_CONDITIONAL(ENABLE_TOMBOY_PLUGIN, test "x$enable_tomboy_plugin" = "xyes")

dnl - Twitter plugin
AC_ARG_ENABLE([twitter-plugin],
        AS_HELP_STRING([--enable-twitter-plugin],[Enable the Twitter plugin]),
        enable_twitter_plugin=$enableval,
        enable_twitter_plugin=yes)

AM_CONDITIONAL(ENABLE_TWITTER_PLUGIN, test "x$enable_twitter_plugin" = "xyes")



dnl Plugin install paths

AC_ARG_WITH(plugindir, 
            AS_HELP_STRING([--with-plugindir=DIR],[Plugin install path [$datadir/gnome-do/plugins]]),
            [plugindir="$withval"], 
	    [plugindir="$datadir/gnome-do/plugins"])

AC_SUBST(plugindir)

CSFLAGS=

AC_SUBST(CSFLAGS)

AC_CONFIG_FILES([
	Makefile
	Amarok/Makefile
	Banshee/Makefile
	Banshee/src/Makefile
	Epiphany/Makefile
	Evolution/Makefile
	Evolution/src/Makefile
	File/Makefile
	GNOME-Screenshot/Makefile
	GNOME-Session/Makefile
	GNOME-Terminal/Makefile
	GoogleCalculator/Makefile
	Launchpad/Makefile
	Launchpad/Resources/Makefile
	Launchpad/src/Makefile
	LocateFiles/Makefile
	OpenSearch/Makefile
	OpenSearch/src/Makefile
	Pastebin/Makefile
	Pastebin/src/Makefile
	Pidgin/Makefile
	Pidgin/src/Makefile
	Rhythmbox/Makefile
	Rhythmbox/src/Makefile
	SSH/Makefile
	SimplePlugins/Makefile
	Tasque/Makefile
	Templates/Makefile
	Thunderbird/Makefile
	Thunderbird/src/Makefile
	Tomboy/Makefile
	Twitter/Makefile
])

AC_OUTPUT

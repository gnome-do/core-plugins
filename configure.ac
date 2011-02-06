dnl vim: set sw=4 sts=4 ts=4 noet ft=config foldmethod=marker foldmarker={{{,}}} :

dnl Process this file with autoconf to produce a configure script.
AC_PREREQ([2.54])
AC_INIT([gnome-do-plugins], [0.8.2])
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

AC_PROG_INSTALL

top_srcdir=$(readlink -f $0 | sed -e s/configure$//)
SHAMROCK_FIND_C_SHARP_3_0_COMPILER

dnl Expansions
SHAMROCK_EXPAND_LIBDIR
SHAMROCK_EXPAND_BINDIR
SHAMROCK_EXPAND_DATADIR

dnl {{{ check for Banshee
AC_ARG_ENABLE(banshee, 
	AC_HELP_STRING([--enable-banshee], [Build plugin to interact with Banshee (default: auto)]),
	enable_banshee=$enableval,
	enable_banshee=auto)

if test "x$enable_banshee" != "xno"; then
	PKG_CHECK_MODULES([BANSHEE_INDEXER], [banshee-1-collection-indexer >= 1.4.2], HAVE_BANSHEE=yes, HAVE_BANSHEE=no)

	if test "x$enable_banshee" = "xyes" -a "x$HAVE_BANSHEE" = "xno"; then
		AC_MSG_ERROR([Banshee support explicitly requested but dependencies not found])
	fi
fi

AM_CONDITIONAL(HAVE_BANSHEE, test "x$HAVE_BANSHEE" = "xyes")

dnl We need to copy Banshee.CollectionIndexer.dll; it's not in the GAC
BANSHEE_INDEXER_DLL=$($PKG_CONFIG --variable=Libraries banshee-1-collection-indexer)

AC_SUBST([BANSHEE_INDEXER_DLL])
dnl }}}

dnl {{{ check for Evolution
AC_ARG_ENABLE(evolution,
	AC_HELP_STRING([--enable-evolution], [Build plugin to interact with Evolution (default: auto)]),
	enable_evolution=$enableval,
	enable_evolution=auto)

if test "x$enable_evolution" != "xno"; then
	PKG_CHECK_MODULES([EVOLUTION_SHARP], [evolution-sharp], HAVE_EVOLUTION=yes, HAVE_EVOLUTION=no)

	if test "x$enable_evolution" = "xyes" -a "x$HAVE_EVOLUTION" = "xno"; then
		AC_MSG_ERROR([Evolution support explicitly requestion but dependencies not found])
	fi
fi

AM_CONDITIONAL(HAVE_EVOLUTION, test "x$HAVE_EVOLUTION" = "xyes")

AC_SUBST([EVOLUTION_SHARP_LIBS])
dnl }}}

dnl {{{ check for Flickr
AC_ARG_ENABLE(flickr,
	AC_HELP_STRING([--enable-flickr], [Build plugin to interact with flickr using flickrnet (default: auto)]),
	enable_flickr=$enableval,
	enable_flickr=auto)

if test "x$enable_flickr" != "xno"; then
	PKG_CHECK_MODULES([FLICKRNET], [flickrnet], HAVE_FLICKR=yes, HAVE_FLICKR=no)

	if test "x$enable_flickr" = "xyes" -a "x$HAVE_FLICKR" = "xno"; then
		AC_MSG_ERROR([Flickr support explicitly requested but dependencies not found])
	fi
fi

AM_CONDITIONAL(HAVE_FLICKR, test "x$HAVE_FLICKR" = "xyes")

AC_SUBST([FLICKRNET_LIBS])
dnl }}}

dnl {{{ check for Google Calendar libraries
AC_ARG_ENABLE(gcal,
	AC_HELP_STRING([--enable-gcal], [Build plugin to interact with google calendar (default: auto)]),
	enable_gcal=$enableval,
	enable_gcal=auto)

if test "x$enable_gcal" != "xno"; then
	PKG_CHECK_MODULES([GDATA_SHARP_CALENDAR], [gdata-sharp-acl gdata-sharp-core gdata-sharp-calendar], HAVE_GCAL=yes, HAVE_GCAL=no)

	if test "x$enable_gcal" = "xyes" -a "x$HAVE_GCAL" = "xno"; then
		AC_MSG_ERROR([Google Calendar support explicitly requested but dependencies not found])
	fi
fi

AM_CONDITIONAL(HAVE_GCAL, test "x$HAVE_GCAL" = "xyes")

AC_SUBST([GDATA_SHARP_CALENDAR_LIBS])
dnl}}}

dnl{{{ check for Google Contact libraries
AC_ARG_ENABLE(gmail,
	AC_HELP_STRING([--enable-gmail], [Build plugin to interact with GMail contacts (defualt: auto)]),
	enable_gmail=$enableval,
	enable_gmail=auto)

if test "x$enable_gcal" != "xno"; then
	PKG_CHECK_MODULES([GDATA_SHARP_CONTACTS], [gdata-sharp-acl gdata-sharp-core gdata-sharp-contacts ], HAVE_GMAIL=yes, HAVE_GMAIL=no)

	if test "x$enable_gmail" = "xyes" -a "x$HAVE_GMAIL" = "xno"; then
   		AC_MSG_ERROR([GMail contacts support explicitly requested but dependencies not found])
	fi
fi

AM_CONDITIONAL(HAVE_GMAIL, test "x$HAVE_GMAIL" = "xyes")

AC_SUBST([GDATA_SHARP_CONTACTS_LIBS])
dnl}}}

dnl{{{ check for Google Documents libraries
AC_ARG_ENABLE(gdocs,
	AC_HELP_STRING([--enable-gdocs], [Build plugin to interact with Google Documents (default: auto]),
	enable_gdocs=$enableval,
	enable_gdocs=auto)

if test "x$enable_gdocs" != "xno"; then
	PKG_CHECK_MODULES([GDATA_SHARP_DOCUMENTS], [gdata-sharp-acl gdata-sharp-core gdata-sharp-documents], HAVE_GDOCS=yes, HAVE_GDOCS=no)
   
	if test "x$enable_gdocs" = "xyes" -a "x$HAVE_GDOCS" = "no"; then
   		AC_MSG_ERROR([Google Documents support explicitly requested but dependencies not found])
   	fi
fi

AM_CONDITIONAL(HAVE_GDOCS, test "x$HAVE_GDOCS" = "xyes")

AC_SUBST([GDATA_SHARP_DOCUMENTS_LIBS])
dnl}}}

dnl{{{ check for YouTube libraries
AC_ARG_ENABLE(youtube,
	AC_HELP_STRING([--enable-youtube], [Build plugin to interact with YouTube (default: auto]),
	enable_youtube=$enableval,
	enable_youtube=auto)

if test "x$enable_youtube" != "xno"; then
   	PKG_CHECK_MODULES([GDATA_SHARP_YOUTUBE], [gdata-sharp-youtube], HAVE_YOUTUBE=yes, HAVE_YOUTUBE=no)

   	if test "x$enable_youtube" = "xyes" -a "x$HAVE_YOUTUBE" = "no"; then
   		AC_MSG_ERROR([YouTube support explicitly requested but dependencies not found])
	fi
fi

AM_CONDITIONAL(HAVE_YOUTUBE, test "x$HAVE_YOUTUBE" = "xyes")

AC_SUBST([GDATA_SHARP_YOUTUBE_LIBS])
dnl}}}

dnl {{{ check for Wnck libraries
AC_ARG_ENABLE(wnck,
	AC_HELP_STRING([--enable-window-manager], [Build plugin to control window (default: auto]),
	enable_wnck=$enableval,
	enable_wnck=auto)

if test "x$enable_wnck" != "xno"; then
   	PKG_CHECK_MODULES([WNCK_SHARP_10], [wnck-sharp-1.0], HAVE_WNCK=yes, HAVE_WNCK=no)

	if test "x$enable_wnck" = "xyes" -a "x$HAVE_WNCK" = "no"; then
   		AC_MSG_ERROR([Window manager support explicitly requested but dependencies not found])
	fi
fi

AM_CONDITIONAL(HAVE_WNCK, test "x$HAVE_WNCK" = "xyes")

AC_SUBST([WNCK_SHARP_10_LIBS])
dnl}}}

dnl {{{
# Temporarily check for dbus-sharp only in Empathy build
# The DBus-Sharp fork is relatively new, and probably not ubiquitous yet.
AC_ARG_ENABLE(empathy,
	AC_HELP_STRING([--enable-empathy], [Build plugin to control window (default: auto]),
	enable_empathy=$enableval,
	enable_empathy=auto)

if test "x$enable_empathy" != "xno"; then
   	PKG_CHECK_MODULES([DBUS_SHARP], [dbus-sharp-1.0], HAVE_EMPATHY=yes, HAVE_EMPATHY=no)
	if test "x$HAVE_EMPATHY" == "xyes" ; then
		PKG_CHECK_MODULES([DBUS_SHARP_GLIB], [dbus-sharp-glib-1.0], HAVE_EMPATHY=yes, HAVE_EMPATHY=no)
	fi

	if test "x$enable_empathy" = "xyes" -a "x$HAVE_EMPATHY" = "no"; then
   		AC_MSG_ERROR([Empathy plugin explicitly requested but dbus-sharp dependencies not found])
	fi
fi
AC_SUBST([DBUS_SHARP_LIBS])
AC_SUBST([DBUS_SHARP_GLIB_LIBS])

AM_CONDITIONAL(HAVE_EMPATHY, test "x$HAVE_EMPATHY" = "xyes")

# We need a bundled telepathy-sharp until there's an actual upstream for it
TELEPATHY_SHARP_LIBS=-r:$(pwd)/build/telepathy-sharp.dll
AC_SUBST([TELEPATHY_SHARP_LIBS])
dnl }}}

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


AC_SUBST([GCONF_SHARP_20_LIBS])
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

dnl Do core references
PKG_CHECK_MODULES([DO_PLATFORM], [do.platform >= 0.8])
PKG_CHECK_MODULES([DO_PLATFORM_LINUX], [do.platform.linux >= 0.8])
PKG_CHECK_MODULES([DO_UNIVERSE], [do.universe >= 0.8])

AC_SUBST([DO_PLATFORM_LIBS])
AC_SUBST([DO_PLATFORM_LINUX_LIBS])
AC_SUBST([DO_UNIVERSE_LIBS])

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
Alias/Resources/Alias.addin.xml
AptURL/Makefile
AptURL/Resources/AptURL.addin.xml
Archive/Makefile
Archive/Resources/Archive.addin.xml
Baconator/Makefile
Baconator/Resources/Baconator.addin.xml
Banshee/Makefile
Banshee/Resources/Banshee.addin.xml
Bibtex/Makefile
Bibtex/Resources/Bibtex.addin.xml
Chromium/Makefile
Chromium/Resources/Chromium.addin.xml
Cl.ickable/Makefile
Cl.ickable/Resources/Cl.ickable.addin.xml
ClawsMail/Makefile
ClawsMail/Resources/ClawsMail.addin.xml
Confluence/Makefile
Confluence/Resources/Confluence.addin.xml
Del.icio.us/Makefile
Del.icio.us/Resources/delicious.addin.xml
DiskMounter/Makefile
DiskMounter/Resources/DiskMounter.addin.xml
Dropbox/Makefile
Dropbox/Resources/Dropbox.addin.xml
Emesene/Makefile
Emesene/Resources/Emesene.addin.xml
Empathy/Makefile
Empathy/Resources/Empathy.addin.xml
EOG-Slideshow/Makefile
EOG-Slideshow/Resources/EOG-Slideshow.addin.xml
Epiphany/Makefile
Epiphany/Resources/Epiphany.addin.xml
Evolution/Makefile
Evolution/Resources/Evolution.addin.xml
Exaile/Makefile
Exaile/Resources/Exaile.addin.xml
File/Makefile
File/Resources/File.addin.xml
Firefox/Makefile
Firefox/Resources/Firefox.addin.xml
Flickr/Makefile
Flickr/Resources/Flickr.addin.xml
GNOME-Calculator/Makefile
GNOME-Calculator/Resources/GNOME-Calculator.addin.xml
GNOME-Dictionary/Makefile
GNOME-Dictionary/Resources/GNOME-Dictionary.addin.xml
GNOME-Screenshot/Makefile
GNOME-Screenshot/Resources/GNOME-Screenshot.addin.xml
GNOME-Session/Makefile
GNOME-Session/Resources/GNOME-Session.addin.xml
GNOME-Terminal/Makefile
GNOME-Terminal/Resources/GNOME-Terminal.addin.xml
GoogleCalculator/Makefile
GoogleCalculator/Resources/GoogleCalculator.addin.xml
GoogleCalendar/Makefile
GoogleCalendar/Resources/GoogleCalendar.addin.xml
GoogleContacts/Makefile
GoogleContacts/Resources/GoogleContacts.addin.xml
GoogleDocs/Makefile
GoogleDocs/Resources/GoogleDocs.addin.xml
GoogleMaps/Makefile
GoogleMaps/Resources/GoogleMaps.addin.xml
GoogleSearch/Makefile
GoogleSearch/Resources/GoogleSearch.addin.xml
ImageShack/Makefile
ImageShack/Resources/ImageShack.addin.xml
JIRA/Makefile
JIRA/Resources/JIRA.addin.xml
Launchpad/Makefile
Launchpad/Resources/Launchpad.addin.xml
LocateFiles/Makefile
LocateFiles/Resources/LocateFiles.addin.xml
ManLookUp/Makefile
ManLookUp/Resources/ManLookUp.addin.xml
Microblogging/Makefile
Microblogging/Resources/Microblogging.addin.xml
MPD/Makefile
MPD/Resources/MPD.addin.xml
NX/Makefile
NX/Resources/NX.addin.xml
OpenSearch/Makefile
OpenSearch/Resources/OpenSearch.addin.xml
Opera/Makefile
Opera/Resources/Opera.addin.xml
Pastebin/Makefile
Pastebin/Resources/Pastebin.addin.xml
Pidgin/Makefile
Pidgin/Resources/Pidgin.addin.xml
PingFM/Makefile
PingFM/Resources/PingFM.addin.xml
Putty/Makefile
Putty/Resources/Putty.addin.xml
Qalculate/Makefile
Qalculate/Resources/Qalculate.addin.xml
Quote/Makefile
Quote/Resources/Quote.addin.xml
RememberTheMilk/Makefile
RememberTheMilk/Resources/RTM.addin.xml
RemindMe/Makefile
RemindMe/Resources/RemindMe.addin.xml
Rhythmbox/Makefile
Rhythmbox/Resources/Rhythmbox.addin.xml
Riptide/Makefile
Riptide/Resources/Riptide.addin.xml
RSS/Makefile
RSS/Resources/RSS.addin.xml
Shelf/Makefile
Shelf/Resources/Shelf.addin.xml
Skype/Makefile
Skype/Resources/Skype.addin.xml
SqueezeCenter/Makefile
SqueezeCenter/Resources/SqueezeCenter.addin.xml
SSH/Makefile
SSH/Resources/SSH.addin.xml
StockQuote/Makefile
StockQuote/Resources/StockQuote.addin.xml
RequestTracker/Makefile
RequestTracker/Resources/RequestTracker.addin.xml
SystemServices/Makefile
SystemServices/Resources/SystemServices.addin.xml
Tasque/Makefile
Tasque/Resources/Tasque.addin.xml
TerminalServerClient/Makefile
TerminalServerClient/Resources/TerminalServerClient.addin.xml
Text/Makefile
Text/Resources/Text.addin.xml
Thunderbird/Makefile
Thunderbird/Resources/Thunderbird.addin.xml
TinyUrl/Makefile
TinyUrl/Resources/TinyUrl.addin.xml
Tracker/Makefile
Tracker/Resources/TrackerSearch.addin.xml
Translate/Makefile
Translate/Resources/Translate.addin.xml
Tomboy/Makefile
Tomboy/Resources/Tomboy.addin.xml
Vinagre/Makefile
Vinagre/Resources/Vinagre.addin.xml
VirtualBox/Makefile
VirtualBox/Resources/VirtualBox.addin.xml
VolumeControl/Makefile
VolumeControl/Resources/VolumeControl.addin.xml
WindowManager/Makefile
WindowManager/Resources/WindowManager.addin.xml
Woof/Makefile
Woof/Resources/Woof.addin.xml
Wordnet/Makefile
Wordnet/Resources/Wordnet.addin.xml
Xmms2/Makefile
Xmms2/Resources/xmms2.addin.xml
XRandR/Makefile
XRandR/Resources/XRandR.addin.xml
YouTube/Makefile
YouTube/Resources/Youtube.addin.xml
Zim/Makefile
Zim/Resources/Zim.addin.xml
Makefile
po/Makefile.in
BundledLibraries/Makefile
BundledLibraries/telepathy-sharp/Makefile
])

AC_OUTPUT

cat <<EOF

${PACKAGE}-${VERSION}

  Build Environment
    Install Prefix:    ${prefix}
    Plugin directory:  ${expanded_libdir}/gnome-do/plugins

    Mono C# Compiler:  ${MCS} ${GMCS_FLAGS}
    Mono Runtime:      ${MONO}

  Optional plugins:
    Banshee:           ${HAVE_BANSHEE}
    Empathy:	       ${HAVE_EMPATHY}
    Evolution:         ${HAVE_EVOLUTION}
    Flickr:            ${HAVE_FLICKR}
    Google Calendar:   ${HAVE_GCAL}
    Google Contacts:   ${HAVE_GMAIL}
    Google Docs:       ${HAVE_GDOCS}
    Window Manager:    ${HAVE_WNCK}
    YouTube:           ${HAVE_YOUTUBE}

  Build/Development:
    Release Build:     ${enable_release}

EOF

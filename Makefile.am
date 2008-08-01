ACLOCAL_AMFLAGS = -I m4 -I m4/shamrock
EXTRA_DIST = COPYING COPYRIGHT AUTHORS

SUBDIRS = po BundledLibraries \
	Archive \
	DiskMounter \
	Epiphany \
	Evolution \
	File \
	Firefox \
	Flickr \
	GCalendar \
	GMailContacts \
	GNOME-Dictionary \
	GNOME-Screenshot \
	GNOME-Session \
	GNOME-Terminal \
	GoogleCalculator \
	GoogleMaps \
	Launchpad \
	LocateFiles \
	OpenSearch \
	Pastebin \
	Pidgin \
	Rhythmbox \
	Riptide \
	RSS \
	Shelf \
	SSH \
	Tasque \
	Text \
	Tomboy \
	Twitter \
	Vinagre \
	VolumeControl \
	WindowManager

if BUILD_AMAROK
SUBDIRS += Amarok 
endif
if BUILD_BANSHEE
SUBDIRS += Banshee
endif

# Current dir _must_ be processed last; this ensures our repo is built with all the plugins
SUBDIRS += .

install-data-hook:
	mdtool setup pack $(DESTDIR)/@datadir@/gnome-do/plugins/*.dll
	mdtool setup rep-build $(DESTDIR)/@datadir@/gnome-do/plugins

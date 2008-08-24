ACLOCAL_AMFLAGS = -I m4 -I m4/shamrock
EXTRA_DIST = COPYING COPYRIGHT AUTHORS

SUBDIRS = po BundledLibraries \
	Archive \
	del.icio.us \
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
	GoogleSearch \
	ImageShack \
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

ASSEMBLY_INSTALL_DIR = $(DESTDIR)/@datadir@/gnome-do/plugins
PLUGIN_ASSEMBLIES = $(notdir $(wildcard $(ASSEMBLY_INSTALL_DIR)/*.dll))
install-data-hook:
	cd $(ASSEMBLY_INSTALL_DIR) && mdtool setup pack $(PLUGIN_ASSEMBLIES)
	mdtool setup rep-build $(DESTDIR)/@datadir@/gnome-do/plugins

uninstall-hook:
	rm $(ASSEMBLY_INSTALL_DIR)/*.mpack
	rm $(ASSEMBLY_INSTALL_DIR)/*.mrep

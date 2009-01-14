ACLOCAL_AMFLAGS = -I m4 -I m4/shamrock
EXTRA_DIST = COPYING COPYRIGHT AUTHORS

SUBDIRS = \
	po \
	BundledLibraries \
	Alias \
	AptURL \
	Archive \
	Bibtex \
	Cl.ickable \
	ClawsMail \
	Confluence \
	DiskMounter \
	EOG-Slideshow \
	Epiphany \
	Evolution \
	File \
	Firefox \
	Flickr \
	GNOME-Dictionary \
	GNOME-Screenshot \
	GNOME-Session \
	GNOME-Terminal \
	GoogleCalculator \
	GoogleCalendar \
	GoogleContacts \
	GoogleDocs \
	GoogleMaps \
	GoogleSearch \
	ImageShack \
	JIRA \
	Launchpad \
	LocateFiles \
	ManLookUp \
	Microblogging \
	MPD \
	NX \
	OpenSearch \
	Opera \
	Pastebin \
	Pidgin \
	PingFM \
	Putty \
	Quote \
	RememberTheMilk \
	Rhythmbox \
	Riptide \
	Shelf \
	Skype \
	SSH \
	StockQuote \
	SystemServices \
	TerminalServerClient \
	Text \
	TinyUrl \
	Translate \
	Tomboy \
	Vinagre \
	VirtualBox \
	VolumeControl \
	WindowManager \
	Woof \
	Wordnet \
	Xmms2 \
	Zim 

# Current dir _must_ be processed last; this ensures our repo is built with all the plugins
SUBDIRS += .

ASSEMBLY_INSTALL_DIR = $(DESTDIR)/@datadir@/gnome-do/plugins
PLUGIN_ASSEMBLIES = $(notdir $(wildcard $(ASSEMBLY_INSTALL_DIR)/*.dll))
BUNDLED_ASSEMBLIES = $(wildcard BundledLibraries/*.dll)
install-data-hook:
	cd $(ASSEMBLY_INSTALL_DIR) && mdtool setup pack $(PLUGIN_ASSEMBLIES)
	mdtool setup rep-build $(DESTDIR)/@datadir@/gnome-do/plugins

uninstall-hook:
	rm $(ASSEMBLY_INSTALL_DIR)/*.mpack
	rm $(ASSEMBLY_INSTALL_DIR)/*.mrep

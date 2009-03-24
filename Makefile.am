ACLOCAL_AMFLAGS = -I m4 -I m4/shamrock
EXTRA_DIST = COPYING COPYRIGHT AUTHORS

SUBDIRS = \
	po \
	BundledLibraries \
	Alias \
	AptURL \
	Archive \
	Banshee \
	Bibtex \
	Cl.ickable \
	ClawsMail \
	Confluence \
	Del.icio.us \
	DiskMounter \
	Emesene \
	EOG-Slideshow \
	Epiphany \
	Evolution \
	Exaile \
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
	RequestTracker \
	Rhythmbox \
	Riptide \
	RSS \
	Shelf \
	Skype \
	SqueezeCenter \
	SSH \
	StockQuote \
	SystemServices \
	Tasque \
	TerminalServerClient \
	Text \
	Thunderbird \
	TinyUrl \
	Tracker \
	Translate \
	Tomboy \
	Vinagre \
	VirtualBox \
	VolumeControl \
	WindowManager \
	Woof \
	Wordnet \
	Xmms2 \
	YouTube \
	Zim 

# Current dir _must_ be processed last; this ensures our repo is built with all
# the plugins
SUBDIRS += .

ASSEMBLY_INSTALL_DIR = $(DESTDIR)/@datadir@/gnome-do/plugins
install-data-hook:
	$(MAUTIL) rep-build $(ASSEMBLY_INSTALL_DIR)

uninstall-hook:
	rm -f $(ASSEMBLY_INSTALL_DIR)/*.mrep

clean-local:
	-rm -rf $(top_builddir)/build

bundledir = ${libdir}/gnome-do/plugins

if HAVE_EMPATHY
SUBDIRS=telepathy-sharp
endif

install-data-hook:
	if test -n "$(BANSHEE_INDEXER_DLL)"; then \
		$(INSTALL) -d $(DESTDIR)/$(bundledir) && \
		$(INSTALL) -m 644 -t $(DESTDIR)/$(bundledir) $(BANSHEE_INDEXER_DLL); \
	fi;

uninstall-hook:
	if test -n "$(BANSHEE_INDEXER_DLL)"; then \
		rm $(DESTDIR)/$(bundledir)/$(notdir $(BANSHEE_INDEXER_DLL)) \
	fi;

EXTRA_DIST= gmcs.exe 

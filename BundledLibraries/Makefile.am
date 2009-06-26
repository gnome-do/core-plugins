bundledir = ${libdir}/gnome-do/plugins

install-data-hook:
	$(INSTALL) -d $(DESTDIR)/$(bundledir)
	$(INSTALL) -m 644 -t $(DESTDIR)/$(bundledir) $(BANSHEE_INDEXER_DLL)

uninstall-hook:
	rm $(DESTDIR)/$(bundledir)/$(notdir $(BANSHEE_INDEXER_DLL))

EXTRA_DIST= gmcs.exe 

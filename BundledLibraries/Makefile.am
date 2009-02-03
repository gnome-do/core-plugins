bundledir = ${datadir}/gnome-do/plugins

bundle_DATA = \
	Google.GData.AccessControl.dll \
	Google.GData.Calendar.dll \
	Google.GData.Client.dll \
	Google.GData.Contacts.dll \
	Google.GData.Documents.dll \
	Google.GData.Extensions.dll 

EXTRA_DIST=$(bundle_DATA) gmcs.exe

BANSHEE_INDEXER_DLL = $(shell $(PKG_CONFIG) --variable=Libraries banshee-1-collection-indexer)
install-data-hook:
	$(INSTALL) -d $(DESTDIR)/$(bundledir)
	$(INSTALL) -m 644 -t $(DESTDIR)/$(bundledir) $(BANSHEE_INDEXER_DLL)

uninstall-hook:
	rm $(DESTDIR)/$(bundledir)/$(notdir $(BANSHEE_INDEXER_DLL))

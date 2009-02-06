bundledir = ${datadir}/gnome-do/plugins

GOOGLE_LIBS = \
	Google.GData.AccessControl.dll \
	Google.GData.Calendar.dll \
	Google.GData.Client.dll \
	Google.GData.Contacts.dll \
	Google.GData.Documents.dll \
	Google.GData.YouTube.dll \
	Google.GData.Extensions.dll

bundle_DATA = \
	$(addprefix $(srcdir)/,$(GOOGLE_LIBS)) \
	$(BANSHEE_INDEXER_DLL)

EXTRA_DIST=$(GOOGLE_LIBS) gmcs.exe

# Copy these to the build dir; some things require the dlls to be present
# during build - particularly mautil.
all-local:
	mkdir -p $(top_builddir)/build
	install -m 644 -t $(top_builddir)/build $(bundle_DATA)

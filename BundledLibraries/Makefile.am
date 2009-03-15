bundledir = ${datadir}/gnome-do/plugins

SUBDIRS = libgoogle-data-mono-1.4.0.2

GOOGLE_LIBS = \
	libgoogle-data-mono-1.4.0.2/Google.GData.AccessControl.dll \
	libgoogle-data-mono-1.4.0.2/Google.GData.Calendar.dll \
	libgoogle-data-mono-1.4.0.2/Google.GData.Client.dll \
	libgoogle-data-mono-1.4.0.2/Google.GData.Contacts.dll \
	libgoogle-data-mono-1.4.0.2/Google.GData.Documents.dll \
	libgoogle-data-mono-1.4.0.2/Google.GData.YouTube.dll \
	libgoogle-data-mono-1.4.0.2/Google.GData.Extensions.dll

bundle_DATA = \
	$(addprefix $(srcdir)/,$(GOOGLE_LIBS)) \
	$(BANSHEE_INDEXER_DLL)

EXTRA_DIST=$(GOOGLE_LIBS) gmcs.exe

# Copy these to the build dir; some things require the dlls to be present
# during build - particularly mautil.
all-local:
	mkdir -p $(top_builddir)/build
	install -m 644 -t $(srcdir) $(bundle_DATA)
	install -m 644 -t $(top_builddir)/build $(bundle_DATA)

clean-local:
	-rm -f $(srcdir)/*.dll

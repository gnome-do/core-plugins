bundledir = ${datadir}/gnome-do/plugins

gdata_subdir = libgoogle-data-mono-1.4.0.2
gdata_subdir_full = $(addprefix $(srcdir)/,$(gdata_subdir))

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

EXTRA_DIST= gmcs.exe $(gdata_subdir_full)

# Copy these to the build dir; some things require the dlls to be present
# during build - particularly mautil.
all-local: $(GOOGLE_LIBS)
	mkdir -p $(top_builddir)/build
	install -m 644 -t $(top_builddir)/build $(bundle_DATA)

dist-hook:
	chmod -R u+w $(distdir)
	rm -f $(distdir)/*.dll

clean-local:
	rm -f $(srcdir)/*.dll
	rm -f $(top_builddir)/build/*.dll
     
%.dll:
	chmod -R u+w $(srcdir)
	$(MAKE) -C $(gdata_subdir_full) $(AM_MAKEFLAGS) $(addsuffix .dll, $(notdir $(basename $@)))
	mv $(gdata_subdir_full)/*.dll $(srcdir)

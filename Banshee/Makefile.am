if HAVE_BANSHEE
include $(top_srcdir)/build.rules.mk
endif

ASSEMBLY=Banshee

FILES = \
	src/AbstractPlayerAction.cs \
	src/Banshee.cs \
	src/BansheeDBus.cs \
	src/BansheeIndexer.cs \
	src/BrowseMediaItems.cs \
	src/EnqueueAction.cs \
	src/MediaItems.cs \
	src/MediaItemSource.cs \
	src/NextAction.cs \
	src/PauseAction.cs \
	src/PlayAction.cs \
	src/PreviousAction.cs \
	src/SearchCollectionAction.cs 

RESOURCES = \
	Resources/Banshee.addin.xml

REFERENCES = \
	System \
	System.Core \
	$(BANSHEE_INDEXER_LIBS) \
	$(DBUS_SHARP_LIBS) \
	$(DBUS_SHARP_GLIB_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS)

include $(top_srcdir)/build.rules.mk

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
	src/SearchCollectionAction.cs \
	src/Util.cs

RESOURCES = \
	Resources/Banshee.addin.xml

REFERENCES = \
	Mono.Posix \
	System \
	System.Core \
	$(BANSHEE_INDEXER_LIBS) \
	$(NDESK_DBUS_10_LIBS) \
	$(NDESK_DBUS_GLIB_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS)

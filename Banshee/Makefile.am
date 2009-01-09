include $(top_srcdir)/build.rules.mk

ASSEMBLY=Banshee

FILES = \
	src/Banshee.cs \
	src/BansheeDBus.cs \
	src/BansheeEnqueueAction.cs \
	src/BansheeIndexer.cs \
	src/BansheeItemSource.cs \
	src/BansheeMediaItems.cs \
	src/BansheePlayAction.cs \
	src/BansheeRunnableItem.cs \
	src/BansheeSearchCollectionAction.cs \
	src/BrowseMediaItems.cs

RESOURCES = \
	Resources/Banshee.addin.xml

REFERENCES = \
	Mono.Posix \
	System \
	System.Core \
	Banshee.CollectionIndexer \
	$(NDESK_DBUS_10_LIBS) \
	$(NDESK_DBUS_GLIB_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS)

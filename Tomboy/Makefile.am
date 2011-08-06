include $(top_srcdir)/build.rules.mk

ASSEMBLY=Tomboy

FILES = \
	src/TomboyDBus.cs \
	src/TomboyItem.cs \
	src/NewNoteAction.cs \
	src/NotesItemSource.cs \
	src/SearchNotesAction.cs \
	gtk-gui/generated.cs \
	src/TomboyConfiguration.cs \
	src/TomboyBrowseNotesItem.cs \
	gtk-gui/Tomboy.TomboyConfiguration.cs

RESOURCES = \
	Resources/Tomboy.addin.xml 

BUILD_DEFINES=$(DBUS_BUILD_DEFINES)

REFERENCES = \
	System \
	System.Core \
	$(DBUS_SHARP_LIBS) \
	$(DBUS_SHARP_GLIB_LIBS) \
	$(GTK_SHARP_20_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS)

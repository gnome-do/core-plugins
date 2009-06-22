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

REFERENCES = \
	System \
	System.Core \
	$(NDESK_DBUS_10_LIBS) \
	$(NDESK_DBUS_GLIB_10_LIBS) \
	$(GTK_SHARP_20_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS)

include $(top_srcdir)/build.rules.mk

ASSEMBLY=WindowManager

FILES = \
	src/WindowListAction.cs \
	src/IWindowList.cs \
	src/WindowItem.cs \
	src/WindowItemSource.cs \
	src/ScreenListAction.cs \
	src/WindowListItems.cs \
	src/GenericWindowItem.cs \
	src/ScreenItemSource.cs

RESOURCES = \
	Resources/WindowManager.addin.xml 

REFERENCES = \
	Mono.Posix \
	System \
	System.Core \
	$(WNCK_SHARP_10_LIBS) \
	$(GTK_SHARP_20_LIBS) \
	$(GLIB_SHARP_20_LIBS) \
	$(GNOME_DESKTOP_SHARP_20_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS) \
	$(DO_INTERFACE_WINK_LIBS)

include $(top_srcdir)/build.rules.mk

ASSEMBLY=WindowManager

FILES = \
	src/CurrentApplicationItem.cs \
	src/CurrentWindowItem.cs \
	src/IWindowList.cs \
	src/WindowItem.cs \
	src/WindowItemSource.cs \
	src/Screen/CurrentScreenItem.cs \
	src/Screen/ScreenItem.cs \
	src/Screen/ScreenItemSource.cs \
	src/Screen/DoModifyGeometry.cs \
	src/Screen/ScreenActionAction.cs \
	src/Screen/ScreenCascadeAction.cs \
	src/Screen/ScreenRestoreAction.cs \
	src/Screen/ScreenSwapAction.cs \
	src/Screen/ScreenTileAction.cs \
	src/WindowActions/WindowActionAction.cs \
	src/WindowActions/WindowCloseAction.cs \
	src/WindowActions/WindowMaximizeAction.cs \
	src/WindowActions/WindowMinimizeAction.cs \
	src/WindowActions/WindowMoveAction.cs \


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

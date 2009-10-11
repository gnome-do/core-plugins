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
	src/Screen/ScreenActionAction.cs \
	src/Screen/ScreenCascadeAction.cs \
	src/Screen/ScreenRestoreAction.cs \
	src/Screen/ScreenSwapAction.cs \
	src/Screen/ScreenTileAction.cs \
	src/Screen/ShowDesktopAction.cs \
	src/WindowActions/WindowActionAction.cs \
	src/WindowActions/WindowCloseAction.cs \
	src/WindowActions/WindowFocusAction.cs \
	src/WindowActions/WindowMaximizeAction.cs \
	src/WindowActions/WindowMinimizeAction.cs \
	src/WindowActions/WindowMoveAction.cs \
	src/Wink/ScreenUtils.cs \
	src/Wink/WindowControl.cs \
	src/Wink/WnckWindow_Extensions.cs \
	src/Wink/Position.cs \
	src/Wink/Viewport.cs \
	src/Wink/WindowUtils.cs \
	src/Xlib/X11Atoms.cs \
	src/Xlib/Xlib.cs      




RESOURCES = \
	Resources/WindowManager.addin.xml 

REFERENCES = \
	System \
	System.Core \
	Mono.Posix \
	$(WNCK_SHARP_10_LIBS) \
	$(GTK_SHARP_20_LIBS) \
	$(GLIB_SHARP_20_LIBS) \
	$(GNOME_DESKTOP_SHARP_20_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS)

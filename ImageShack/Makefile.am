include $(top_srcdir)/build.rules.mk

ASSEMBLY=ImageShack

FILES = \
	gtk-gui/generated.cs \
	gtk-gui/ImageShack.ImageShackConfig.cs \
	src/ImageShackAction.cs \
	src/ImageShackConfig.cs \
	src/Notifications.cs

RESOURCES = \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml \
	Resources/icons/imageshack.png \
	Resources/ImageShack.addin.xml

REFERENCES = \
	Mono.Posix \
	System \
	System.Core \
	System.Web \
	System.Xml.Linq \
	$(GTK_SHARP_20_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS)

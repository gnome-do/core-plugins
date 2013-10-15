include $(top_srcdir)/build.rules.mk

ASSEMBLY=Tasque

FILES = \
	src/TasqueDBus.cs \
	src/TasqueCategoryItem.cs \
	src/Tasque.cs \
	src/TasqueAction.cs

GENERATED_RESOURCES = \
	Resources/Tasque.addin.xml 

BUILD_DEFINES=$(DBUS_BUILD_DEFINES)

REFERENCES = \
	System \
	System.Core \
	$(DBUS_SHARP_LIBS) \
	$(DBUS_SHARP_GLIB_LIBS) \
	$(GCONF_SHARP_20_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS)

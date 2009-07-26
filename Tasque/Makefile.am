include $(top_srcdir)/build.rules.mk

ASSEMBLY=Tasque

FILES = \
	src/TasqueDBus.cs \
	src/TasqueCategoryItem.cs \
	src/Tasque.cs \
	src/TasqueAction.cs

RESOURCES = \
	Resources/Tasque.addin.xml 

REFERENCES = \
	System \
	System.Core \
	$(NDESK_DBUS_10_LIBS) \
	$(NDESK_DBUS_GLIB_LIBS) \
	$(GCONF_SHARP_20_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS)

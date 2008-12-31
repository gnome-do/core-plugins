include $(top_srcdir)/build.rules.mk

ASSEMBLY=Banshee

FILES = \
	src/AssemblyInfo.cs \
	src/BansheeAction.cs \
	src/Banshee.cs \
	src/BansheeDBus.cs \
	src/BansheeItems.cs \
	src/BansheeItemSource.cs

RESOURCES = \
	src/Banshee.addin.xml

REFERENCES = \
	Mono.Data.Sqlite \
	System \
	System.Data \
	System.Xml
	$(NDESK_DBUS_10_LIBS) \
	$(NDESK_DBUS_GLIB_10_LIBS)

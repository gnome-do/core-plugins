include $(top_srcdir)/build.rules.mk

ASSEMBLY=SystemServices

FILES = \
	src/ServiceActions.cs \
	src/ServiceActionType.cs \
	src/Service.cs \
	src/ServiceItemSource.cs \
	src/SystemServicesConfig.cs \
	src/SystemServices.cs \
	gtk-gui/generated.cs \
	gtk-gui/SystemServices.SystemServicesConfig.cs

RESOURCES = \
	Resources/SystemServices.addin.xml

REFERENCES = \
	Mono.Posix \
	System \
	System.Core \
	System.Xml \
	$(GTK_SHARP_20_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS)

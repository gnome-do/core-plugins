include $(top_srcdir)/build.rules.mk

ASSEMBLY=Confluence

FILES = \
	gtk-gui/Confluence.ConfluenceConfigWidget.cs \
	gtk-gui/generated.cs \
	src/ConfluenceConfiguration.cs \
	src/ConfluenceConfigWidget.cs \
	src/ConfluenceSearchAction.cs \
	src/Remote/ConfluenceSoapServiceService.cs

RESOURCES = \
	Resources/Confluence.addin.xml \
	Resources/icons/confluence.png \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml

REFERENCES = \
	Mono.Posix \
	System \
	System.Core \
	System.Web \
	System.Web.Services \
	System.Xml \
	$(GTK_SHARP_20_LIBS) \
	$(GCONF_SHARP_20_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS)

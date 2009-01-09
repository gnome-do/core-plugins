include $(top_srcdir)/build.rules.mk

ASSEMBLY=GoogleSearch

FILES = \
	gtk-gui/generated.cs \
	gtk-gui/InlineGoogleSearch.InlineGoogleSearchConfig.cs \
	src/GoogleSearch.cs \
	src/GoogleSearchResult.cs \
	src/ImFeelingLuckyAction.cs \
	src/InlineGoogleSearchConfig.cs \
	src/InlineGoogleSearch.cs

RESOURCES = \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml \
	Resources/GoogleSearch.addin.xml

REFERENCES = \
	Mono.Posix \
	System \
	System.Core \
	System.Web \
	System.Web.Services \
	$(GTK_SHARP_20_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS)

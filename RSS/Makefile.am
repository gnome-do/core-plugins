
EXTRA_DIST =  



if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/RSS.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug


endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/RSS.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release


endif

AL=al2
SATELLITE_ASSEMBLY_NAME=.resources.dll

all: $(ASSEMBLY)


DATA_FILES = 

FILES = \
	gtk-gui/Do.Plugins.Rss.Configuration.cs \
	gtk-gui/generated.cs \
	src/Configuration.cs \
	src/FirefoxLiveBookmarksItemSource.cs \
	src/RssFeedAction.cs \
	src/RssFeedItem.cs \
	src/RssFeedLinkItem.cs \
	src/RssItemSource.cs \
	src/RSS.NET/Day.cs \
	src/RSS.NET/Hour.cs \
	src/RSS.NET/RssChannel/RssChannel.cs \
	src/RSS.NET/RssChannel/RssCloud.cs \
	src/RSS.NET/RssChannel/RssImage.cs \
	src/RSS.NET/RssChannel/RssTextInput.cs \
	src/RSS.NET/RssFeed.cs \
	src/RSS.NET/RssItem/RssEnclosure.cs \
	src/RSS.NET/RssItem/RssGuid.cs \
	src/RSS.NET/RssItem/RssItem.cs \
	src/RSS.NET/RssItem/RssSource.cs \
	src/RSS.NET/RssModule.cs \
	src/RSS.NET/RssModuleItem.cs \
	src/RSS.NET/RssReader.cs \
	src/RSS.NET/Shared/RssCategory.cs \
	src/RSS.NET/Shared/RssDefault.cs \
	src/RSS.NET/Shared/RssElement.cs \
	src/RSS.NET/Shared/RssEnumerators.cs

RESOURCES = \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml \
	resources/feed-icon.png \
	resources/RSS.addin.xml

REFERENCES =  \
	System \
	System.Xml \
	System.Web \
	Mono.Posix \
	$(DOADDINS_LIBS) \
	$(GTK_SHARP_20_LIBS) \
	$(GCONF_SHARP_20_LIBS)

DLL_REFERENCES = 

CLEANFILES = 

include $(top_srcdir)/Makefile.include


$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(build_resx_resources) : %.resources: %.resx
	resgen2 '$<' '$@'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(dir $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)

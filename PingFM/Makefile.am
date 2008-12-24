
EXTRA_DIST =  



if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/PingFM.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug


endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/PingFM.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release


endif

AL=al2
SATELLITE_ASSEMBLY_NAME=.resources.dll

all: $(ASSEMBLY)

FILES = \
	gtk-gui/Do.Addins.PingFM.Configuration.cs \
	gtk-gui/generated.cs \
	src/Configuration.cs \
	src/PingFMApi.cs \
	src/PingFM.cs \
	src/PingFMPost.cs \
	src/PingFMServiceItem.cs \
	src/PingFMServiceItemSource.cs

RESOURCES = \
	PingFM.addin.xml \
	resources/bebo.png \
	resources/blogger.png \
	resources/brightkite.png \
	resources/delicious.png \
	resources/facebook.png \
	resources/flickr.png \
	resources/friendfeed.png \
	resources/friendster.png \
	resources/hi5.png \
	resources/identica.png \
	resources/jaiku.png \
	resources/koornk.png \
	resources/kwippy.png \
	resources/linkedin.png \
	resources/livejournal.png \
	resources/mashable.png \
	resources/multiply.png \
	resources/myspace.png \
	resources/pingfm.png \
	resources/plaxo.png \
	resources/plurk.png \
	resources/pownce.png \
	resources/rejaw.png \
	resources/tumblr.png \
	resources/twitter.png \
	resources/wordpress.png \
	resources/xanga.png \
	resources/yahoo360.png \
	resources/yammer.png \
	resources/youare.png 

DATA_FILES = 

REFERENCES =  \
	System \
	System.Core \
	System.Data \
	System.Web \
	System.Xml \
	Mono.Posix \
	$(GNOME_KEYRING_SHARP_LIBS) \
	$(DOADDINS_LIBS) \
	$(GTK_SHARP_20_LIBS)

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

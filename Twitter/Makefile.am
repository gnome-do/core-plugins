
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/Twitter.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug


endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/Twitter.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release


endif

AL=al2
SATELLITE_ASSEMBLY_NAME=DoTwitter.resources.dll

all: $(ASSEMBLY) 

FILES = \
	src/Configuration.cs \
	src/TwitterAction.cs \
	src/TwitterFriendSource.cs \
	src/TwitterTweet.cs \
	src/Twitterizer/Twitter.cs \
	src/Twitterizer/TwitterizerException.cs \
	src/Twitterizer/TwitterRequest.cs \
	src/Twitterizer/TwitterRequestData.cs \
	src/Twitterizer/TwitterStatus.cs \
	src/Twitterizer/TwitterStatusCollection.cs \
	src/Twitterizer/TwitterUser.cs \
	src/Twitterizer/TwitterUserCollection.cs \
	src/Twitterizer/Properties/AssemblyInfo.cs \
	gtk-gui/generated.cs \
	src/GenConfig.cs \
	gtk-gui/DoTwitter.GenConfig.cs 

DATA_FILES = 

RESOURCES = \
	Twitter.addin.xml \
	twitter-icon.png \
	twitter_items.png \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml 

REFERENCES =  \
	$(DOADDINS_LIBS) \
	$(GTK_SHARP_20_LIBS) \
	System \
	System.Xml \
	System.Web \
	Mono.Posix

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

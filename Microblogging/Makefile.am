
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

TWITTER_DLL_MDB_SOURCE=bin/Debug/Twitter.dll.mdb
TWITTER_DLL_MDB=$(BUILD_DIR)/Twitter.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/Twitter.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

TWITTER_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(TWITTER_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(TWITTER_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/Configuration.cs \
	src/TwitterAction.cs \
	src/TwitterFriendSource.cs \
	src/TwitterTweet.cs \
	gtk-gui/generated.cs \
	src/GenConfig.cs \
	gtk-gui/DoTwitter.GenConfig.cs \
	src/Twitterizer/Twiterizer.Framework/Twitter.cs \
	src/Twitterizer/Twiterizer.Framework/TwitterizerException.cs \
	src/Twitterizer/Twiterizer.Framework/TwitterRequest.cs \
	src/Twitterizer/Twiterizer.Framework/TwitterRequestData.cs \
	src/Twitterizer/Twiterizer.Framework/TwitterStatus.cs \
	src/Twitterizer/Twiterizer.Framework/TwitterStatusCollection.cs \
	src/Twitterizer/Twiterizer.Framework/TwitterUser.cs \
	src/Twitterizer/Twiterizer.Framework/TwitterUserCollection.cs \
	src/Twitterizer/Twiterizer.Framework/Properties/AssemblyInfo.cs 

DATA_FILES = 

RESOURCES = \
	Twitter.addin.xml \
	twitter-icon.png \
	twitter_items.png \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml 

EXTRAS = \
	src/Twitterizer/Twiterizer.Framework/Twitterizer.Framework.csproj \
	twitter.pc.in 

REFERENCES =  \
	Do.Addins \
	$(GTK_SHARP_20_LIBS) \
	System \
	System.Xml \
	System.Web \
	Mono.Posix \
	System.Configuration

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

TWITTER_PC = $(BUILD_DIR)/twitter.pc

$(eval $(call emit-deploy-wrapper,TWITTER_PC,twitter.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)

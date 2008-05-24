
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

if ENABLE_REPO
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/Twitter.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug


endif

AL=al2
SATELLITE_ASSEMBLY_NAME=.resources.dll

LINUX_PKGCONFIG = \
	$(TWITTER_PC)  


	
all: $(ASSEMBLY) $(LINUX_PKGCONFIG) 

FILES = \
	src/Notifications.cs \
	src/Twitter.cs \
	src/TwitterAction.cs 

DATA_FILES = 

RESOURCES = \
	twitter-icon.png 

EXTRAS = \
	twitter.pc.in 

REFERENCES =  \
	$(DOADDINS_LIBS) \
	System \
	$(NDESK_DBUS_GLIB_10_LIBS) \
	$(NDESK_DBUS_10_LIBS) \
	System.Xml \
	$(GCONF_SHARP_20_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

TWITTER_PC = $(BUILD_DIR)/twitter.pc

$(eval $(call emit-deploy-wrapper,TWITTER_PC,twitter.pc))


$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(build_resx_resources) : %.resources: %.resx
	resgen2 '$<' '$@'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(dir $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)

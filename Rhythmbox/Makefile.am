
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/Rhythmbox.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

RHYTHMBOX_ADDIN_XML_SOURCE=src/Rhythmbox.addin.xml

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/Rhythmbox.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

RHYTHMBOX_ADDIN_XML_SOURCE=src/Rhythmbox.addin.xml

endif

if ENABLE_REPO
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/Rhythmbox.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

RHYTHMBOX_ADDIN_XML_SOURCE=src/Rhythmbox.addin.xml

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=.resources.dll

PROGRAMFILES = \
	$(RHYTHMBOX_ADDIN_XML)  

LINUX_PKGCONFIG = \
	$(RHYTHMBOX_PC)  


	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/MusicItems.cs \
	src/Rhythmbox.cs \
	src/RhythmboxEnqueueAction.cs \
	src/RhythmboxItems.cs \
	src/RhythmboxItemSource.cs \
	src/RhythmboxPlayAction.cs \
	src/AssemblyInfo.cs 

DATA_FILES = \
	src/Rhythmbox.addin.xml 

RESOURCES = 

EXTRAS = \
	rhythmbox.pc.in 

REFERENCES =  \
	System.Xml \
	System \
	$(DOADDINS_LIBS) \
	$(DODBUS_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

RHYTHMBOX_PC = $(BUILD_DIR)/rhythmbox.pc
RHYTHMBOX_ADDIN_XML = $(BUILD_DIR)/Rhythmbox.addin.xml

$(eval $(call emit-deploy-wrapper,RHYTHMBOX_PC,rhythmbox.pc))
$(eval $(call emit-deploy-target,RHYTHMBOX_ADDIN_XML))


$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(build_resx_resources) : %.resources: %.resx
	resgen2 '$<' '$@'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(dir $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)


EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/DiskMounter.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

DISKMOUNTER_ADDIN_XML_SOURCE=src/DiskMounter.addin.xml

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/DiskMounter.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

DISKMOUNTER_ADDIN_XML_SOURCE=src/DiskMounter.addin.xml

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=DiskMounter.resources.dll

PROGRAMFILES = \
	$(DISKMOUNTER_ADDIN_XML)  

all: $(ASSEMBLY) $(PROGRAMFILES)

FILES = \
	src/DriveItem.cs \
	src/DriveItemSource.cs \
	src/MountAction.cs \
	src/OpenVolumeAction.cs \
	src/UnmountAction.cs 

DATA_FILES = \
	src/DiskMounter.addin.xml 

RESOURCES = 

REFERENCES =  \
	System \
	$(GNOME_VFS_SHARP_20_LIBS) \
	$(DOADDINS_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES)

include $(top_srcdir)/Makefile.include

DISKMOUNTER_ADDIN_XML = $(BUILD_DIR)/DiskMounter.addin.xml

$(eval $(call emit-deploy-target,DISKMOUNTER_ADDIN_XML))


$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(build_resx_resources) : %.resources: %.resx
	resgen2 '$<' '$@'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(dir $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)


EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/Pastebin.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

PASTEBIN_ADDIN_XML_SOURCE=src/Pastebin.addin.xml

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/Pastebin.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

PASTEBIN_ADDIN_XML_SOURCE=src/Pastebin.addin.xml

endif

if ENABLE_REPO
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug -define:DEBUG
ASSEMBLY = ../buildrepo/Pastebin.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ../buildrepo

PASTEBIN_ADDIN_XML_SOURCE=src/Pastebin.addin.xml

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=.resources.dll

PROGRAMFILES = \
	$(PASTEBIN_ADDIN_XML)  

LINUX_PKGCONFIG = \
	$(PASTEBIN_PC)  


	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/AssemblyInfo.cs \
	src/IPastebinProvider.cs \
	src/ITextSyntaxItem.cs \
	src/Paste2.cs \
	src/Pastebin.cs \
	src/PastebinAction.cs \
	src/TextSyntaxItem.cs 

DATA_FILES = \
	src/Pastebin.addin.xml 

RESOURCES = 

EXTRAS = \
	pastebin.pc.in 

REFERENCES =  \
	System \
	$(DOADDINS_LIBS) \
	Mono.Posix \
	System.Web

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

PASTEBIN_PC = $(BUILD_DIR)/pastebin.pc
PASTEBIN_ADDIN_XML = $(BUILD_DIR)/Pastebin.addin.xml

$(eval $(call emit-deploy-wrapper,PASTEBIN_PC,pastebin.pc))
$(eval $(call emit-deploy-target,PASTEBIN_ADDIN_XML))


$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(build_resx_resources) : %.resources: %.resx
	resgen2 '$<' '$@'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(dir $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)

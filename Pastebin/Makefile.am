
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

PASTEBIN_DLL_MDB_SOURCE=bin/Debug/Pastebin.dll.mdb
PASTEBIN_DLL_MDB=$(BUILD_DIR)/Pastebin.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/Pastebin.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

PASTEBIN_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(PASTEBIN_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(PASTEBIN_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/AssemblyInfo.cs \
	src/Pastebin.cs \
	src/PastebinAction.cs \
	gtk-gui/generated.cs \
	gtk-gui/Pastebin.PastebinConfig.cs \
	src/Providers/PastebinCA.cs \
	src/Providers/IPastebinProvider.cs \
	src/Providers/Paste2.cs \
	src/Config/PastebinConfig.cs \
	src/Providers/PastebinProviderFactory.cs \
	src/TextSyntax/ITextSyntaxItem.cs \
	src/TextSyntax/TextSyntaxItem.cs 

DATA_FILES = 

RESOURCES = \
	src/Pastebin.addin.xml \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml 

EXTRAS = \
	pastebin.pc.in 

REFERENCES =  \
	System \
	Mono.Posix \
	System.Web \
	Do.Addins \
	$(GTK_SHARP_20_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

PASTEBIN_PC = $(BUILD_DIR)/pastebin.pc

$(eval $(call emit-deploy-wrapper,PASTEBIN_PC,pastebin.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)

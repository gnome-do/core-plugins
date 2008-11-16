
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/GDocs.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

GOOGLE_GDATA_ACCESSCONTROL_DLL_SOURCE=../BundledLibraries/Google.GData.AccessControl.dll
GOOGLE_GDATA_CLIENT_DLL_SOURCE=../BundledLibraries/Google.GData.Client.dll
GOOGLE_GDATA_DOCUMENTS_DLL_SOURCE=../BundledLibraries/Google.GData.Documents.dll
GDOCS_DLL_MDB_SOURCE=bin/Debug/GDocs.dll.mdb
GDOCS_DLL_MDB=$(BUILD_DIR)/GDocs.dll.mdb
GOOGLE_GDATA_EXTENSIONS_DLL_SOURCE=../BundledLibraries/Google.GData.Extensions.dll

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/GDocs.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

GOOGLE_GDATA_ACCESSCONTROL_DLL_SOURCE=../BundledLibraries/Google.GData.AccessControl.dll
GOOGLE_GDATA_CLIENT_DLL_SOURCE=../BundledLibraries/Google.GData.Client.dll
GOOGLE_GDATA_DOCUMENTS_DLL_SOURCE=../BundledLibraries/Google.GData.Documents.dll
GDOCS_DLL_MDB=
GOOGLE_GDATA_EXTENSIONS_DLL_SOURCE=../BundledLibraries/Google.GData.Extensions.dll

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(GOOGLE_GDATA_ACCESSCONTROL_DLL) \
	$(GOOGLE_GDATA_CLIENT_DLL) \
	$(GOOGLE_GDATA_DOCUMENTS_DLL) \
	$(GDOCS_DLL_MDB) \
	$(GOOGLE_GDATA_EXTENSIONS_DLL)  

LINUX_PKGCONFIG = \
	$(GDOCS_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/CertHandler.cs \
	src/Configuration.cs \
	src/GDocs.cs \
	src/GDocsItemSource.cs \
	src/GDocsItem.cs \
	src/GDocsDocumentItem.cs \
	src/GDocsSpreadsheetItem.cs \
	src/GDocsPDFItem.cs \
	src/GDocsPresentationItem.cs \
	src/GDocsUploadDocument.cs \
	src/GDocsTrashDocument.cs 

DATA_FILES = 

RESOURCES = \
	resources/gDocsIcon.png \
	resources/gDocsUploadIcon.png \
	resources/gDocsTrashIcon.png \
	GDocs.addin.xml 

EXTRAS = \
	../BundledLibraries/Google.GData.AccessControl.dll \
	../BundledLibraries/Google.GData.Extensions.dll \
	../BundledLibraries/Google.GData.Documents.dll \
	../BundledLibraries/Google.GData.Client.dll \
	gdocs.pc.in 

REFERENCES =  \
	System \
	Do.Addins \
	$(GTK_SHARP_20_LIBS) \
	Mono.Posix \
	System.Xml

DLL_REFERENCES =  \
	../BundledLibraries/Google.GData.AccessControl.dll \
	../BundledLibraries/Google.GData.Client.dll \
	../BundledLibraries/Google.GData.Documents.dll \
	../BundledLibraries/Google.GData.Extensions.dll

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

GDOCS_PC = $(BUILD_DIR)/gdocs.pc
GOOGLE_GDATA_ACCESSCONTROL_DLL = $(BUILD_DIR)/Google.GData.AccessControl.dll
GOOGLE_GDATA_CLIENT_DLL = $(BUILD_DIR)/Google.GData.Client.dll
GOOGLE_GDATA_DOCUMENTS_DLL = $(BUILD_DIR)/Google.GData.Documents.dll
GOOGLE_GDATA_EXTENSIONS_DLL = $(BUILD_DIR)/Google.GData.Extensions.dll

$(eval $(call emit-deploy-wrapper,GDOCS_PC,gdocs.pc))
$(eval $(call emit-deploy-target,GOOGLE_GDATA_ACCESSCONTROL_DLL))
$(eval $(call emit-deploy-target,GOOGLE_GDATA_CLIENT_DLL))
$(eval $(call emit-deploy-target,GOOGLE_GDATA_DOCUMENTS_DLL))
$(eval $(call emit-deploy-target,GOOGLE_GDATA_EXTENSIONS_DLL))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)

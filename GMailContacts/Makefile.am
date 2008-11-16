
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/GMailContacts.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

GMAILCONTACTS_DLL_MDB_SOURCE=bin/Debug/GMailContacts.dll.mdb
GMAILCONTACTS_DLL_MDB=$(BUILD_DIR)/GMailContacts.dll.mdb
GOOGLE_GDATA_CLIENT_DLL_SOURCE=../BundledLibraries/Google.GData.Client.dll
GOOGLE_GDATA_CONTACTS_DLL_SOURCE=../BundledLibraries/Google.GData.Contacts.dll
GOOGLE_GDATA_EXTENSIONS_DLL_SOURCE=../BundledLibraries/Google.GData.Extensions.dll

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/GMailContacts.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

GMAILCONTACTS_DLL_MDB=
GOOGLE_GDATA_CLIENT_DLL_SOURCE=../BundledLibraries/Google.GData.Client.dll
GOOGLE_GDATA_CONTACTS_DLL_SOURCE=../BundledLibraries/Google.GData.Contacts.dll
GOOGLE_GDATA_EXTENSIONS_DLL_SOURCE=../BundledLibraries/Google.GData.Extensions.dll

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(GMAILCONTACTS_DLL_MDB) \
	$(GOOGLE_GDATA_CLIENT_DLL) \
	$(GOOGLE_GDATA_CONTACTS_DLL) \
	$(GOOGLE_GDATA_EXTENSIONS_DLL)  

LINUX_PKGCONFIG = \
	$(GMAILCONTACTS_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/GMail.cs \
	src/GMailContactItemSource.cs \
	src/GMailContactDetailItem.cs \
	src/CertHandler.cs \
	src/GMailConfig.cs \
	gtk-gui/generated.cs 

DATA_FILES = 

RESOURCES = \
	resources/gmail-logo.png \
	GMailContacts.addin.xml \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml \
	resources/phone.png 

EXTRAS = \
	../BundledLibraries/Google.GData.Contacts.dll \
	../BundledLibraries/Google.GData.Extensions.dll \
	../BundledLibraries/Google.GData.Client.dll \
	gmailcontacts.pc.in 

REFERENCES =  \
	Do.Addins \
	System \
	System.Security \
	$(GTK_SHARP_20_LIBS) \
	Mono.Posix

DLL_REFERENCES =  \
	../BundledLibraries/Google.GData.Client.dll \
	../BundledLibraries/Google.GData.Contacts.dll \
	../BundledLibraries/Google.GData.Extensions.dll

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

GMAILCONTACTS_PC = $(BUILD_DIR)/gmailcontacts.pc
GOOGLE_GDATA_CLIENT_DLL = $(BUILD_DIR)/Google.GData.Client.dll
GOOGLE_GDATA_CONTACTS_DLL = $(BUILD_DIR)/Google.GData.Contacts.dll
GOOGLE_GDATA_EXTENSIONS_DLL = $(BUILD_DIR)/Google.GData.Extensions.dll

$(eval $(call emit-deploy-wrapper,GMAILCONTACTS_PC,gmailcontacts.pc))
$(eval $(call emit-deploy-target,GOOGLE_GDATA_CLIENT_DLL))
$(eval $(call emit-deploy-target,GOOGLE_GDATA_CONTACTS_DLL))
$(eval $(call emit-deploy-target,GOOGLE_GDATA_EXTENSIONS_DLL))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)

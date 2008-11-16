
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/GCalendar.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

GOOGLE_GDATA_ACCESSCONTROL_DLL_SOURCE=../BundledLibraries/Google.GData.AccessControl.dll
GOOGLE_GDATA_CLIENT_DLL_SOURCE=../BundledLibraries/Google.GData.Client.dll
GCALENDAR_DLL_MDB_SOURCE=bin/Debug/GCalendar.dll.mdb
GCALENDAR_DLL_MDB=$(BUILD_DIR)/GCalendar.dll.mdb
GOOGLE_GDATA_CALENDAR_DLL_SOURCE=../BundledLibraries/Google.GData.Calendar.dll
GOOGLE_GDATA_EXTENSIONS_DLL_SOURCE=../BundledLibraries/Google.GData.Extensions.dll

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/GCalendar.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

GOOGLE_GDATA_ACCESSCONTROL_DLL_SOURCE=../BundledLibraries/Google.GData.AccessControl.dll
GOOGLE_GDATA_CLIENT_DLL_SOURCE=../BundledLibraries/Google.GData.Client.dll
GCALENDAR_DLL_MDB=
GOOGLE_GDATA_CALENDAR_DLL_SOURCE=../BundledLibraries/Google.GData.Calendar.dll
GOOGLE_GDATA_EXTENSIONS_DLL_SOURCE=../BundledLibraries/Google.GData.Extensions.dll

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(GOOGLE_GDATA_ACCESSCONTROL_DLL) \
	$(GOOGLE_GDATA_CLIENT_DLL) \
	$(GCALENDAR_DLL_MDB) \
	$(GOOGLE_GDATA_CALENDAR_DLL) \
	$(GOOGLE_GDATA_EXTENSIONS_DLL)  

LINUX_PKGCONFIG = \
	$(GCALENDAR_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/CertHandler.cs \
	src/Configuration.cs \
	src/GCalendarEventItem.cs \
	src/GCalendarItem.cs \
	src/GCalendarItemSource.cs \
	src/GCalendarNewEvent.cs \
	src/GCalendarSearchEvents.cs \
	src/GCal2.cs 

DATA_FILES = 

RESOURCES = \
	resources/calIcon.png \
	GCalendar.addin.xml 

EXTRAS = \
	../BundledLibraries/Google.GData.Calendar.dll \
	../BundledLibraries/Google.GData.AccessControl.dll \
	../BundledLibraries/Google.GData.Extensions.dll \
	../BundledLibraries/Google.GData.Client.dll \
	gcalendar.pc.in 

REFERENCES =  \
	System \
	Do.Addins \
	$(GTK_SHARP_20_LIBS) \
	Mono.Posix \
	System.Xml

DLL_REFERENCES =  \
	../BundledLibraries/Google.GData.AccessControl.dll \
	../BundledLibraries/Google.GData.Calendar.dll \
	../BundledLibraries/Google.GData.Client.dll \
	../BundledLibraries/Google.GData.Extensions.dll

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

GOOGLE_GDATA_ACCESSCONTROL_DLL = $(BUILD_DIR)/Google.GData.AccessControl.dll
GOOGLE_GDATA_CLIENT_DLL = $(BUILD_DIR)/Google.GData.Client.dll
GCALENDAR_PC = $(BUILD_DIR)/gcalendar.pc
GOOGLE_GDATA_CALENDAR_DLL = $(BUILD_DIR)/Google.GData.Calendar.dll
GOOGLE_GDATA_EXTENSIONS_DLL = $(BUILD_DIR)/Google.GData.Extensions.dll

$(eval $(call emit-deploy-target,GOOGLE_GDATA_ACCESSCONTROL_DLL))
$(eval $(call emit-deploy-target,GOOGLE_GDATA_CLIENT_DLL))
$(eval $(call emit-deploy-wrapper,GCALENDAR_PC,gcalendar.pc))
$(eval $(call emit-deploy-target,GOOGLE_GDATA_CALENDAR_DLL))
$(eval $(call emit-deploy-target,GOOGLE_GDATA_EXTENSIONS_DLL))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)

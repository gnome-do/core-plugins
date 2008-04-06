
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

GOOGLE_GDATA_ACCESSCONTROL_DLL_SOURCE=Google.GData.AccessControl.dll
GOOGLE_GDATA_CLIENT_DLL_SOURCE=Google.GData.Client.dll
GOOGLE_GDATA_CALENDAR_DLL_SOURCE=Google.GData.Calendar.dll
GCALENDAR_ADDIN_XML_SOURCE=src/GCalendar.addin.xml
GOOGLE_GDATA_EXTENSIONS_DLL_SOURCE=Google.GData.Extensions.dll

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/GCalendar.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

GOOGLE_GDATA_ACCESSCONTROL_DLL_SOURCE=Google.GData.AccessControl.dll
GOOGLE_GDATA_CLIENT_DLL_SOURCE=Google.GData.Client.dll
GOOGLE_GDATA_CALENDAR_DLL_SOURCE=Google.GData.Calendar.dll
GCALENDAR_ADDIN_XML_SOURCE=src/GCalendar.addin.xml
GOOGLE_GDATA_EXTENSIONS_DLL_SOURCE=Google.GData.Extensions.dll

endif

if ENABLE_REPO
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/GCalendar.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

GOOGLE_GDATA_ACCESSCONTROL_DLL_SOURCE=Google.GData.AccessControl.dll
GOOGLE_GDATA_CLIENT_DLL_SOURCE=Google.GData.Client.dll
GOOGLE_GDATA_CALENDAR_DLL_SOURCE=Google.GData.Calendar.dll
GCALENDAR_ADDIN_XML_SOURCE=src/GCalendar.addin.xml
GOOGLE_GDATA_EXTENSIONS_DLL_SOURCE=Google.GData.Extensions.dll

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=.resources.dll

PROGRAMFILES = \
	$(GOOGLE_GDATA_ACCESSCONTROL_DLL) \
	$(GOOGLE_GDATA_CLIENT_DLL) \
	$(GOOGLE_GDATA_CALENDAR_DLL) \
	$(GCALENDAR_ADDIN_XML) \
	$(GOOGLE_GDATA_EXTENSIONS_DLL)  

LINUX_PKGCONFIG = \
	$(GCALENDAR_PC)  


	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/DoGCal.cs \
	src/GCalendarEventItem.cs \
	src/GCalendarItem.cs \
	src/GCalendarItemSource.cs \
	src/GCalendarNewEvent.cs \
	src/GCalendarSearchEvents.cs 

DATA_FILES = \
	Google.GData.AccessControl.dll \
	Google.GData.Calendar.dll \
	Google.GData.Client.dll \
	Google.GData.Extensions.dll \
	src/GCalendar.addin.xml 

RESOURCES = 

EXTRAS = \
	gcalendar.pc.in 

REFERENCES =  \
	$(DOADDINS_LIBS) \
	System.Xml \
	System \
	$(GCONF_SHARP_20_LIBS)

DLL_REFERENCES =  \
	Google.GData.AccessControl.dll \
	Google.GData.Calendar.dll \
	Google.GData.Client.dll \
	Google.GData.Extensions.dll

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

GOOGLE_GDATA_ACCESSCONTROL_DLL = $(BUILD_DIR)/Google.GData.AccessControl.dll
GOOGLE_GDATA_CLIENT_DLL = $(BUILD_DIR)/Google.GData.Client.dll
GCALENDAR_PC = $(BUILD_DIR)/gcalendar.pc
GOOGLE_GDATA_CALENDAR_DLL = $(BUILD_DIR)/Google.GData.Calendar.dll
GCALENDAR_ADDIN_XML = $(BUILD_DIR)/GCalendar.addin.xml
GOOGLE_GDATA_EXTENSIONS_DLL = $(BUILD_DIR)/Google.GData.Extensions.dll

$(eval $(call emit-deploy-target,GOOGLE_GDATA_ACCESSCONTROL_DLL))
$(eval $(call emit-deploy-target,GOOGLE_GDATA_CLIENT_DLL))
$(eval $(call emit-deploy-wrapper,GCALENDAR_PC,gcalendar.pc))
$(eval $(call emit-deploy-target,GOOGLE_GDATA_CALENDAR_DLL))
$(eval $(call emit-deploy-target,GCALENDAR_ADDIN_XML))
$(eval $(call emit-deploy-target,GOOGLE_GDATA_EXTENSIONS_DLL))


$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(build_resx_resources) : %.resources: %.resx
	resgen2 '$<' '$@'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(dir $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)


EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/GoogleCalculator.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

GOOGLECALCULATORACTION_ADDIN_XML_SOURCE=src/GoogleCalculatorAction.addin.xml

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/GoogleCalculator.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

GOOGLECALCULATORACTION_ADDIN_XML_SOURCE=src/GoogleCalculatorAction.addin.xml

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=.resources.dll

PROGRAMFILES = \
	$(GOOGLECALCULATORACTION_ADDIN_XML)  

LINUX_PKGCONFIG = \
	$(GOOGLECALCULATOR_PC)  


	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	src/GoogleCalculatorAction.cs 

DATA_FILES = \
	src/GoogleCalculatorAction.addin.xml 

RESOURCES = 

EXTRAS = \
	googlecalculator.pc.in 

REFERENCES =  \
	System \
	$(DOADDINS_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

GOOGLECALCULATORACTION_ADDIN_XML = $(BUILD_DIR)/GoogleCalculatorAction.addin.xml
GOOGLECALCULATOR_PC = $(BUILD_DIR)/googlecalculator.pc

$(eval $(call emit-deploy-target,GOOGLECALCULATORACTION_ADDIN_XML))
$(eval $(call emit-deploy-wrapper,GOOGLECALCULATOR_PC,googlecalculator.pc))


$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(build_resx_resources) : %.resources: %.resx
	resgen2 '$<' '$@'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(dir $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)

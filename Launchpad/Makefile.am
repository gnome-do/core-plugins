SUBDIRS=src

ASSEMBLY_NAME=Launchpad
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

launchpad_references = $(DO_ADDINS_LIBS) $(DO_LIBS) -r:Mono.Posix.dll -r:System

launchpad_sources =  \
	AssemblyInfo.cs \
	LaunchpadCodeItem.cs \
	LaunchpadBugsItem.cs \
	LaunchpadBlueprintsItem.cs \
	LaunchpadIcons.cs \
	LaunchpadRegisterItem.cs \
	LaunchpadAction.cs \
	LaunchpadTranslationsItem.cs \
	LaunchpadItemSource.cs \
	LaunchpadAnswersItem.cs

launchpad_resources = \
	Resources/LaunchpadBlueprints.png \
	Resources/LaunchpadBugs.png \
	Resources/LaunchpadCode.png \
	Resources/LaunchpadRegister.png \
	Resources/LaunchpadTranslations.png \
	Resources/Launchpad.png \
	Resources/LaunchpadAnswers.png

plugin_DATA = $(ASSEMBLY)

launchpad_build_sources = $(addprefix $(srcdir)/src/, $(launchpad_sources))

all: $(ASSEMBLY)


launchpad_resource_args = $(addprefix -resource:$(srcdir)/, $(launchpad_resources))

$(ASSEMBLY): $(launchpad_build_sources)
	$(CSC) $(CSFLAGS) $(launchpad_references) $(launchpad_resource_args) -target:library -out:$@ $(launchpad_build_sources) 

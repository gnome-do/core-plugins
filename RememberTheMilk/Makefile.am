
EXTRA_DIST =



if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+ -debug "-define:DEBUG"

ASSEMBLY = bin/Debug/RTM.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =
BUILD_DIR = bin/Debug


endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize+
ASSEMBLY = bin/Release/RTM.dll
ASSEMBLY_MDB =
COMPILE_TARGET = library
PROJECT_REFERENCES =
BUILD_DIR = bin/Release


endif

AL=al2
SATELLITE_ASSEMBLY_NAME=.resources.dll

all: $(ASSEMBLY)

FILES = \
	gtk-gui/Do.Addins.RTM.Configuration.cs \
	gtk-gui/generated.cs \
	src/Configuration.cs \
	src/RTMCompleteTask.cs \
	src/RTM.cs \
	src/RTMDeleteTask.cs \
	src/RTMListItem.cs \
	src/RTMListItemSource.cs \
	src/RTMMoveTask.cs \
	src/RTMNewTask.cs \
	src/RTMPriorityItem.cs \
	src/RTMRenameTask.cs \
	src/RTMSetDue.cs \
	src/RTMSetPriority.cs \
	src/RTMTaskItem.cs \
	src/RtmNet/ApiKeyRequiredException.cs \
	src/RtmNet/Auth.cs \
	src/RtmNet/AuthenticationRequiredException.cs \
	src/RtmNet/Categories.cs \
	src/RtmNet/Contacts.cs \
	src/RtmNet/DateGranularity.cs \
	src/RtmNet/Enums.cs \
	src/RtmNet/Groups.cs \
	src/RtmNet/GroupSearchResults.cs \
	src/RtmNet/List.cs \
	src/RtmNet/Methods.cs \
	src/RtmNet/Note.cs \
	src/RtmNet/Response.cs \
	src/RtmNet/ResponseXmlException.cs \
	src/RtmNet/RtmApiException.cs \
	src/RtmNet/Rtm.cs \
	src/RtmNet/RtmException.cs \
	src/RtmNet/RtmWebException.cs \
	src/RtmNet/SignatureRequiredException.cs \
	src/RtmNet/Tags.cs \
	src/RtmNet/Task.cs \
	src/RtmNet/User.cs \
	src/RtmNet/Utils.cs

RESOURCES = \
	RTM.addin.xml \
	resources/rtm.png \
	resources/task-add.png \
	resources/task-complete.png \
	resources/task-delete.png \
	resources/task-due.png \
	resources/task-high-due.png \
	resources/task-high-overdue.png \
	resources/task-high.png \
	resources/task-low-due.png \
	resources/task-low-overdue.png \
	resources/task-low.png \
	resources/task-medium-due.png \
	resources/task-medium-overdue.png \
	resources/task-medium.png \
	resources/task-overdue.png \
	resources/task.png \
	resources/task-priority-down.png \
	resources/task-priority.png \
	resources/task-priority-up.png \
	resources/task-rename.png \
	resources/task-setdue.png

DATA_FILES =

REFERENCES =  \
	System \
	System.Core \
	System.Data \
	System.Web \
	System.Xml \
	Mono.Posix \
	$(GNOME_KEYRING_SHARP_LIBS) \
	$(DOADDINS_LIBS) \
	$(GTK_SHARP_20_LIBS)

DLL_REFERENCES =

CLEANFILES =

include $(top_srcdir)/Makefile.include


$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(build_resx_resources) : %.resources: %.resx
	resgen2 '$<' '$@'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(dir $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)

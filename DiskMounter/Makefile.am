EXTRA_DIST = MountAction.cs \
             UnmountAction.cs \
             OpenVolumeAction.cs \
             DriveItemSource.cs \
             MountedDriveItem.cs \
             UnmountedDriveItem.cs 
             
ASSEMBLY_NAME=DiskMounter
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

diskmounter_references = $(DO_ADDINS_LIBS) $(GNOME_VFS_SHARP_LIBS) -r:System

diskmounter_sources  = 	MountAction.cs \
                        UnmountAction.cs \
                        OpenVolumeAction.cs \
                        DriveItemSource.cs \
                        MountedDriveItem.cs \
                        UnmountedDriveItem.cs 

diskmounter_build_sources = $(addprefix $(srcdir)/, $(diskmounter_sources))

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(diskmounter_build_sources)
	$(CSC) $(CSFLAGS) $(diskmounter_references) -target:library -out:$@ $(diskmounter_build_sources)

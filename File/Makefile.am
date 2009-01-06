include $(top_srcdir)/build.rules.mk

ASSEMBLY=File

FILES = \
	gtk-gui/generated.cs \
	gtk-gui/Do.FilesAndFolders.Configuration.cs \
	src/PathNodeView.cs \
	src/Do.FilesAndFolders/ImportantFolders.cs \
	src/Do.FilesAndFolders/FileItemSource.cs \
	src/Do.FilesAndFolders/RecentFileItemSource.cs \
	src/Do.FilesAndFolders/IndexedFolderCollection.cs \
	src/Do.FilesAndFolders/Preferences.cs \
	src/Do.FilesAndFolders/Plugin.cs \
	src/Do.FilesAndFolders/Configuration.cs \
	src/Do.FilesAndFolders/NewFileAction.cs \
	src/Do.FilesAndFolders/IndexedFolder.cs \
	src/Do.FilesAndFolders/NewFolderAction.cs

RESOURCES = \
	Resources/File.addin.xml \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml 

EXTRAS = \
	file.pc.in 

REFERENCES = \
	System \
	System.Core \
	Mono.Posix \
	$(DO_PLATFORM_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS) \
	$(GTK_SHARP_20_LIBS) \
	$(GLIB_SHARP_20_LIBS)

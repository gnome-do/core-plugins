include $(top_srcdir)/build.rules.mk

ASSEMBLY=Dropbox

FILES = \
  gtk-gui/Dropbox.DropboxConfig.cs \
  gtk-gui/generated.cs \
  src/Config/DropboxConfig.cs \
  src/DropboxWebInterfaceAction.cs \
  src/DropboxAbstractAction.cs \
  src/DropboxRevisionsAction.cs \
  src/DropboxPuburlAction.cs \
  src/DropboxUnshareAction.cs \
  src/DropboxShareAction.cs \
  src/DropboxStartAction.cs \
  src/DropboxStopAction.cs \
  src/DropboxLinkAction.cs \
  src/Dropbox.cs

RESOURCES = \
  Resources/Dropbox.addin.xml \
  gtk-gui/gui.stetic

REFERENCES = \
  System \
  System.Core \
  $(DO_PLATFORM_LINUX_LIBS) \
  $(DO_UNIVERSE_LIBS) \
  $(GTK_SHARP_20_LIBS)

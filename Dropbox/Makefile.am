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
  gtk-gui/gui.stetic \
  Resources/icons/dropbox-add.png \
  Resources/icons/dropbox-remove.png \
  Resources/icons/dropbox-revisions.png \
  Resources/icons/dropbox-share.png \
  Resources/icons/dropbox-start.png \
  Resources/icons/dropbox-stop.png \
  Resources/icons/dropbox-web.png

REFERENCES = \
  System \
  System.Core \
  Mono.Posix \
  $(DO_PLATFORM_LINUX_LIBS) \
  $(DO_UNIVERSE_LIBS) \
  $(GTK_SHARP_20_LIBS)

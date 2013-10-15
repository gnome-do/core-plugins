include $(top_srcdir)/build.rules.mk

ASSEMBLY=Transmission

FILES = \
  gtk-gui/Transmission.TransmissionConfig.cs \
  gtk-gui/generated.cs \
  src/Config/TransmissionConfig.cs \
  src/ITorrentEntry.cs \
  src/Utils.cs \
  src/TorrentDirectoryItem.cs \
  src/TorrentFileItem.cs \
  src/TorrentFileSetPriorityAction.cs \
  src/TorrentItem.cs \
  src/TorrentItemSource.cs \
  src/TorrentAbstractLimitSpeedAction.cs \
  src/TorrentLimitDownloadSpeedAction.cs \
  src/TorrentLimitUploadSpeedAction.cs \
  src/TorrentMarkForDownloadAction.cs \
  src/TorrentStartAction.cs \
  src/TorrentStopAction.cs \
  src/TorrentUnmarkForDownloadAction.cs \
  src/TorrentVerifyAction.cs \
  src/TorrentOperateAction.cs \
  src/TransmissionAPI.cs \
  src/TransmissionPlugin.cs

GENERATED_RESOURCES = Resources/Transmission.addin.xml

RESOURCES = \
  gtk-gui/gui.stetic \
  Resources/icons/transmission.png

REFERENCES = \
  System \
  System.Core \
  $(NEWTONSOFT_JSON_LIBS) \
  $(DO_PLATFORM_LINUX_LIBS) \
  $(DO_UNIVERSE_LIBS) \
  $(GTK_SHARP_20_LIBS)

include $(top_srcdir)/build.rules.mk

ASSEMBLY=Skype

FILES = \
	src/StatusItem.cs \
	src/SkypeContactItemSource.cs \
	src/SkypeContactDetailItem.cs \
	src/SkypeBrowseBuddyItem.cs \
	src/SkypeBrowseStatusItem.cs \
	src/SkypeCallAction.cs \
	src/SkypeChatAction.cs \
	src/SkypeStatusItemSource.cs \
	src/SkypeSetStatusAction.cs \
	src/Skype.cs

RESOURCES = \
	Resources/Skype.addin.xml \
	Resources/icons/CallStart.png \
	Resources/icons/Message.png \
	Resources/icons/SkypeOut.png \
	Resources/icons/StatusAway.png \
	Resources/icons/StatusDoNotDisturb.png \
	Resources/icons/StatusInvisible.png \
	Resources/icons/StatusNotAvailable.png \
	Resources/icons/StatusOffline.png \
	Resources/icons/StatusOnline.png \
	Resources/icons/StatusPending.png \
	Resources/icons/StatusSkypeMe.png

BUILD_DEFINES=$(DBUS_BUILD_DEFINES)

REFERENCES = \
	System \
	System.Core \
	$(DBUS_SHARP_LIBS) \
	$(DBUS_SHARP_GLIB_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS)


include $(top_srcdir)/build.rules.mk

ASSEMBLY=Pidgin

FILES = \
	src/Pidgin.cs \
	src/PidginChatAction.cs \
	src/PidginContactItemSource.cs \
	src/PidginHandleContactDetailItem.cs \
	src/PidginSavedStatusItemSource.cs \
	src/PidginAccountItemSource.cs \
	src/PidginSavedStatusItem.cs \
	src/PidginAccountItem.cs \
	src/PidginSetStatusAction.cs \
	src/PidginStatusTypeItem.cs \
	src/PidginAccountActions.cs \
	src/PidginBrowseBuddyItem.cs \
	src/PidginBrowseAccountItem.cs \
	src/PidginBrowseStatusItem.cs

RESOURCES = \
	Resources/Pidgin.addin.xml \
	Resources/icons/internet-group-chat.svg

BUILD_DEFINES=$(DBUS_BUILD_DEFINES)

REFERENCES = \
	System \
	System.Xml \
	System.Core \
	$(DBUS_SHARP_LIBS) \
	$(DBUS_SHARP_GLIB_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS) 

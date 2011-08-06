include $(top_srcdir)/build.rules.mk

ASSEMBLY=Emesene

FILES = \
    src/EmeseneAvatarItem.cs \
    src/EmeseneContactItemSource.cs \
    src/EmeseneAvatarItemSource.cs \
    src/Emesene.cs \
    src/EmeseneChangeAvatarAction.cs \
    src/EmeseneOpenAvatarHistoryAction.cs \
    src/EmeseneChangeNickAction.cs \
    src/EmeseneOpenConversationHistoryAction.cs \
    src/EmeseneChangePSMAction.cs \
    src/EmeseneStatusItem.cs \
    src/EmeseneChangeStatusAction.cs \
    src/EmeseneStatusItemSource.cs \
    src/EmeseneChatAction.cs


RESOURCES = \
	Resources/Emesene.addin.xml \
    Resources/statusIcons/away.png \
    Resources/statusIcons/busy.png \
    Resources/statusIcons/invisible.png \
    Resources/statusIcons/offline.png \
    Resources/statusIcons/phone.png \
    Resources/statusIcons/brb.png \
    Resources/statusIcons/idle.png \
    Resources/statusIcons/lunch.png \
    Resources/statusIcons/online.png

BUILD_DEFINES=$(DBUS_BUILD_DEFINES)

REFERENCES = \
	System \
	System.Core \
	System.Xml \
	$(DBUS_SHARP_LIBS) \
	$(DBUS_SHARP_GLIB_LIBS) \
	$(DO_UNIVERSE_LIBS) \
	$(DO_PLATFORM_LIBS)


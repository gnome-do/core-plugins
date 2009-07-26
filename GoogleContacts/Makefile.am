if HAVE_GMAIL
include $(top_srcdir)/build.rules.mk
endif

ASSEMBLY=GoogleContacts

FILES = \
	gtk-gui/generated.cs \
	src/CertHandler.cs \
	src/GMailClient.cs \
	src/GMailConfig.cs \
	src/GMailContactDetailItem.cs \
	src/GMailContactItemSource.cs \
	src/GMail.cs \
	src/Preferences.cs \
	src/RecentConversationsAction.cs

RESOURCES = \
	Resources/GoogleContacts.addin.xml \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml \
	Resources/icons/gmail-logo.png \
	Resources/icons/phone.png

REFERENCES = \
	System \
	System.Core \
	System.Security \
	$(GTK_SHARP_20_LIBS) \
	$(GDATA_SHARP_CONTACTS_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS)

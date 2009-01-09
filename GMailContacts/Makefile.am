include $(top_srcdir)/build.rules.mk

ASSEMBLY=GMailContacts

FILES = \
	gtk-gui/generated.cs \
	src/CertHandler.cs \
	src/GMailClient.cs \
	src/GMailConfig.cs \
	src/GMailContactDetailItem.cs \
	src/GMailContactItemSource.cs \
	src/GMail.cs \
	src/Preferences.cs

RESOURCES = \
	Resources/GMailContacts.addin.xml \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml \
	resources/gmail-logo.png \
	resources/phone.png

REFERENCES = \
	Mono.Posix \
	System \
	System.Core \
	System.Security \
	$(GTK_SHARP_20_LIBS) \
	$(GDATA_SHARP_ACL_LIBS) \
	$(GDATA_SHARP_CONTACTS_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS)

if HAVE_EMPATHY
include $(top_srcdir)/build.rules.mk
endif

ASSEMBLY=Empathy

FILES = \
	src/Account.cs \
	src/AccountManagerQuery.cs \
	src/Contact.cs \
	src/EmpathyAccountActions.cs \
	src/EmpathyAccountItem.cs \
	src/EmpathyAccountItemSource.cs \
	src/EmpathyBrowseAccountItem.cs \
	src/EmpathyBrowseBuddyItem.cs \
	src/EmpathyBrowseStatusItem.cs \
	src/EmpathyChatAction.cs \
	src/EmpathyContactItemSource.cs \
	src/EmpathyPlugin.cs \
	src/EmpathySavedStatusItem.cs \
	src/EmpathySavedStatusItemSource.cs \
	src/EmpathySetStatusAction.cs \
	src/EmpathyStatusItem.cs \
	src/Status.cs

RESOURCES = \
	Resources/Empathy.addin.xml

REFERENCES = \
	$(DBUS_SHARP_LIBS) \
	$(DBUS_SHARP_GLIB_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS) \
	$(MONO_ADDINS_LIBS) \
	$(TELEPATHY_SHARP_LIBS) \
	System \
	System.Core \
	System.Xml

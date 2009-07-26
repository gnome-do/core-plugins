if HAVE_EVOLUTION
include $(top_srcdir)/build.rules.mk
endif

ASSEMBLY=Evolution

FILES = \
	src/AddressContactDetailItem.cs \
	src/ContactDetailItem.cs \
	src/ContactItemSource.cs \
	src/EmailContactDetailItem.cs \
	src/PhoneContactDetailItem.cs \
	src/AddressContactDetailItem.cs 

RESOURCES = \
	Resources/Evolution.addin.xml \
	Resources/icons/phone.png

REFERENCES = \
	System \
	System.Core \
	$(EVOLUTION_SHARP_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS)

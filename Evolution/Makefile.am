include $(top_srcdir)/build.rules.mk

ASSEMBLY=Evolution

FILES = \
	src/ContactItemSource.cs \
	src/ContactDetailItem.cs \
	src/EmailContactDetailItem.cs \
	src/PhoneContactDetailItem.cs \
	src/AddressContactDetailItem.cs 

RESOURCES = \
	Resources/phone.png \
	Resources/Evolution.addin.xml 

EXTRAS = \
	evolution.pc.in 

REFERENCES = \
	System \
	System.Core \
	Mono.Posix \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS) \
	$(EVOLUTION_SHARP_LIBS)


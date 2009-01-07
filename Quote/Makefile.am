include $(top_srcdir)/build.rules.mk

ASSEMBLY=Quote

FILES = \
	src/Quote.cs \
	src/QuoteAction.cs \
	src/QuoteTagItem.cs \
	src/Providers/Bubash.cs \
	src/Providers/IQuoteProvider.cs \
	src/Providers/QuoteProviderFactory.cs

RESOURCES = \
	Quote.addin.xml \
	quoted-globe.svg

REFERENCES = \
	Mono.Posix \
	System \
	System.Core \
	System.Web \
	$(GTK_SHARP_20_LIBS) \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS)

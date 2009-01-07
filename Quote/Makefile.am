include $(top_srcdir)/build.rules.mk

ASSEMBLY=Quote

FILES = \
	src/QuoteAction.cs \
	src/Quote.cs \
	src/Providers/IQuoteProvider.cs \
	src/Providers/Bubash.cs \
	src/Providers/QuoteProviderFactory.cs \
	src/TextSyntax/QuoteTagItem.cs

RESOURCES = \
	src/Quote.addin.xml \
	quoted-globe.svg

REFERENCES = \
	Mono.Posix \
	System \
	System.Core \
	System.Web \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS)

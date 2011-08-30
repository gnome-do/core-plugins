include $(top_srcdir)/build.rules.mk

ASSEMBLY=XRandR

FILES = \
	src/XRandRLib.cs \
	src/XRandRWrapper.cs \
	src/OutputItem.cs \
	src/OutputModeItem.cs \
	src/XRandRItemSource.cs

DLL_CONFIG_FILES = \
	XRandR.dll.config

RESOURCES = \
	Resources/XRandR.addin.xml

REFERENCES = \
	System \
	System.Core \
	Mono.Posix \
	$(DO_PLATFORM_LIBS) \
	$(DO_UNIVERSE_LIBS)

include $(top_srcdir)/build.rules.mk

ASSEMBLY=Pastebin

FILES = \
	gtk-gui/generated.cs \
	gtk-gui/Pastebin.PastebinConfig.cs \
	src/AssemblyInfo.cs \
	src/Config/PastebinConfig.cs \
	src/PastebinAction.cs \
	src/Pastebin.cs \
	src/Providers/IPastebinProvider.cs \
	src/Providers/LodgeIt.cs \
	src/Providers/Paste2.cs \
	src/Providers/PastebinCA.cs \
	src/Providers/PastebinProviderFactory.cs \
	src/TextSyntax/ITextSyntaxItem.cs \
	src/TextSyntax/TextSyntaxItem.cs

RESOURCES = \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml \
	src/Pastebin.addin.xml

REFERENCES = \
	Mono.Posix \
	System \
	System.Core \
	System.Web \
	$(GTK_SHARP_20_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS)

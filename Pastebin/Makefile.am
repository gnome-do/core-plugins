include $(top_srcdir)/build.rules.mk

ASSEMBLY=Pastebin

FILES = \
	gtk-gui/generated.cs \
	gtk-gui/Pastebin.PastebinConfig.cs \
	src/Config/PastebinConfig.cs \
	src/PastebinAction.cs \
	src/Pastebin.cs \
	src/Providers/IPastebinProvider.cs \
	src/Providers/AbstractPastebinProvider.cs \
	src/Providers/LodgeIt.cs \
	src/Providers/Paste2.cs \
	src/Providers/PasteUbuntu.cs \
	src/Providers/PastebinCA.cs \
	src/Providers/PastebinCOM.cs \
	src/Providers/PastebinProviderFactory.cs \
	src/TextSyntax/ITextSyntaxItem.cs \
	src/TextSyntax/TextSyntaxItem.cs

RESOURCES = \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml \
	Resources/Pastebin.addin.xml \
	Resources/LodgeIt.xml \
	Resources/Paste2.xml \
	Resources/PasteUbuntu.xml \
	Resources/PastebinCA.xml \
	Resources/Pastebin.xml

REFERENCES = \
	System \
	System.Core \
	System.Web \
	System.Xml \
	$(GTK_SHARP_20_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS)

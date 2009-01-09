include $(top_srcdir)/build.rules.mk

ASSEMBLY=GDocs

FILES = \
	src/CertHandler.cs \
	src/Configuration.cs \
	src/GDocs.cs \
	src/GDocsDocumentItem.cs \
	src/GDocsItem.cs \
	src/GDocsItemSource.cs \
	src/GDocsPDFItem.cs \
	src/GDocsPresentationItem.cs \
	src/GDocsSpreadsheetItem.cs \
	src/GDocsTrashDocument.cs \
	src/GDocsUploadDocument.cs \
	src/Preferences.cs

RESOURCES = \
	Resources/GDocs.addin.xml \
	Resources/gDocsIcon.png \
	Resources/gDocsTrashIcon.png \
	Resources/gDocsUploadIcon.png

REFERENCES = \
	Mono.Posix \
	System \
	System.Core \
	System.Xml \
	$(GDATA_SHARP_ACL_LIBS) \
	$(GDATA_SHARP_DOCUMENTS_LIBS) \
	$(GTK_SHARP_20_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS)

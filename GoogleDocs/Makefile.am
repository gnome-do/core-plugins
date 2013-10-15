if HAVE_GDOCS
include $(top_srcdir)/build.rules.mk
endif

ASSEMBLY=GoogleDocs

FILES = \
	src/CertHandler.cs \
	src/Configuration.cs \
	src/GDocs.cs \
	src/GDocsAbstractItem.cs \
	src/GDocsDocumentItem.cs \
	src/GDocsItemSource.cs \
	src/GDocsPDFItem.cs \
	src/GDocsPresentationItem.cs \
	src/GDocsSpreadsheetItem.cs \
	src/GDocsTrashDocument.cs \
	src/GDocsUploadDocument.cs \
	src/Preferences.cs

GENERATED_RESOURCES = \
	Resources/GoogleDocs.addin.xml
RESOURCES = \
	Resources/icons/gDocsIcon.png \
	Resources/icons/gDocsTrashIcon.png \
	Resources/icons/gDocsUploadIcon.png

REFERENCES = \
	System \
	System.Core \
	System.Xml \
	$(GDATA_SHARP_DOCUMENTS_LIBS) \
	$(GTK_SHARP_20_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS)

if HAVE_FLICKR
include $(top_srcdir)/build.rules.mk
endif

ASSEMBLY=Flickr

FILES = \
	gtk-gui/Flickr.AccountConfig.cs \
	gtk-gui/Flickr.UploadConfig.cs \
	gtk-gui/Flickr.UploadDialog.cs \
	gtk-gui/generated.cs \
	src/AccountConfig.cs \
	src/FlickrItemSource.cs \
	src/UploadAction.cs \
	src/UploadPool.cs \
	src/UploadDialog.cs \
	src/UploadConfig.cs

GENERATED_RESOURCES = \
	Resources/Flickr.addin.xml
RESOURCES = \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml \
	Resources/icons/flickr.png

REFERENCES = \
	System \
	System.Core \
	$(GTK_SHARP_20_LIBS) \
	$(FLICKRNET_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS)

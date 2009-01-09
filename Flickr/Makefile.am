include $(top_srcdir)/build.rules.mk

ASSEMBLY=Flickr

FILES = \
	gtk-gui/Flickr.AccountConfig.cs \
	gtk-gui/Flickr.UploadConfig.cs \
	gtk-gui/generated.cs \
	src/AccountConfig.cs \
	src/FlickrItemSource.cs \
	src/UploadAction.cs \
	src/UploadConfig.cs

RESOURCES = \
	Resources/Flickr.addin.xml \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml \
	resources/flickr.png

REFERENCES = \
	Mono.Posix \
	System \
	System.Core \
	$(GTK_SHARP_20_LIBS) \
	$(FLICKRNET_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS)

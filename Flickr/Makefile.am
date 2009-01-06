include $(top_srcdir)/build.rules.mk

ASSEMBLY=Flickr

FILES = \
	src/UploadAction.cs \
	gtk-gui/generated.cs \
	src/AccountConfig.cs \
	src/FlickrItemSource.cs \
	gtk-gui/Flickr.AccountConfig.cs \
	gtk-gui/Flickr.UploadConfig.cs \
	src/UploadConfig.cs 

RESOURCES = \
	Flickr.addin.xml \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml \
	resources/flickr.png 

EXTRAS = \
	flickr.pc.in 

REFERENCES =  \
	System \
	Mono.Posix \
	$(DO_PLATFORM_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS)
	$(FLICKRNET_LIBS) \
	$(GTK_SHARP_20_LIBS)


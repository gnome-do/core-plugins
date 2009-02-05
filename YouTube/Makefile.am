include $(top_srcdir)/build.rules.mk

ASSEMBLY=YouTube

FILES = \
    src/Preferences.cs \
	src/Youtube.cs \
	src/YoutubeConfig.cs \
	src/YouTubeFavoriteItemSource.cs \
	src/YouTubeOwnVideosItemSource.cs \
	src/YouTubeSearchAction.cs \
	src/YouTubeSubscriptionItem.cs \
	src/YouTubeSubscriptionItemSource.cs \
	src/YoutubeVideoItem.cs

RESOURCES = \
	Resources/Youtube.addin.xml \
	Resources/youtube_user.png \
	Resources/youtube_logo.png

REFERENCES = \
	System \
	System.Core \
	Mono.Posix \
	$(GDATA_SHARP_CONTACTS_LIBS) \
	$(top_srcdir)/BundledLibraries/Google.GData.YouTube.dll \
	$(DO_PLATFORM_LIBS) \
	$(GTK_SHARP_20_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \	
	$(DO_UNIVERSE_LIBS)

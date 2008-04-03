ENABLED_SUBDIRS =

if ENABLE_EVO_PLUGIN
ENABLED_SUBDIRS += Evolution
endif

if ENABLE_AMAROK_PLUGIN
ENABLED_SUBDIRS += Amarok
endif

if ENABLE_OPENSEARCH_PLUGIN
ENABLED_SUBDIRS += OpenSearch
endif

if ENABLE_PASTEBIN_PLUGIN
ENABLED_SUBDIRS += Pastebin
endif

if ENABLE_BANSHEE_PLUGIN
ENABLED_SUBDIRS += Banshee
endif

if ENABLE_SSH_PLUGIN
ENABLED_SUBDIRS += SSH
endif

if ENABLE_LAUNCHPAD_PLUGIN
ENABLED_SUBDIRS += Launchpad
endif

if ENABLE_TOMBOY_PLUGIN
ENABLED_SUBDIRS += Tomboy
endif

if ENABLE_TWITTER_PLUGIN
ENABLED_SUBDIRS += Twitter
endif

if ENABLE_TASQUE_PLUGIN
ENABLED_SUBDIRS += Tasque
endif

SUBDIRS = $(ENABLED_SUBDIRS) 	\
			Epiphany			\
			File				\
			GCalendar		\
			GNOME-Screenshot	\
			GNOME-Session		\
			GNOME-Terminal		\
			GoogleCalculator	\
			LocateFiles			\
			Pidgin				\
			Rhythmbox			\
			SimplePlugins		\
			Tasque                  \
			Templates			\
			Thunderbird		\
			Vinagre

EXTRA_DIST = DoPlugins.mds Packages.mdse COPYRIGHT

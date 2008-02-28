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

SUBDIRS = $(ENABLED_SUBDIRS) Epiphany Rhythmbox Pidgin LocateFiles Thunderbird \
	  GoogleCalculator Templates GNOME-Session SimplePlugins AppendText \
	  Archive File

EXTRA_DIST = DoPlugins.mds Packages.mdse COPYRIGHT

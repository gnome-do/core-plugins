EXTRA_DIST = 	\
			Twitter.cs \
			Notifications.cs \
			TwitterFriendItem.cs \
			TwitterFriendItemSource.cs \
			twitter-icon.png

ASSEMBLY_NAME=Twitter
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

twitter_references = $(DO_ADDINS_LIBS) $(DO_LIBS) -r:System -r:Mono.Posix $(NDESK_DBUS_LIBS) $(GCONF_SHARP_LIBS)

twitter_sources  = 	\
			Twitter.cs \
			Notifications.cs \
			TwitterFriendItem.cs \
			TwitterFriendItemSource.cs

twitter_resources =     \
			twitter-icon.png

twitter_build_sources = $(addprefix $(srcdir)/, $(twitter_sources))

twitter_resource_args = $(addprefix -resource:$(srcdir)/, $(twitter_resources))

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(twitter_build_sources)
	$(CSC) $(CSFLAGS) $(twitter_references) $(twitter_resource_args) -target:library -out:$@ $(twitter_build_sources)

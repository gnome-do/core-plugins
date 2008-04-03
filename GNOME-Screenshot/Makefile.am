EXTRA_DIST = CurrentWindowScreenshotItem.cs   \
		    	ScreenshotDelayItem.cs			 \
		    	ScreenshotItem.cs				 \
		    	ScreenshotItemSource.cs			 \
		    	TakeScreenshotAction.cs			 \
		    	WholeScreenScreenshotItem.cs

ASSEMBLY_NAME=GNOME-Screenshot
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

gnome_screenshot_references = $(DO_ADDINS_LIBS)

gnome_screenshot_sources  = 	\
	CurrentWindowScreenshotItem.cs   \
	ScreenshotDelayItem.cs			 \
	ScreenshotItem.cs				 \
	ScreenshotItemSource.cs			 \
	TakeScreenshotAction.cs			 \
	WholeScreenScreenshotItem.cs

gnome_screenshot_build_sources = $(addprefix $(srcdir)/, $(gnome_screenshot_sources))

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(gnome_screenshot_sources)
	$(CSC) $(CSFLAGS) $(gnome_screenshot_references) -target:library -out:$@ $(gnome_screenshot_build_sources)

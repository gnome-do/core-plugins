include $(top_srcdir)/build.rules.mk

ASSEMBLY=JIRA

FILES = \
	gtk-gui/generated.cs \
	gtk-gui/JIRA.ConfigWidget.cs \
	src/JIRAIssueItem.cs \
	src/ConfigWidget.cs \
	src/JIRAIssueAction.cs \
	src/Remote/IJIRAServerFacade.cs \
	src/Remote/JIRARssClient.cs \
	src/Remote/JIRAServerFacade.cs \
	src/Remote/AtlassianJiraSoapService.cs \
	src/JIRAConfiguration.cs \
	src/JIRAIssueSource.cs

GENERATED_RESOURCES = \
	Resources/JIRA.addin.xml
RESOURCES = \
	Resources/icons/jira.png

REFERENCES = \
	System \
	System.Core \
	System.Web.Services \
	System.Xml \
	$(DO_PLATFORM_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS) \
	$(GTK_SHARP_20_LIBS) \
	$(GCONF_SHARP_20_LIBS) 

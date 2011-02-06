include $(top_srcdir)/build.rules.mk

ASSEMBLY=telepathy-sharp

FILES = \
	telepathy-sharp/AssemblyInfo.cs \
	telepathy-sharp/Constants.cs \
	telepathy-sharp/MissionControl.cs \
	telepathy-sharp/Telepathy.cs

REFERENCES = \
	$(DBUS_SHARP_LIBS) \
	$(DBUS_SHARP_GLIB_LIBS)

# Include all of telepathy-sharp files, even though we don't
# use them
EXTRA_DIST += $(addprefix, $(srcdir),\
	AUTHORS \
	LICENSE \
	PROVIDENCE \
	README \
	telpathy-sharp.sln \
	telepathy-sharp/telepathy-sharp.csproj \
	tests/AssemblyInfo.cs \
	tests/DTubeTest.cs \
	tests/FileTransfer.cs \
	tests/Main.cs \
	tests/McTest.cs \
	tests/MiscTest.cs \
	tests.csproj \
	)
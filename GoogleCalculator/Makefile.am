EXTRA_DIST = GoogleCalculatorAction.cs

ASSEMBLY_NAME=GoogleCalculatorAction
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

google_calculator_references = $(DO_ADDINS_LIBS)

google_calculator_sources  = 	\
			GoogleCalculatorAction.cs

google_calculator_build_sources = $(addprefix $(srcdir)/, $(google_calculator_sources))

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(google_calculator_sources)
	$(CSC) $(CSFLAGS) $(google_calculator_references) -target:library -out:$@ $(google_calculator_build_sources)

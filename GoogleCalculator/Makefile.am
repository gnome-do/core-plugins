ASSEMBLY_NAME=GoogleCalculatorAction
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

google_calculator_references = $(DO_ADDINS_LIBS)

google_calculator_sources  = 	\
			GoogleCalculatorAction.cs

all: $(ASSEMBLY)

$(ASSEMBLY): $(google_calculator_sources)
	$(CSC) $(CSFLAGS) $(google_calculator_references) -target:library -out:$@ $(google_calculator_sources)

install-exec-local: $(ASSEMBLY)
	mkdir -p $(plugin_path)
	install -m 644 $(ASSEMBLY) $(plugin_path)

uninstall-local:
	rm $(plugin_path)/$(ASSEMBLY)

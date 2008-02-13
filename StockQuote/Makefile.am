EXTRA_DIST = StockQuoteAction.cs

ASSEMBLY_NAME=StockQuoteAction
ASSEMBLY=$(ASSEMBLY_NAME).dll

CSFLAGS+= -debug+ -debug:full

CLEANFILES = $(ASSEMBLY){,.mdb}

stock_quote_references = $(DO_ADDINS_LIBS)

stock_quote_sources  = 	\
			StockQuoteAction.cs

stock_quote_build_sources = $(addprefix $(srcdir)/, $(stock_quote_sources))

plugin_DATA = $(ASSEMBLY)

all: $(ASSEMBLY)

$(ASSEMBLY): $(stock_quote_sources)
	$(CSC) $(CSFLAGS) $(stock_quote_references) -target:library -out:$@ $(stock_quote_build_sources)

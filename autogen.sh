#!/bin/sh
# Run this to generate all the initial makefiles, etc.

srcdir=`dirname $0`
test -z "$srcdir" && srcdir=.

PKG_NAME="gnome-do-plugins"
ACLOCAL_FLAGS="-I m4/shamrock $ACLOCAL_FLAGS"
REQUIRED_M4MACROS="i18n.m4"

(test -f $srcdir/configure.ac \
  && test -d $srcdir/File) || {
    echo -n "**Error**: Directory "\`$srcdir\'" does not look like the"
    echo " top-level gnome-do-plugins directory"
    exit 1
}

which gnome-autogen.sh || {
    echo "You need to install gnome-common from the GNOME CVS"
    exit 1
}
USE_COMMON_DOC_BUILD=yes . gnome-autogen.sh

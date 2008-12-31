#!/bin/sh

RESOURCES=$(egrep "EmbedAsResource" $1 | \
            egrep -o "\".*\.(xml|png|stetic)\"" | \
            sed s/\"// | sed s/\"// | sort)

OLD_IFS=$IFS
IFS="
"
echo "RESOURCES = \\" >> Makefile.am
for RESOURCE in $RESOURCES ; do
    echo "\t$RESOURCE \\" >> Makefile.am
done
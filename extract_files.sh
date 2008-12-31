#!/bin/sh

FILES=$(egrep "subtype=\"Code\" buildaction=\"Compile\"" $1 | \
        egrep -o "\".*\.cs" | \
        sed s/\"// | sort)

OLD_IFS=$IFS
IFS="
"
echo "FILES = \\" >> Makefile.am
for FILE in $FILES ; do
    echo "\t$FILE \\" >> Makefile.am
done
#!/bin/sh

REFERENCES=$(egrep "ProjectReference" $1 | \
            egrep -o "refto=\"[[:alnum:].]+," | \
            sed s/refto\"// | sed s/,// | sort)

OLD_IFS=$IFS
IFS="
"
echo "REFERENCES = \\" >> Makefile.am
for REFERENCE in $REFERENCES ; do
    echo "\t$REFERENCE \\" >> Makefile.am
done
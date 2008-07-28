#!/usr/bin/python

"""
To build plugins from MonoDevelop solution:
	$ ./repo.py --make

To build the Mono.Addins repository in folder "repo":
	$ ./repo.py --build

To publish the repository (example publishes to community repo v0.4):
	$ ./repo.py --publish

To do everything:
	$ ./repo.py --make --build --publish
"""

from os import system
from os.path import commonprefix, abspath
from subprocess import Popen, PIPE

REPO_VERSION = "0.5.97"
REPO_NAME = "official"
REPO_SCP = "gnomedo@do.davebsd.com:do.davebsd.com/repo/%s/%s" % \
	(REPO_VERSION, REPO_NAME)

REPO_DIR = abspath ("repo")
REPO_CONFIG = "Debug"

def main (argv):
	if "--clean" in argv or "--all" in argv:
		clean ()
	if "--make" in argv or "--all" in argv:
		make ()
	if "--build" in argv or "--all" in argv:
		build ()
	if "--publish" in argv or "--all" in argv:
		publish ()

def clean ():
	system ("rm -rf repo */bin")

def make ():
	system ("mdtool build")

def build ():
	repo = escape (REPO_DIR)
	system ("rm -rf %s" % repo)
	system ("mkdir -p %s" % repo)
	for asm in assemblies ():
		system ("cp %s %s" % (escape (asm), repo))
		for man in manifests (asm):
			system ("cp %s %s" % (escape (man), repo))
	system ("mdtool setup pack %s/*.addin.xml -d:%s" % (repo, repo))
	system ("mdtool setup rep-build %s" % repo)
	system ("rm -rf %s/*.dll %s/*.addin.xml" % (repo, repo))

def publish ():
	repo = escape (REPO_DIR)
	system ("rsync -rve ssh --delete %s/* %s" % (repo, REPO_SCP))

def manifests (asm=None):
	dir = "."
	if asm:
		ls = map (abspath, shsp ('ls'))
		common = [(commonprefix ([p, asm]), p) for p in ls]
		dir = abspath (max (common)[1])
		dir = escape (dir) 
	find = 'find %s -name *.addin.xml' % dir
	return map (abspath, shsp (find))

def assemblies (config=None):
	config = config or REPO_CONFIG
	find = 'find . -path *%s* -name *.dll' % config
	return map (abspath, shsp (find))

def escape (path):
	return path.replace (' ', '\ ')

def shsp (*lines):
	return sh (*lines).splitlines ()

def sh (*lines):
	"""Pipe commands together and return the output.

	Examples:
		ls1 = sh ("ls")
		ls2 = sh ("ls", "grep Hello")
		ps1 = sh ("ps -aux", "grep firefox")
	"""
	out = None
	for argv in map (to_argv, lines):
		if out:
			out = Popen (argv, stdin=out.stdout, stdout=PIPE)
		else:
			out = Popen (argv, stdout=PIPE)
	if out:
		return out.communicate ()[0]
	else:
		return None

def to_argv (line):
	mask = lambda s: s.replace ('\ ', '____')
	unmask = lambda s: s.replace ('____', ' ')
	return map (unmask, mask (line).split ())

if __name__ == "__main__":
	import sys
	main (sys.argv)

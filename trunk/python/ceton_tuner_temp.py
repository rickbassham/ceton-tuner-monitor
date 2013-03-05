#!/usr/bin/env python
"""
munin Ceton InfiniTV4 Tuner Temperature Monitor

Draws tuner temperature in C.
"""

import sys
import urllib
import re

url = 'http://192.168.200.1/get_var.json?i=%d&s=diag&v=Temperature'

re_Temp = re.compile("\{\s+\"result\":\s+\"([\d.]+)\s+C\"\s+}")

if len(sys.argv) == 2 and sys.argv[1] == "autoconf":
	print "yes"

elif len(sys.argv) == 2 and sys.argv[1] == "config":
	print 'graph_title Ceton InfiniTV4 Tuner Temperature'
	print 'graph_vlabel Temperature in C'
	print 'graph_category sensors'

	print 'tuner1temp.label Tuner 1'
	print 'tuner1temp.type GAUGE'
	print 'tuner2temp.label Tuner 2'
	print 'tuner2temp.type GAUGE'
	print 'tuner3temp.label Tuner 3'
	print 'tuner3temp.type GAUGE'
	print 'tuner4temp.label Tuner 4'
	print 'tuner4temp.type GAUGE'

else:
	for x in range(0,4):
		u = urllib.urlopen(url % x)
		txt = u.read()
		u.close()

		temp = re_Temp.findall(txt)[0]

		print 'tuner%dtemp.value %s' % (x, temp)

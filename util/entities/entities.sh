#!/bin/bash
# Super simple script for parsing the W3C's entity list into something less javascripty
curl -s 'http://dev.w3.org/html5/spec/entities.json' \
| grep ';' \
| sed -r 's/,\s*"characters"\s*:.*$//' \
| sed -r 's/"codepoints":/,/' \
| sed -r 's/[^a-zA-Z0-9,]+//g'

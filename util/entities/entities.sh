#!/bin/bash
# Super simple script for converting the W3C's entity list into something that isn't JSON
# This script is SLOW, and generates a binary file containing entity definitions in the
# following easy-to-read format:
# 
# 1. 8 bits denoting the number of characters in the entity name
# 2. A sequence of 8 bit ASCII characters, the entity name
# 3. 8 bits denoting the number of codepoints in the entity value
# 4. A sequence of 32 bit UTF32 codepoints, the entity value

data="$(dirname "$0")/entities.json";

if [ ! -f "$data" ]; then
	curl -s 'http://dev.w3.org/html5/spec/entities.json' > "$data";
fi

cat "$data" \
| grep ';' \
| sed -r 's/,\s*"characters"\s*:.*$//' \
| sed -r 's/"codepoints":/,/' \
| sed -r 's/[^a-zA-Z0-9,]+//g' \
| sed -r 's/,/\t/g' \
| sort \
| while read -a fields; do
	# 8 bits indicating the number of characters in the entity name
	echo -ne "\x$(printf '%02X' ${#fields[0]})";
	# The entity name, as a sequence of 8-bit characters
	echo -n "${fields[0]}";
	# 8 bits announcing the number of 32-bit codepoints making up the entity
	echo -ne "\x$(printf '%02X' $((${#fields[*]} - 1)))";
	# The 32-bit codepoints represented by the entity
	for cp in ${fields[@]:1}; do
		echo -ne "\x$(printf '%02X' $((($cp >> 24) & 0xFF)))";
		echo -ne "\x$(printf '%02X' $((($cp >> 16) & 0xFF)))";
		echo -ne "\x$(printf '%02X' $((($cp >> 8) & 0xFF)))";
		echo -ne "\x$(printf '%02X' $((($cp >> 0) & 0xFF)))";
	done
done
